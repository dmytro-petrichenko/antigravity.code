import { describe, it, expect, beforeEach } from 'vitest';
import { MathEngine } from '../../src/math/MathEngine';

describe('MathEngine', () => {
    let engine: MathEngine;

    beforeEach(() => {
        engine = new MathEngine();
    });

    it('should parse valid formula and generate non-empty grid', () => {
        engine.updateFormula('z = x * y');
        const grid = engine.computeGrid();
        expect(grid.length).toBeGreaterThan(0);
        // Default range -10 to 10, step 1 -> 21 points each dimension -> 21*21 = 441 points -> 441*3 = 1323 floats
        // However, implementation check: floor((20)/1) + 1 = 21. 
        expect(grid.length).toBe(21 * 21 * 3);
    });

    it('should calculate correct values for z = x * y', () => {
        // Use a smaller range/step for easier verification
        engine.updateScale(0.1); // Range -1 to 1
        engine.setStep(1); // -1, 0, 1

        engine.updateFormula('x * y');
        const grid = engine.computeGrid();

        // Expected points:
        // x=-1: y=-1(z=1), y=0(z=0), y=1(z=-1)
        // x=0:  y=-1(z=0), y=0(z=0), y=1(z=0)
        // x=1:  y=-1(z=-1), y=0(z=0), y=1(z=1)

        // 3x3 = 9 points * 3 coords = 27 floats
        expect(grid.length).toBe(27);

        // Check a few manually
        // First point (-1, -1, 1)
        expect(grid[0]).toBeCloseTo(-1);
        expect(grid[1]).toBeCloseTo(-1);
        expect(grid[2]).toBeCloseTo(1);

        // Center point (0, 0, 0)
        // Index: 
        // x=0 is the 2nd column (i=1)
        // y=0 is the 2nd row (j=1)
        // index = (i * ySteps + j) * 3
        // index = (1 * 3 + 1) * 3 = 12
        expect(grid[12]).toBeCloseTo(0); // x
        expect(grid[13]).toBeCloseTo(0); // y
        expect(grid[14]).toBeCloseTo(0); // z
    });

    it('should handle constants', () => {
        engine.updateFormula('z = 5');
        engine.updateScale(0.1);
        engine.setStep(1);
        const grid = engine.computeGrid();

        // All z should be 5
        for (let i = 2; i < grid.length; i += 3) {
            expect(grid[i]).toBe(5);
        }
    });

    it('should handle division', () => {
        engine.updateFormula('x / 2');
        engine.updateScale(0.1); // -1 to 1
        engine.setStep(1);

        const grid = engine.computeGrid();

        // x=-1 -> z=-0.5
        // x=0 -> z=0
        // x=1 -> z=0.5

        // check x=1, y=0 (i=2, j=1) -> index (2*3 + 1)*3 = 21
        // Wait: i=0,1,2. x=-1,0,1.

        // x=-1 (i=0): y=-1,0,1
        // x=0  (i=1)
        // x=1  (i=2)

        // Check x=-1, y=-1 (index 0)
        expect(grid[2]).toBe(-0.5);
    });

    it('should throw error on invalid formula', () => {
        expect(() => {
            engine.updateFormula('x # y');
        }).toThrow();
    });

    it('should update grid when scale changes', () => {
        engine.updateFormula('x');
        // Default scale 1 -> -10 to 10
        const grid1 = engine.computeGrid();

        engine.updateScale(2); // -20 to 20
        const grid2 = engine.computeGrid();

        expect(grid2.length).toBeGreaterThan(grid1.length);
    });
});
