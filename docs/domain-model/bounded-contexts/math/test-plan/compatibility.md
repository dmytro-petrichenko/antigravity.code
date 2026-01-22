# Compatibility & Environment

## 1. Browser Support
*   **Web Workers**: Verify functionality in **Chrome**.
*   **Transferable Objects**: Ensure `postMessage` with transferables works correctly.

## 2. Data Representation
*   **Float32 Precision**: Verify that precision lost during `Float32` conversion does not visibly affect the grid quality.
*   **Endianness**: (Edge case) Ensure binary data matches system endianness if typed arrays are viewed incorrectly (standard TypedArrays handle this, but good to verify).
