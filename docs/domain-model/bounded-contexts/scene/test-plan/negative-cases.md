# Negative Cases & Edge Handling

## 1. Corrupt or Missing Data
*   **Empty Grid**: `GridUpdated` with empty array.
    *   **Handling**: Render nothing or a default plane. Do not crash WebGL.
*   **Malformed Grid**: Array length not divisible by 3.
    *   **Handling**: Truncate to nearest triangle or log warning and skip update.

## 2. WebGL Context Loss
*   **Scenario**: Browser tab backgrounded or GPU reset.
*   **Simulation**: `gl.getExtension('WEBGL_lose_context').loseContext()`.
*   **Expected**: 
    *   Worker pauses rendering.
    *   On `restoreContext`, scene resources (shaders, buffers) are re-initialized.

## 3. Performance Overload
*   **Scenario**: `GridUpdated` events arriving faster than frame time (e.g., > 30fps).
*   **Expected**: 
    *   Skip intermediate frames ("drop frames") to prioritize latest data.
    *   Do not queue up mesh generations (memory leak risk).

## 4. Invalid Geometry
*   **Scenario**: Point coordinates containing `NaN` or `Infinity` (passed from Math context).
*   **Handling**: Shader should discard these fragments or Vertex shader should clamp positions to avoid visual artifacts.
