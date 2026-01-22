# Negative Cases & Edge Handling

## 1. Invalid Formulas
*   **Syntax Error**: `z = x **` (Trailing operator)
*   **Unknown Symbol**: `z = x + foo` (Variable not defined)
*   **Empty String**: `""`
    *   **Expected Behavior**: Worker emits an Error Event (or logs error) and retains previous valid state. **Does not crash.**

## 2. Mathematical Anomalies
*   **Division by Zero**: `z = 1 / 0`
    *   **Result**: JavaScript returns `Infinity`.
    *   **Handling**: Math context should clamp values to `MAX_VALUE` or substitute `0` to prevent rendering artifacts, OR Scene context must handle Infinity. (Decision: Clamp in Math context).
*   **Domain Errors**: `z = sqrt(-1)` (if implemented) -> `NaN`.
    *   **Handling**: Replace `NaN` with `0`.

## 3. Resource Exhaustion
*   **Massive Scale**: Requesting an extremely large grid resolution.
    *   **Constraint**: Limit grid size (e.g., max 1000x1000) to prevent OOM in Worker.

## 4. Message Flooding
*   **Scenario**: Sending `FormulaChanged` events at 60Hz.
*   **Expected**: 
    *   Debounce inputs OR
    *   Cancel previous calculation and restart immediately.
