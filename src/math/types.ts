export type Point = [number, number, number]; // x, y, z

export interface GridConfig {
    range: { min: number; max: number };
    step: number;
}

export interface Formula {
    expression: string;
}

export interface FormulaChanged {
    expression: string;
}

export interface ScaleChanged {
    factor: number;
}

export interface GridUpdated {
    points: Float32Array;
}
