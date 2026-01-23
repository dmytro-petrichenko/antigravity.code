import { Formula } from "./domain/formula";
import { Grid } from "./domain/grid";
import { GridConfig, GridUpdated } from "./types";

export class MathService {
    private currentConfig: GridConfig;
    private currentFormula: Formula;

    constructor() {
        // Defaults
        this.currentConfig = {
            range: { min: -10, max: 10 },
            step: 1
        };
        this.currentFormula = new Formula("x * y");
    }

    public updateFormula(expression: string): GridUpdated {
        this.currentFormula = new Formula(expression);
        return this.regenerate();
    }

    public updateScale(range: { min: number, max: number }, step: number): GridUpdated {
        this.currentConfig = { range, step };
        return this.regenerate();
    }

    private regenerate(): GridUpdated {
        const points = Grid.generate(this.currentFormula, this.currentConfig);
        return { points };
    }
}
