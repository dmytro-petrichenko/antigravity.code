# API Use Cases (Functional Tests)

## 1. Formula Input
*   **Scenario**: User updates the function to plot.
*   **Action**: Type `z = sin(x)` into the text input.
*   **Verify**:
    *   `FormulaChanged` event is dispatched.
    *   Payload contains standard `expression: "z = sin(x)"`.
    *   UI State updates to reflect the new value.

## 2. Scale Control
*   **Scenario**: User zooms in/out using buttons.
*   **Action**: Click Scale (+) button.
*   **Verify**:
    *   `ScaleChanged` event is dispatched with `factor > 1.0` (e.g., 1.1 or 2.0).
*   **Action**: Click Scale (-) button.
*   **Verify**:
    *   `ScaleChanged` event is dispatched with `factor < 1.0` (e.g., 0.9 or 0.5).

## 3. View Rotation (Orbit)
*   **Scenario**: User rotates the 3D scene.
*   **Sequence**:
    1.  `mousedown` on canvas.
    2.  `mousemove` by `(+100, -50)` pixels.
    3.  `mouseup` to release.
*   **Verify**:
    *   `ViewRotated` events are dispatched continuously during drag.
    *   `deltaX` corresponds to horizontal movement.
    *   `deltaY` corresponds to vertical movement.
    *   No events dispatched after `mouseup`.

## 4. Reset View
*   **Scenario**: User resets the scene to default.
*   **Action**: Click "Reset" button (if implemented).
*   **Verify**:
    *   `FormulaChanged` resets to default expression.
    *   Camera position resets (may involve Scene context event interaction).
