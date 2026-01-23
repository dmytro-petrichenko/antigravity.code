import { GridConfig } from "../types";
import { Formula } from "./formula";

export class Grid {
    public static generate(formula: Formula, config: GridConfig): Float32Array {
        const { range, step } = config;
        const countX = Math.floor((range.max - range.min) / step) + 1;
        const countY = Math.floor((range.max - range.min) / step) + 1;

        // 3 coordinates (x, y, z) per point
        const buffer = new Float32Array(countX * countY * 3);
        let index = 0;

        for (let x = range.min; x <= range.max; x += step) {
            for (let y = range.min; y <= range.max; y += step) {
                const z = formula.evaluate(x, y);

                buffer[index++] = x;
                buffer[index++] = y;
                buffer[index++] = z;
            }
        }

        return buffer;
    }
}
