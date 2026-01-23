export type GridData = Float32Array;

export interface ProjectionDescriptor {
    type: "LinearMatrix" | "Custom";
    matrix?: Float32Array;
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

export interface SceneOptions {
    canvas: OffscreenCanvas;
    width: number;
    height: number;
    pixelRatio: number;
}

export type SceneEvent =
    | { type: 'INIT'; payload: SceneOptions }
    | { type: 'UPDATE_GRID'; payload: GridData }
    | { type: 'RESIZE'; payload: { width: number; height: number } }
    | { type: 'ROTATE_VIEW'; payload: { deltaX: number; deltaY: number } }
    | { type: 'SAMPLING_CONTEXT_UPDATED'; payload: SamplingContextUpdated };

export type WorkerMessage = SceneEvent;
