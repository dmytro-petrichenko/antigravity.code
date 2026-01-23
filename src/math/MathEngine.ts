import { ExpressionParser, ExpressionNode } from './ExpressionParser';

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

    computeGrid(): Float32Array {
        if (!this.currentExpressionNode) {
            return new Float32Array(0);
        }

        const { xRange, yRange, step } = this.space;
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
                points[ptr++] = x * this.currentZoom;
                points[ptr++] = y * this.currentZoom;
                points[ptr++] = z * this.currentZoom;
            }
        }

        return points;
    }
}
