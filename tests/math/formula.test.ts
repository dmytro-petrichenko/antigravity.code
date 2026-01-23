import { describe, it, expect } from "vitest";
import { Formula } from "../../src/math/domain/formula";

describe("Formula", () => {
    it("should accept valid expressions", () => {
        expect(() => new Formula("x * y")).not.toThrow();
        expect(() => new Formula("x * x")).not.toThrow();
        expect(() => new Formula("2 * x")).not.toThrow();
        expect(() => new Formula("x / 2")).not.toThrow();
        expect(() => new Formula("(x * y) / 2")).not.toThrow();
    });

    it("should reject invalid expressions", () => {
        expect(() => new Formula("x ^ y")).toThrow(); // ^ not allowed
        expect(() => new Formula("sin(x)")).toThrow(); // functions not allowed
        expect(() => new Formula("console.log(x)")).toThrow(); // malicious code
    });

    it("should evaluate correctly", () => {
        const f1 = new Formula("x * y");
        expect(f1.evaluate(2, 3)).toBe(6);
        expect(f1.evaluate(0, 5)).toBe(0);
        expect(f1.evaluate(-2, 3)).toBe(-6);

        const f2 = new Formula("x * x");
        expect(f2.evaluate(3, 4)).toBe(9); // y is ignored

        const f3 = new Formula("x / y");
        expect(f3.evaluate(6, 2)).toBe(3);
    });

    it("should handle division by zero gracefully", () => {
        const f = new Formula("x / y");
        expect(f.evaluate(1, 0)).toBe(0); // Defined behavior in our code
    });
});
