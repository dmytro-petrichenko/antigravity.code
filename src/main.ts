import { InteractionContext } from './interaction/InteractionContext';
import { FormulaChanged, ScaleChanged, ViewRotated } from './interaction/domain/events';

// Initialize Interaction Context
const interaction = new InteractionContext('controls-container');

// Initialize Scene Worker
const sceneWorker = new Worker(new URL('./scene/worker.ts', import.meta.url), { type: 'module' });

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

// Orchestration
interaction.subscribe((event) => {
    if (event instanceof ViewRotated) {
        sceneWorker.postMessage({
            type: 'ROTATE_VIEW',
            payload: { deltaX: event.deltaX, deltaY: event.deltaY }
        });
    } else if (event instanceof FormulaChanged) {
        console.log('Formula updated:', event.expression);
        // TODO: Send to Math Worker, then result to Scene Worker
    } else if (event instanceof ScaleChanged) {
        console.log('Scale changed:', event.factor);
        // TODO: Adjust Scene scale
    }
});

console.log('Application Initialized');
