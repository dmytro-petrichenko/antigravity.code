import { FormulaChanged, ScaleChanged, ViewRotated, InteractionEvent } from '../domain/events';

export class ControlPanel {
    private container: HTMLElement;
    private input: HTMLInputElement;
    private scaleUpBtn: HTMLButtonElement;
    private scaleDownBtn: HTMLButtonElement;
    private dispatch: (event: InteractionEvent) => void;
    private isDragging: boolean = false;
    private lastMouseX: number = 0;
    private lastMouseY: number = 0;

    constructor(containerId: string, dispatch: (event: InteractionEvent) => void) {
        const el = document.getElementById(containerId);
        if (!el) throw new Error(`Container ${containerId} not found`);
        this.container = el;
        this.dispatch = dispatch;

        this.input = document.createElement('input');
        this.input.id = 'formula-input';
        this.input.type = 'text';
        this.input.placeholder = 'Enter formula (e.g. z = x * y)';

        this.scaleUpBtn = document.createElement('button');
        this.scaleUpBtn.id = 'scale-up';
        this.scaleUpBtn.textContent = '+';

        this.scaleDownBtn = document.createElement('button');
        this.scaleDownBtn.id = 'scale-down';
        this.scaleDownBtn.textContent = '-';

        this.render();
        this.attachListeners();
    }

    private render(): void {
        const controls = document.createElement('div');
        controls.id = 'controls';
        controls.style.position = 'absolute';
        controls.style.top = '10px';
        controls.style.left = '10px';
        controls.style.zIndex = '100';

        this.input.style.marginRight = '5px';

        controls.appendChild(this.input);
        controls.appendChild(this.scaleDownBtn);
        controls.appendChild(this.scaleUpBtn);
        this.container.appendChild(controls);
    }

    private attachListeners(): void {
        this.input.addEventListener('input', () => {
            this.dispatch(new FormulaChanged(this.input.value));
        });

        this.scaleUpBtn.addEventListener('click', () => {
            this.dispatch(new ScaleChanged(1.1));
        });

        this.scaleDownBtn.addEventListener('click', () => {
            this.dispatch(new ScaleChanged(0.9));
        });

        // Mouse drag on container for rotation. 
        // We listen on container for mousedown, but window for move/up to handle drag outside
        this.container.addEventListener('mousedown', this.onMouseDown);
        window.addEventListener('mouseup', this.onMouseUp);
        window.addEventListener('mousemove', this.onMouseMove);
    }

    private onMouseDown = (e: MouseEvent) => {
        // Prevent interfering with input
        if (e.target === this.input || e.target === this.scaleUpBtn || e.target === this.scaleDownBtn) return;

        this.isDragging = true;
        this.lastMouseX = e.clientX;
        this.lastMouseY = e.clientY;
    }

    private onMouseUp = () => {
        this.isDragging = false;
    }

    private onMouseMove = (e: MouseEvent) => {
        if (!this.isDragging) return;

        const deltaX = e.clientX - this.lastMouseX;
        const deltaY = e.clientY - this.lastMouseY;

        this.lastMouseX = e.clientX;
        this.lastMouseY = e.clientY;

        if (deltaX !== 0 || deltaY !== 0) {
            this.dispatch(new ViewRotated(deltaX, deltaY));
        }
    }

    public destroy(): void {
        this.container.removeEventListener('mousedown', this.onMouseDown);
        window.removeEventListener('mouseup', this.onMouseUp);
        window.removeEventListener('mousemove', this.onMouseMove);

        const controls = document.getElementById('controls');
        if (controls) controls.remove();
    }
}
