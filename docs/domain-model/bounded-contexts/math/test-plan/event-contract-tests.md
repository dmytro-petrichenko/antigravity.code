# Event Contract Tests

## 1. Input Validation (`FormulaChanged`)
*   **Valid Payload**:
    ```json
    { "expression": "z = x + y" }
    ```
    *   **Result**: Worker accepts message, starts calculation.
*   **Missing Field**:
    ```json
    { "calc": "x + y" } // Missing "expression"
    ```
    *   **Result**: Worker ignores message or logs error (does not crash).

## 2. Output Validation (`GridUpdated`)
*   **Schema Check**:
    *   Ensure output matches `docs/domain-model/bounded-contexts/math/schemas/GridUpdated.json`.
    *   `points` MUST be a `Float32Array` (or Transferable).
    *   `timestamp` MUST be a valid Integer.
*   **Data Integrity**:
    *   `points.length` should equal `resolution * resolution * 3` (x, y, z tuples).
    *   Ensure no `NaN` values in the buffer.

## 3. Serialization
*   **Transferable Check**:
    *   Verify that the `Float32Array` buffer is *transferred* (length becomes 0 in sender context) to prevent memory cloning overhead.
