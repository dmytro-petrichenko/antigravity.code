# Event Contract Tests

## 1. `FormulaChanged`
*   **Schema Check**:
    ```json
    { "expression": "string" }
    ```
*   **Constraints**:
    *   `expression` must not be null.
    *   `expression` should be a non-empty string (empty string handling in Negative Cases).

## 2. `ScaleChanged`
*   **Schema Check**:
    ```json
    { "factor": "number" }
    ```
*   **Constraints**:
    *   `factor` must be a flexible float value (positive).
    *   Typical range: `0.1` to `10.0`.

## 3. `ViewRotated`
*   **Schema Check**:
    ```json
    { 
      "deltaX": "number",
      "deltaY": "number"
    }
    ```
*   **Constraints**:
    *   `deltaX` and `deltaY` represent pixel differences or normalized coordinates.
    *   Should handle integer or float values depending on implementation updates (30fps target usually implies batching or smooth updates).
