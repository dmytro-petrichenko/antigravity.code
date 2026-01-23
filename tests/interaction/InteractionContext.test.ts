/**
 * @vitest-environment jsdom
 */
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { InteractionContext } from '../../src/interaction/InteractionContext';
import { FormulaChanged, ScaleChanged, ViewRotated } from '../../src/interaction/domain/events';

describe('InteractionContext', () => {
    let context: InteractionContext;
    let container: HTMLElement;

    beforeEach(() => {
        document.body.innerHTML = '<div id="app"></div>';
        container = document.getElementById('app')!;
        context = new InteractionContext('app');
    });

    it('should initialize control panel', () => {
        const input = document.getElementById('formula-input') as HTMLInputElement;
        expect(input).not.toBeNull();
        expect(input.placeholder).toContain('Enter formula');
    });

    it('should emit FormulaChanged event on input', () => {
        const spy = vi.fn();
        context.subscribe(spy);

        const input = document.getElementById('formula-input') as HTMLInputElement;
        input.value = 'z = x + y';
        input.dispatchEvent(new Event('input'));

        expect(spy).toHaveBeenCalledWith(expect.any(FormulaChanged));
        expect(spy).toHaveBeenCalledWith(expect.objectContaining({ expression: 'z = x + y' }));
    });

    it('should emit ScaleChanged event on button click', () => {
        const spy = vi.fn();
        context.subscribe(spy);

        const btnUp = document.getElementById('scale-up') as HTMLButtonElement;
        btnUp.click();

        expect(spy).toHaveBeenCalledWith(expect.any(ScaleChanged));
        expect(spy).toHaveBeenCalledWith(expect.objectContaining({ factor: 1.1 }));
    });

    it('should emit ViewRotated on drag', () => {
        const spy = vi.fn();
        context.subscribe(spy);

        container.dispatchEvent(new MouseEvent('mousedown', { clientX: 0, clientY: 0 }));

        // Move
        window.dispatchEvent(new MouseEvent('mousemove', { clientX: 10, clientY: 5 }));

        // Up
        window.dispatchEvent(new MouseEvent('mouseup'));

        expect(spy).toHaveBeenCalledWith(expect.any(ViewRotated));
        expect(spy).toHaveBeenCalledWith(expect.objectContaining({ deltaX: 10, deltaY: 5 }));
    });
});
