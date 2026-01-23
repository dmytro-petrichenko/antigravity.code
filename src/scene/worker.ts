import { SceneEngine } from './SceneEngine';
import { WorkerMessage } from './types';

let engine: SceneEngine | null = null;

self.onmessage = (event: MessageEvent<WorkerMessage>) => {
    const { type, payload } = event.data;

    switch (type) {
        case 'INIT':
            engine = new SceneEngine(payload);
            engine.onContextUpdate = (context) => {
                self.postMessage({
                    type: 'SAMPLING_CONTEXT_UPDATED',
                    payload: context
                });
            };
            break;
        case 'UPDATE_GRID':
            engine?.updateGrid(payload);
            break;
        case 'ROTATE_VIEW':
            engine?.rotateView(payload.deltaX, payload.deltaY);
            break;
        case 'RESIZE':
            engine?.resize(payload.width, payload.height);
            break;
    }
};
