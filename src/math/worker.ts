import { MathEngine } from './MathEngine';
import { FormulaChanged, ScaleChanged } from './types';

// Define Worker Message Types locally or import if shared effectively
// For simplicity, defining structure here matching checks
type WorkerMessage =
    | { type: 'INIT' }
    | { type: 'UPDATE_FORMULA'; payload: FormulaChanged }
    | { type: 'UPDATE_SCALE'; payload: ScaleChanged };

const engine = new MathEngine();

self.onmessage = (event: MessageEvent<WorkerMessage>) => {
    const { type } = event.data;

    switch (type) {
        case 'INIT':
            // Just ready to start, maybe send initial grid if needed, 
            // but usually we wait for formula
            break;
        case 'UPDATE_FORMULA':
            engine.updateFormula((event.data as any).payload.expression);
            broadcastGrid();
            break;
        case 'UPDATE_SCALE':
            engine.updateScale((event.data as any).payload.factor);
            broadcastGrid();
            break;
    }
};

function broadcastGrid() {
    const points = engine.computeGrid();
    // Transferable if possible, but Float32Array buffer is good to transfer
    self.postMessage({
        type: 'GRID_UPDATED',
        payload: { points }
    }, [points.buffer]);
}
