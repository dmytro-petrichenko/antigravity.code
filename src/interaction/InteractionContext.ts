import { InteractionEvent } from './domain/events';
import { ControlPanel } from './ui/ControlPanel';

type EventListener = (event: InteractionEvent) => void;

export class InteractionContext {
    private listeners: EventListener[] = [];
    private controlPanel: ControlPanel;

    constructor(containerId: string) {
        this.controlPanel = new ControlPanel(containerId, (event) => this.publish(event));
    }

    public subscribe(listener: EventListener): void {
        this.listeners.push(listener);
    }

    private publish(event: InteractionEvent): void {
        this.listeners.forEach(l => l(event));
    }

    public destroy(): void {
        this.controlPanel.destroy();
    }
}
