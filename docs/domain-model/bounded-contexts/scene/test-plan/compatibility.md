# Compatibility & Environment

## 1. Browser Support
*   **OffscreenCanvas**:
    *   **Chrome/Edge**: Full support.
    *   **Firefox/Safari**: Verify partial support or fallback requirements (if targeting).
*   **WebGL 2.0**:
    *   Verify shader compilation on target devices.

## 2. Hardware Limitations
*   **Max Vertex Count**: Ensure grid size does not exceed `gl.MAX_ELEMENTS_VERTICES` (usually 65k for 16-bit indices, though we likely use unindexed or 32-bit if supported).
*   **Texture Limits**: If using textures for colormaps, respect `MAX_TEXTURE_SIZE`.

## 3. Device Performance
*   **Low-Power Mode**: Verify behavior on laptops on battery (requestAnimationFrame throttling).
