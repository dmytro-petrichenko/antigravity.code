export class FormulaChanged {
    constructor(public readonly expression: string) { }
}

export class ScaleChanged {
    constructor(public readonly factor: number) { }
}

export class ViewRotated {
    constructor(public readonly deltaX: number, public readonly deltaY: number) { }
}

export type InteractionEvent = FormulaChanged | ScaleChanged | ViewRotated;
