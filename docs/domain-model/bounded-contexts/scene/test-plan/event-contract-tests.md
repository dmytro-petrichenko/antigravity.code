# Event Contract Tests

## 1. Input Validation (`GridUpdated`)
*   **Valid Payload**:
    *   `points`: Valid `Float32Array` with length divisible by 3.
*   **Data Integrity**:
    *   Ensure the buffer is successfully *received* (transferred or copied).
    *   Constructed Geometry `position` attribute length matches input length.

## 2. Input Validation (`ViewRotated`)
*   **Valid Payload**:
    ```json
    { "deltaX": 0.05, "deltaY": -0.02 }
    ```
*   **Result**: Camera updates internal spherical coordinates.
*   **Edge Case**: 
    *   `NaN` or `Infinity` inputs: Should be ignored to prevent camera "flying away".

## 3. Internal State
*   Ensure that the `Scene` object maintains synchronization with the received contracts.
