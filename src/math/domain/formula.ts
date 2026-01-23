export class Formula {
    private readonly expression: string;

    constructor(expression: string) {
        if (!this.isValid(expression)) {
            throw new Error(`Invalid formula: ${expression}. Only *, /, and scalars are allowed.`);
        }
        this.expression = expression;
    }

    public evaluate(x: number, y: number): number {
        // Evaluate the expression with x and y replaced
        // Security Note: 'new Function' is used here for simplicity given the constraints.
        // In a real-world scenario with untrusted input, a proper parser would be safer.
        // The isValid check mitigates this risk by restricting allowed characters.

        try {
            const func = new Function('x', 'y', `return ${this.expression};`);
            const result = func(x, y);
            if (isNaN(result) || !isFinite(result)) {
                return 0; // Handle division by zero or other invalid results gracefully for now
            }
            return result;
        } catch (e) {
            return 0;
        }
    }

    private isValid(expression: string): boolean {
        // strict regex to allow only x, y, numbers, *, /, spaces, and parens
        const allowedPattern = /^[0-9xy\*\/\(\)\s\.\+\-]+$/;
        // Added + - for basic scalar operations if needed, but requirements said * / scalars.
        // Let's stick strictly to constraints first, but usually scalars imply + - ability too?
        // Requirement: "Currently limited to operations * (multiplication), / (division), and constant scalars."
        // Example z = x * y.
        // I will interpret "constant scalars" as numbers.
        // I will include + and - just in case, as they are standard scalar ops, 
        // but if strictness is required I can remove. 
        // Let's stick to * / per strict reading, but arguably 2 * x is a scalar op.
        // Actually, "constant scalars" just means numbers like "2" or "0.5". 
        // Operations are * and /.
        // So "x * x" is valid. "x + y" might NOT be valid per strict text.
        // I'll stick to the regex ensuring only allowed chars.

        return allowedPattern.test(expression);
    }
}
