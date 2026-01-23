export type Point = [number, number, number]; // x, y, z

export interface ProjectionDescriptor {
    type: "LinearMatrix" | "Custom";
    matrix?: Float32Array; // 4x4 column-major matrix
    // Custom projection function not serializable for basic events usually, 
    // but kept in mind for internal logic if needed. 
    // For pure data transfer, matrix is preferred.
}

export interface SubdivisionLimits {
    maxDepth: number;
    minStep: number;
}

export interface SamplingContextUpdated {
    projection: ProjectionDescriptor;
    tolerance: number;
    limits: SubdivisionLimits;
}

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
