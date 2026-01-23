import { InteractionContext } from './interaction/InteractionContext';
import { FormulaChanged, ScaleChanged, ViewRotated } from './interaction/domain/events';

// Initialize Interaction Context
const interaction = new InteractionContext('controls-container');

// Initialize Scene Worker
const sceneWorker = new Worker(new URL('./scene/worker.ts', import.meta.url), { type: 'module' });

// Initialize Math Worker
const mathWorker = new Worker(new URL('./math/worker.ts', import.meta.url), { type: 'module' });

// Canvas Setup
const canvas = document.getElementById('scene-canvas') as HTMLCanvasElement;
if (canvas) {
    // Wait for resize to know size, or init immediately
    const offscreen = canvas.transferControlToOffscreen();

    sceneWorker.postMessage({
        type: 'INIT',
        payload: {
            canvas: offscreen,
            width: window.innerWidth,
            height: window.innerHeight,
            pixelRatio: window.devicePixelRatio
        }
    }, [offscreen]);

    window.addEventListener('resize', () => {
        sceneWorker.postMessage({
            type: 'RESIZE',
            payload: {
                width: window.innerWidth,
                height: window.innerHeight
            }
        });
    });
}

// Coordinate Workers
mathWorker.onmessage = (event) => {
    const { type, payload } = event.data;
    if (type === 'GRID_UPDATED') {
        sceneWorker.postMessage({
            type: 'UPDATE_GRID',
            payload: payload.points
        }, [payload.points.buffer]);
    }
};

// Orchestration
interaction.subscribe((event) => {
    if (event instanceof ViewRotated) {
        sceneWorker.postMessage({
            type: 'ROTATE_VIEW',
            payload: { deltaX: event.deltaX, deltaY: event.deltaY }
        });
    } else if (event instanceof FormulaChanged) {
        console.log('Formula updated:', event.expression);
        mathWorker.postMessage({
            type: 'UPDATE_FORMULA',
            payload: { expression: event.expression }
        });
    } else if (event instanceof ScaleChanged) {
        console.log('Scale changed:', event.factor);
        mathWorker.postMessage({
            type: 'UPDATE_SCALE',
            payload: { factor: event.factor }
        });
    }
});

// Initial kick-off
mathWorker.postMessage({
    type: 'UPDATE_FORMULA',
    payload: { expression: 'z = x * y' }
});

console.log('Application Initialized');
