export type Operation = '*' | '/';

export interface ExpressionNode {
    type: 'operation' | 'variable' | 'constant';
    value?: string | number;
    left?: ExpressionNode;
    right?: ExpressionNode;
}

export class ExpressionParser {
    parse(expression: string): ExpressionNode {
        // Remove "z =" or "z=" prefix if present
        const rhs = expression.replace(/^z\s*=\s*/, '').trim();

        // Simple tokenizer: split by spaces, but also handle no spaces around operators
        // yielding token stream
        const tokens = this.tokenize(rhs);
        return this.parseExpression(tokens);
    }

    private tokenize(expr: string): string[] {
        const tokens: string[] = [];
        let current = '';

        for (let i = 0; i < expr.length; i++) {
            const char = expr[i];
            if (char === ' ') {
                if (current) {
                    tokens.push(current);
                    current = '';
                }
            } else if (['*', '/'].includes(char)) {
                if (current) {
                    tokens.push(current);
                    current = '';
                }
                tokens.push(char);
            } else {
                current += char;
            }
        }
        if (current) {
            tokens.push(current);
        }
        return tokens;
    }

    private parseExpression(tokens: string[]): ExpressionNode {
        // Handle simple left-associative binary operations
        // A op B op C -> ((A op B) op C)

        if (tokens.length === 0) {
            throw new Error("Empty expression");
        }

        let left = this.parseTerm(tokens[0]);
        let i = 1;

        while (i < tokens.length) {
            const op = tokens[i];
            if (op !== '*' && op !== '/') {
                throw new Error(`Unexpected token: ${op}`);
            }
            if (i + 1 >= tokens.length) {
                throw new Error("Missing right operand");
            }
            const right = this.parseTerm(tokens[i + 1]);

            left = {
                type: 'operation',
                value: op,
                left,
                right
            };
            i += 2;
        }

        return left;
    }

    private parseTerm(token: string): ExpressionNode {
        if (token === 'x' || token === 'y') {
            return { type: 'variable', value: token };
        }
        const num = parseFloat(token);
        if (!isNaN(num)) {
            return { type: 'constant', value: num };
        }
        throw new Error(`Invalid token: ${token}`);
    }

    evaluate(node: ExpressionNode, x: number, y: number): number {
        if (node.type === 'constant') {
            return node.value as number;
        }
        if (node.type === 'variable') {
            return node.value === 'x' ? x : y;
        }
        if (node.type === 'operation') {
            const l = this.evaluate(node.left!, x, y);
            const r = this.evaluate(node.right!, x, y);
            if (node.value === '*') return l * r;
            if (node.value === '/') return l / r;
        }
        return 0;
    }
}
