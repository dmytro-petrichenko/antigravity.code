import { describe, it, expect } from "vitest";
import { Grid } from "../../src/math/domain/grid";
import { Formula } from "../../src/math/domain/formula";

describe("Grid", () => {
    it("should generate correct number of points", () => {
        const formula = new Formula("x * y");
        const config = { range: { min: 0, max: 2 }, step: 1 };
        // Points: 0, 1, 2 = 3 points per axis
        // Total points = 3 * 3 = 9
        // Float32Array size = 9 * 3 (x,y,z) = 27

        const grid = Grid.generate(formula, config);
        expect(grid.length).toBe(27);
    });

    it("should calculate correct z values", () => {
        const formula = new Formula("x + y");
        const config = { range: { min: 0, max: 1 }, step: 1 };
        // Points: (0,0), (0,1), (1,0), (1,1)

        const grid = Grid.generate(formula, config);

        // Structure: x, y, z, ...

        // (0,0) -> z=0
        expect(grid[0]).toBe(0);
        expect(grid[1]).toBe(0);
        expect(grid[2]).toBe(0);

        // (0,1) -> z=1
        expect(grid[3]).toBe(0);
        expect(grid[4]).toBe(1);
        expect(grid[5]).toBe(1);

        // (1,0) -> z=1
        expect(grid[6]).toBe(1);
        expect(grid[7]).toBe(0);
        expect(grid[8]).toBe(1);

        // (1,1) -> z=2
        expect(grid[9]).toBe(1);
        expect(grid[10]).toBe(1);
        expect(grid[11]).toBe(2);
    });
});
