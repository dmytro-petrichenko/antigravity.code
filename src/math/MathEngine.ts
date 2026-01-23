import { ExpressionParser, ExpressionNode } from './ExpressionParser';
import { SamplingContextUpdated, ProjectionDescriptor, SubdivisionLimits } from './types';

export interface CoordinateSpace {
    xRange: { min: number; max: number };
    yRange: { min: number; max: number };
    step: number;
}

export class MathEngine {
    private parser: ExpressionParser;
    private currentExpressionNode: ExpressionNode | null = null;
    private space: CoordinateSpace = {
        xRange: { min: -10, max: 10 },
        yRange: { min: -10, max: 10 },
        step: 1
    };

    private currentZoom: number = 1.0;
    private readonly baseRange: number = 10;
    private readonly RESOLUTION: number = 20;

    private samplingContext: SamplingContextUpdated | null = null;

    constructor() {
        this.parser = new ExpressionParser();
    }

    updateFormula(expression: string): void {
        this.currentExpressionNode = this.parser.parse(expression);
    }

    updateScale(scale: number): void {
        // scale is a multiplication factor from the UI (e.g. 1.1 or 0.9)
        this.currentZoom *= scale;

        // Zoom In (Higher Zoom) -> Smaller Range
        const newMax = this.baseRange / this.currentZoom;
        const newMin = -newMax;

        this.space.xRange = { min: newMin, max: newMax };
        this.space.yRange = { min: newMin, max: newMax };
    }

    // Allow setting step directly for testing/configuration if needed, 
    // although not explicitly in "Inbound Events"
    setStep(step: number) {
        this.space.step = step;
    }

    updateSamplingContext(context: SamplingContextUpdated) {
        this.samplingContext = context;
    }

    private project(x: number, y: number, z: number): { x: number, y: number } {
        if (!this.samplingContext || !this.samplingContext.projection.matrix) {
            // Fallback if no projection: Identity (orthographic top-down)
            return { x, y };
        }

        const m = this.samplingContext.projection.matrix;

        // 4x4 Matrix multiplication (Column-Major)
        // x' = m0*x + m4*y + m8*z + m12
        // y' = m1*x + m5*y + m9*z + m13
        // z' = m2*x + m6*y + m10*z + m14
        // w' = m3*x + m7*y + m11*z + m15

        const w = m[3] * x + m[7] * y + m[11] * z + m[15];
        if (Math.abs(w) < 1e-9) {
            // Degenerate case, return 0,0 or something safe
            return { x: 0, y: 0 };
        }

        const screenX = (m[0] * x + m[4] * y + m[8] * z + m[12]) / w;
        const screenY = (m[1] * x + m[5] * y + m[9] * z + m[13]) / w;

        // We only care about 2D screen distance for error metric
        return { x: screenX, y: screenY };
    }

    private distance(p1: { x: number, y: number }, p2: { x: number, y: number }): number {
        const dx = p1.x - p2.x;
        const dy = p1.y - p2.y;
        return Math.sqrt(dx * dx + dy * dy);
    }

    private evalPoint(x: number, y: number) {
        if (!this.currentExpressionNode) return { x, y, z: 0 };
        // NOTE: The formula results are in "Math Space".
        // Visuals apply currentZoom. 
        // We should perform adaptive logic on the VISUAL coordinates because that's what the camera sees.
        // So we apply currentZoom to x,y before checking?
        // Actually, the `space` ranges are already adjusted by zoom. 
        // If xRange is -5 to 5, that corresponds to screen -10 to 10 if zoom is 2.
        // Wait, updateScale updates xRange/yRange based on zoom.
        // So x, y are in World Space (Post-Zoom equivalent? No).

        // Let's trace:
        // updateScale: newMax = 10 / zoom. 
        // If zoom=2, max=5. range is -5..5.
        // In computeGrid loop, x goes from -5 to 5.
        // evaluate(x,y) gives z.
        // Then we do x*zoom, y*zoom, z*zoom. This scales it back to -10..10.
        // So the "World Point" sent to Scene is (x*Z, y*Z, z*Z).
        // This World Point is what should be projected.

        const z = this.parser.evaluate(this.currentExpressionNode, x, y);

        const wx = x * this.currentZoom;
        const wy = y * this.currentZoom;
        const wz = z * this.currentZoom;

        return { x: wx, y: wy, z: wz };
    }


    private subdivide(
        p0: { x: number, y: number, z: number },
        p1: { x: number, y: number, z: number }, // Diagonal of the current quad/block
        p2: { x: number, y: number, z: number }, // Other corner
        depth: number,
        points: number[]
    ) {
        if (!this.samplingContext) return;
        const { tolerance, limits } = this.samplingContext;

        const p0_proj = this.project(p0.x, p0.y, p0.z);
        const p1_proj = this.project(p1.x, p1.y, p1.z);
        const p2_proj = this.project(p2.x, p2.y, p2.z);

        // Check midpoint of the diagonal p0-p1
        const midX = (p0.x + p1.x) / 2; // World space average?
        // Actually, we are iterating over parameter space (x, y). 
        // We should subdivide in parameter space.

        // Let's change strategy: Recursive Quadtree on Domain (u, v)
        // This function signature is messy for that.
    }

    // Recursive function for adaptive sampling on the domain
    private sampleQuad(
        minX: number, maxX: number,
        minY: number, maxY: number,
        depth: number,
        outputPoints: number[]
    ) {
        if (!this.currentExpressionNode || !this.samplingContext) return;
        const { tolerance, limits } = this.samplingContext;

        // Evaluate corners
        // We need 4 corners + center to check error
        // Or just check if linear interpolation matches the center value

        // Let's use the error metric defined in spec:
        // error = distance( project(mid(surface)), midpoint(project(p0), project(p1)) )
        // p0, p1 are corners. 

        // Let's look at the diagonal from (minX, minY) to (maxX, maxY)
        // p0 = eval(minX, minY)
        // p1 = eval(maxX, maxY)
        // midParam = eval((minX+maxX)/2, (minY+maxY)/2)

        // Wait, we need to respect minStep constraints too.
        if (maxX - minX < limits.minStep) {
            // Too small, stop and emit points
            // We emit the midpoint or the corners?
            // "GridUpdated" expects points. Since it's a triangle soup or points, 
            // let's output the center point of this quad to represent it, 
            // OR output the corners if it's the leaf.
            // If we output corners, we will have duplicates. 
            // Ideally we output a regular grid, but adaptive means it's irregular.
            // Let's output the center point for now as a simplification, or the top-left?
            // If we assume Points rendering, one point per leaf quad is fine.

            const cx = (minX + maxX) / 2;
            const cy = (minY + maxY) / 2;
            const p = this.evalPoint(cx, cy);
            outputPoints.push(p.x, p.y, p.z);
            return;
        }

        const cx = (minX + maxX) / 2;
        const cy = (minY + maxY) / 2;
        const pMidReal = this.evalPoint(cx, cy);

        // Diagonal 1: (minX, minY) -> (maxX, maxY)
        const p0 = this.evalPoint(minX, minY);
        const p1 = this.evalPoint(maxX, maxY);

        const projP0 = this.project(p0.x, p0.y, p0.z);
        const projP1 = this.project(p1.x, p1.y, p1.z);
        const projMidReal = this.project(pMidReal.x, pMidReal.y, pMidReal.z);

        const projMidLinear = {
            x: (projP0.x + projP1.x) / 2,
            y: (projP0.y + projP1.y) / 2
        };

        const dist = this.distance(projMidReal, projMidLinear);

        // If error > tolerance and depth < maxDepth, subdivide
        if (dist > tolerance && depth < limits.maxDepth) {
            // Subdivide
            this.sampleQuad(minX, cx, minY, cy, depth + 1, outputPoints); // TL
            this.sampleQuad(cx, maxX, minY, cy, depth + 1, outputPoints); // TR
            this.sampleQuad(minX, cx, cy, maxY, depth + 1, outputPoints); // BL
            this.sampleQuad(cx, maxX, cy, maxY, depth + 1, outputPoints); // BR
        } else {
            // Leaf node
            // Emit 2 triangles (6 vertices) to form a solid surface
            // p00 (minX, minY)
            // p10 (maxX, minY)
            // p01 (minX, maxY)
            // p11 (maxX, maxY)

            const p00 = this.evalPoint(minX, minY);
            const p10 = this.evalPoint(maxX, minY);
            const p01 = this.evalPoint(minX, maxY);
            const p11 = this.evalPoint(maxX, maxY);

            // Triangle 1: p00 -> p10 -> p01
            if (this.isTriangleInBounds(p00, p10, p01)) {
                // console.log('Emit T1');
                outputPoints.push(p00.x, p00.y, p00.z);
                outputPoints.push(p10.x, p10.y, p10.z);
                outputPoints.push(p01.x, p01.y, p01.z);
            }

            // Triangle 2: p10 -> p11 -> p01
            if (this.isTriangleInBounds(p10, p11, p01)) {
                // console.log('Emit T2');
                outputPoints.push(p10.x, p10.y, p10.z);
                outputPoints.push(p11.x, p11.y, p11.z);
                outputPoints.push(p01.x, p01.y, p01.z);
            }
        }
    }

    private isTriangleInBounds(p1: { x: number, y: number, z: number }, p2: { x: number, y: number, z: number }, p3: { x: number, y: number, z: number }): boolean {
        // Strict: All vertices must be in bounds? Or relax? 
        // User asked "draw points within boundaries".
        // If we draw a triangle crossing boundary, it might look ugly or distinct.
        // Let's require all vertices to be inside [-10, 10].
        const limit = 10;
        const check = (p: { x: number, y: number, z: number }) =>
            Math.abs(p.x) <= limit && Math.abs(p.y) <= limit && Math.abs(p.z) <= limit;

        return check(p1) && check(p2) && check(p3);
    }

    computeGrid(): Float32Array {
        if (!this.currentExpressionNode) {
            return new Float32Array(0);
        }

        // usage of adaptive subdivision if context is available
        if (this.samplingContext) {
            const points: number[] = [];
            const { xRange, yRange } = this.space;

            // Start the recursive sampling
            this.sampleQuad(xRange.min, xRange.max, yRange.min, yRange.max, 0, points);

            return new Float32Array(points);
        }

        // Fallback to legacy fixed grid logic if no context (or for initial load)
        const { xRange, yRange } = this.space;

        // Calculate step dynamically based on fixed resolution
        const rangeWidth = xRange.max - xRange.min;
        const step = rangeWidth / this.RESOLUTION;

        const xSteps = Math.floor((xRange.max - xRange.min) / step) + 1;
        const ySteps = Math.floor((yRange.max - yRange.min) / step) + 1;

        // Storage: x, y, z for each point
        const points = new Float32Array(xSteps * ySteps * 3);
        let ptr = 0;

        for (let i = 0; i < xSteps; i++) {
            const x = xRange.min + i * step;
            for (let j = 0; j < ySteps; j++) {
                const y = yRange.min + j * step;

                const z = this.parser.evaluate(this.currentExpressionNode, x, y);

                // Scale spatial coordinates to normalize visual size
                const vx = x * this.currentZoom;
                const vy = y * this.currentZoom;
                const vz = z * this.currentZoom;

                // Clip to bounds (-10 to 10)
                if (Math.abs(vx) <= 10 && Math.abs(vy) <= 10 && Math.abs(vz) <= 10) {
                    points[ptr++] = vx;
                    points[ptr++] = vy;
                    points[ptr++] = vz;
                }
            }
        }

        // Resize buffer to actual points used (since we might have culled some)
        return points.slice(0, ptr);
    }
}
