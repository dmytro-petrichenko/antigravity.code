export type GridData = Float32Array;

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
    | { type: 'ROTATE_VIEW'; payload: { deltaX: number; deltaY: number } };

export type WorkerMessage = SceneEvent;
