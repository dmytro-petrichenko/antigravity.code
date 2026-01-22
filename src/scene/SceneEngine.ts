import * as THREE from 'three';
import { SceneOptions, GridData } from './types';

export class SceneEngine {
    private scene: THREE.Scene;
    private camera: THREE.PerspectiveCamera;
    private renderer: THREE.WebGLRenderer;
    private mesh: THREE.Mesh | null = null;
    private isRendering = false;

    constructor(options: SceneOptions) {
        const { canvas, width, height, pixelRatio } = options;

        this.scene = new THREE.Scene();
        this.scene.background = new THREE.Color(0x1a1a1a);

        this.camera = new THREE.PerspectiveCamera(75, width / height, 0.1, 1000);
        this.camera.position.z = 5;

        // We cast canvas to any because Three.js types might not strictly align with OffscreenCanvas without specific overrides,
        // although it usually supports it.
        this.renderer = new THREE.WebGLRenderer({ canvas: canvas as unknown as HTMLCanvasElement, antialias: true });
        this.renderer.setPixelRatio(pixelRatio);
        this.renderer.setSize(width, height, false);

        // Initial basic lighting
        const ambientLight = new THREE.AmbientLight(0xffffff, 0.5);
        this.scene.add(ambientLight);

        const directionalLight = new THREE.DirectionalLight(0xffffff, 1);
        directionalLight.position.set(1, 1, 1);
        this.scene.add(directionalLight);

        // Start render loop
        this.start();
    }

    public updateGrid(data: GridData) {
        // Clean up old mesh
        if (this.mesh) {
            this.scene.remove(this.mesh);
            this.mesh.geometry.dispose();
            (this.mesh.material as THREE.Material).dispose();
        }

        const vertexCount = data.length / 3;
        const gridSize = Math.sqrt(vertexCount);

        // Validate if it's a square grid
        if (!Number.isInteger(gridSize)) {
            console.warn('Grid data does not represent a square grid. Rendering points instead.');
            // Fallback to points
            const geometry = new THREE.BufferGeometry();
            geometry.setAttribute('position', new THREE.BufferAttribute(data, 3));
            const material = new THREE.PointsMaterial({ color: 0x00ff00, size: 0.1 });
            this.mesh = new THREE.Points(geometry, material) as unknown as THREE.Mesh;
            this.scene.add(this.mesh);
            return;
        }

        const geometry = new THREE.BufferGeometry();
        geometry.setAttribute('position', new THREE.BufferAttribute(data, 3));

        // Generate indices for a grid
        const indices: number[] = [];
        for (let z = 0; z < gridSize - 1; z++) {
            for (let x = 0; x < gridSize - 1; x++) {
                const a = z * gridSize + x;
                const b = z * gridSize + x + 1;
                const c = (z + 1) * gridSize + x;
                const d = (z + 1) * gridSize + x + 1;

                // Two triangles per quad
                indices.push(a, b, d);
                indices.push(a, d, c);
            }
        }
        geometry.setIndex(indices);
        geometry.computeVertexNormals();

        const material = new THREE.MeshBasicMaterial({
            color: 0x00ff00,
            wireframe: true,
            side: THREE.DoubleSide
        });

        this.mesh = new THREE.Mesh(geometry, material);
        this.scene.add(this.mesh);
    }

    public rotateView(deltaX: number, deltaY: number) {
        // Spherical orbit logic
        const offset = new THREE.Vector3();
        offset.copy(this.camera.position).sub(new THREE.Vector3(0, 0, 0));

        const spherical = new THREE.Spherical();
        spherical.setFromVector3(offset);

        // Rotate around Y axis (theta)
        spherical.theta -= deltaX * 0.005;

        // Rotate around X axis (phi, pitch)
        spherical.phi -= deltaY * 0.005;

        // Clamp phi to avoid flipping over poles
        spherical.phi = Math.max(0.1, Math.min(Math.PI - 0.1, spherical.phi));

        offset.setFromSpherical(spherical);

        this.camera.position.copy(offset);
        this.camera.lookAt(0, 0, 0);
    }

    public resize(width: number, height: number) {
        this.camera.aspect = width / height;
        this.camera.updateProjectionMatrix();
        this.renderer.setSize(width, height, false);
    }

    private start() {
        if (this.isRendering) return;
        this.isRendering = true;
        this.animate();
    }

    private animate = () => {
        if (!this.isRendering) return;

        requestAnimationFrame(this.animate);
        this.render();
    };

    private render() {
        this.renderer.render(this.scene, this.camera);
    }

    public stop() {
        this.isRendering = false;
    }
}
