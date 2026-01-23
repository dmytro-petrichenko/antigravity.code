import * as THREE from 'three';
import { SceneOptions, GridData, SamplingContextUpdated } from './types';

export class SceneEngine {
    private scene: THREE.Scene;
    private camera: THREE.PerspectiveCamera;
    private renderer: THREE.WebGLRenderer;
    private mesh: THREE.Mesh | null = null;
    private isRendering = false;

    public onContextUpdate: ((context: SamplingContextUpdated) => void) | null = null;
    private lastContextTimestamp = 0;

    constructor(options: SceneOptions) {
        const { canvas, width, height, pixelRatio } = options;

        this.scene = new THREE.Scene();
        this.scene.background = new THREE.Color(0x1a1a1a);

        this.camera = new THREE.PerspectiveCamera(75, width / height, 0.1, 1000);
        this.camera.position.z = 30;

        // We cast canvas to any because Three.js types might not strictly align with OffscreenCanvas without specific overrides,
        // although it usually supports it.
        this.renderer = new THREE.WebGLRenderer({ canvas: canvas as unknown as HTMLCanvasElement, antialias: true });
        this.renderer.setPixelRatio(pixelRatio);
        this.renderer.setSize(width, height, false);

        // Add Bounding Cube (20x20x20)
        const boxGeo = new THREE.BoxGeometry(20, 20, 20);
        const edges = new THREE.EdgesGeometry(boxGeo);
        const line = new THREE.LineSegments(edges, new THREE.LineBasicMaterial({ color: 0x0000ff }));
        this.scene.add(line);

        // Initial basic lighting
        const ambientLight = new THREE.AmbientLight(0xffffff, 0.5);
        this.scene.add(ambientLight);

        const directionalLight = new THREE.DirectionalLight(0xffffff, 1);
        directionalLight.position.set(1, 1, 1);
        this.scene.add(directionalLight);

        // Emit initial context
        // Defer slightly to ensure listeners are attached if needed, or just emit.
        // Since constructor is sync, caller needs to attach callback after instance creation.
        // We can emit in 'start' or explicit init.

        // Start render loop
        this.start();
        setTimeout(() => this.emitContext(), 0);
    }

    private emitContext() {
        if (this.onContextUpdate) {
            const context = this.getSamplingContext();
            this.onContextUpdate(context);
        }
    }

    private getSamplingContext(): SamplingContextUpdated {
        this.camera.updateMatrixWorld();
        this.camera.updateProjectionMatrix();

        const projectionMatrix = this.camera.projectionMatrix.clone();
        const viewMatrix = this.camera.matrixWorldInverse.clone();

        // View-Projection Matrix = Projection * View
        const vpMatrix = projectionMatrix.multiply(viewMatrix);

        // Tolerance: roughly 1.0 screen units? 
        // 1.0 might be too loose or tight depending on coordinate system.
        // In normalized device coordinates (NDC), range is -1 to 1.
        // Screen space usually refers to pixels? 
        // The project method in math returns screen coords.
        // Let's assume math project returns NDC (-1 to 1) if we just use VP matrix.
        // AND we want error in PIXELS.

        // Wait, three.js projection matrix transforms to Clip Coordinates (Homogeneous).
        // Division by w gives NDC (-1 to 1).
        // To get pixels, we multiply by half width/height.

        // The MathEngine project function I wrote:
        // screenX = (m[0]*x + ...)/w
        // This is X_NDC.

        // So the distance in MathEngine is distance in NDC.
        // 1 pixel in NDC is roughly 2.0 / height.

        // If we want sub-pixel accuracy (e.g. 0.5 pixel error), 
        // tolerance should be (1.0 / height) * 0.5 approx?
        // Let's pick a conservative tolerance in NDC.
        // e.g. 0.005

        const tolerance = 0.01; // Tunable

        return {
            projection: {
                type: 'LinearMatrix',
                matrix: new Float32Array(vpMatrix.elements)
            },
            tolerance: tolerance,
            limits: {
                maxDepth: 8,
                minStep: 0.01 // Minimal world space step
            }
        };
    }

    public updateGrid(data: GridData) {
        // Clean up old mesh
        if (this.mesh) {
            this.scene.remove(this.mesh);
            this.mesh.geometry.dispose();
            (this.mesh.material as THREE.Material).dispose();
        }

        const vertexCount = data.length / 3;

        // Adaptive data (Triangle Soup)
        // Render as Mesh without indices (Soup)
        const geometry = new THREE.BufferGeometry();
        geometry.setAttribute('position', new THREE.BufferAttribute(data, 3));
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

        this.emitContext();
    }

    public resize(width: number, height: number) {
        this.camera.aspect = width / height;
        this.camera.updateProjectionMatrix();
        this.renderer.setSize(width, height, false);
        this.emitContext();
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
