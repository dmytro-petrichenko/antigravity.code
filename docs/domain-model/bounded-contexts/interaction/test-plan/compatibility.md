# Compatibility & Environment

## 1. Browser Support
*   **Event Handling**: Ensure standard `addEventListener` usage for `mousedown`, `mousemove`, `mouseup`.
*   **Touch Devices**:
    *   **Requirement**: Mobile/Tablet support.
    *   **Tests**: Verify `touchstart`, `touchmove` map correctly to rotation events if touch is supported. Multitouch (pinch-to-zoom) might map to Scale controls.

## 2. Input Devices
*   **Trackpad**: Inertial scrolling or gestures should not cause erratic rotation or scaling.
*   **Keyboard Accessibility**: Ensure interactive elements (Inputs, Buttons) are focusable and usable via keyboard (`Tab`, `Enter`).

## 3. Screen Readers
*   **ARIA Labels**: Input fields and buttons must have descriptive `aria-label` or `title` attributes for accessibility.
