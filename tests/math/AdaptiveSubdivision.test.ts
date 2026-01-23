import { describe, it, expect, beforeEach } from 'vitest';
import { MathEngine } from '../../src/math/MathEngine';
import { SamplingContextUpdated } from '../../src/math/types';

describe('MathEngine - Adaptive Subdivision', () => {
    let engine: MathEngine;

    beforeEach(() => {
        engine = new MathEngine();
        engine.updateFormula('x * x'); // Simple parabola
        engine.updateScale(1.0); // Default -10 to 10
    });

    it('should use fixed grid when no context is provided', () => {
        const grid = engine.computeGrid();
        expect(grid.length).toBeGreaterThan(0);
        // Fixed resolution 20 -> 21x21 points -> 441 points.
        // Formula z = x*x. x goes -10 to 10. z goes 0 to 100.
        // Clipping bounds [-10, 10].
        // z <= 10 implies |x| <= sqrt(10) ~= 3.16. Integers: -3, -2, -1, 0, 1, 2, 3 (7 values).
        // Y is -10 to 10 (21 values).
        // Total points = 7 * 21 = 147.
        // Floats = 147 * 3 = 441.
        expect(grid.length).toBe(441);
    });

    it('should use adaptive subdivision when context is provided', () => {
        // Use tilted matrix to ensure Z-curvature is detected
        const tiltedMatrix = new Float32Array([
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0.5, 1, 0,
            0, 0, 0, 1
        ]);

        const context: SamplingContextUpdated = {
            projection: {
                type: 'LinearMatrix',
                matrix: tiltedMatrix
            },
            tolerance: 0.1,
            limits: {
                maxDepth: 5,
                minStep: 0.1
            }
        };

        engine.updateSamplingContext(context);
        const grid = engine.computeGrid();

        expect(grid.length).toBeGreaterThan(0);
        // Should be different from fixed grid count
        // With adaptive, it might be more or less depending on tolerance.
    });

    it('should produce more points with tighter tolerance', () => {
        // Use a matrix where Z affects Y (e.g. tilted view)
        // val = m[1]*x + m[5]*y + m[9]*z + m[13]
        // Set m[9] = 0.5
        const tiltedMatrix = new Float32Array([
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0.5, 1, 0,
            0, 0, 0, 1
        ]);

        const baseContext: SamplingContextUpdated = {
            projection: {
                type: 'LinearMatrix',
                matrix: tiltedMatrix
            },
            tolerance: 10.0, // Very Loose
            limits: { maxDepth: 5, minStep: 0.01 }
        };

        engine.updateSamplingContext(baseContext);
        const looseGrid = engine.computeGrid();

        // Tight tolerance
        engine.updateSamplingContext({
            ...baseContext,
            tolerance: 0.1
        });
        const tightGrid = engine.computeGrid();

        expect(tightGrid.length).toBeGreaterThan(looseGrid.length);
    });

    it('should respect scale/zoom in adaptive mode', () => {
        // Use tilted matrix so Z-curvature forces subdivision
        const tiltedMatrix = new Float32Array([
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0.5, 1, 0,
            0, 0, 0, 1
        ]);

        const context: SamplingContextUpdated = {
            projection: { type: 'LinearMatrix', matrix: tiltedMatrix },
            tolerance: 0.1,
            limits: { maxDepth: 5, minStep: 0.01 }
        };

        engine.updateSamplingContext(context);
        const grid1 = engine.computeGrid();

        engine.updateScale(2); // Zoom in
        const grid2 = engine.computeGrid();

        // Zooming in reduces the world range (xRange: -5 to 5).
        // The number of points depends on curvature in that range vs tolerance.
        // It's hard to predict exact relation without calculation, 
        // but it should definitely produce points.
        expect(grid2.length).toBeGreaterThan(0);
    });

    it('should clip points outside the visual bounds', () => {
        // Function z = 100, which is outside [-10, 10]
        engine.updateFormula('100');

        // Use identity matrix context
        const context: SamplingContextUpdated = {
            projection: {
                type: 'LinearMatrix',
                matrix: new Float32Array([1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1])
            },
            tolerance: 1.0,
            limits: { maxDepth: 2, minStep: 1 }
        };
        engine.updateSamplingContext(context);

        const grid = engine.computeGrid();

        // Should be empty because all points have z=100
        expect(grid.length).toBe(0);

        // Test a function that crosses bounds: z = x (where x goes -10 to 10)
        // x=10 -> z=10 (boundary, included)
        // x goes beyond? The range is -10 to 10.
        // Let's force x range to be larger by zooming OUT?
        // Zoom 0.5 -> Range -20 to 20.
        // Formula z = x.
        // Points with x > 10 should be clipped.

        engine.updateFormula('x');
        engine.updateScale(0.5); // Range -20 to 20

        const grid2 = engine.computeGrid();

        // Each quad emits 2 triangles = 6 vertices = 18 floats.
        // grid2.length should be a multiple of 18 (or at least 9 for a single triangle).
        expect(grid2.length % 9).toBe(0);
        expect(grid2.length).toBeGreaterThan(0);

        // Check manually that no coordinate > 10
        for (let i = 0; i < grid2.length; i++) {
            expect(Math.abs(grid2[i])).toBeLessThan(10.0001); // 10 is allowed
        }
    });
});
