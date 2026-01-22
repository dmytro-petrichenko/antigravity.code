import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import * as THREE from 'three';
import { SceneEngine } from '../../src/scene/SceneEngine';
import { GridData } from '../../src/scene/types';

// Mock Three.js WebGLRenderer
vi.mock('three', async () => {
    const actual = await vi.importActual('three');
    return {
        ...actual,
        WebGLRenderer: vi.fn().mockImplementation(() => ({
            render: vi.fn(),
            setPixelRatio: vi.fn(),
            setSize: vi.fn(),
            dispose: vi.fn(),
        })),
    };
});

describe('SceneEngine', () => {
    let canvasMock: OffscreenCanvas;

    beforeEach(() => {
        // Mock OffscreenCanvas
        canvasMock = {
            width: 800,
            height: 600,
            getContext: vi.fn(),
            addEventListener: vi.fn(),
            removeEventListener: vi.fn(),
            dispatchEvent: vi.fn(),
        } as unknown as OffscreenCanvas;

        // Stub requestAnimationFrame
        vi.stubGlobal('requestAnimationFrame', (fn: any) => setTimeout(fn, 0));
    });

    afterEach(() => {
        vi.clearAllMocks();
    });

    it('should initialize without crashing', () => {
        const engine = new SceneEngine({
            canvas: canvasMock,
            width: 800,
            height: 600,
            pixelRatio: 1,
        });
        expect(engine).toBeDefined();
        expect(THREE.WebGLRenderer).toHaveBeenCalled();
    });

    it('should update grid and create a mesh', () => {
        const engine = new SceneEngine({
            canvas: canvasMock,
            width: 800,
            height: 600,
            pixelRatio: 1,
        });

        // Create dummy grid data (4 points = 2x2 grid)
        const gridData: GridData = new Float32Array([
            0, 0, 0,
            1, 0, 0,
            0, 1, 0,
            1, 1, 0
        ]);

        // Spy on Scene.add
        // We can't easily spy on the private scene property, but we can verify execution logic doesn't throw
        // and potentially spy on THREE.Scene.prototype.add if we want to be strict.
        const addSpy = vi.spyOn(THREE.Scene.prototype, 'add');

        engine.updateGrid(gridData);

        expect(addSpy).toHaveBeenCalled();
        // One for light, one for mesh (or more lights)
        // We expect at least the lights + the new mesh
    });

    it('should resize', () => {
        const engine = new SceneEngine({
            canvas: canvasMock,
            width: 800,
            height: 600,
            pixelRatio: 1,
        });

        engine.resize(1024, 768);
        // Verified by check in mock call if needed, or just ensuring no crash
    });
});
