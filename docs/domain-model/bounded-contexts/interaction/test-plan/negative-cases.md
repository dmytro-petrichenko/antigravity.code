# Negative Cases & Edge Handling

## 1. Input Anomalies
*   **Empty Formula**: User deletes all text.
    *   **Handling**: Dispatch `FormulaChanged("")`. Math context is responsible for error handling/clearing, but Interaction Layer might implement a minimum length check or UI warning.
    *   **Decision**: Allow empty string event; prevent crash.
*   **Rapid Typing**:
    *   **Scenario**: User types fast.
    *   **Handling**: Implement **Debouncing** (e.g., 200ms) to prevent flooding the Event Bus with partial expressions (`z`, `z=`, `z=x`).

## 2. Interaction Glitches
*   **Mouse Exit**: 
    *   **Scenario**: User starts drag inside canvas, moves mouse *out of browser window* or into developer console, then releases button.
    *   **Expected**: Drag state must be cleared. `mouseup` or `mouseleave` (on window/document) should catch logic to stop rotation.
*   **Context Menu**:
    *   **Scenario**: Right-click during drag.
    *   **Expected**: Context menu might appear; drag operation should cancel or complete gracefully.

## 3. Invalid Scaling
*   **Negative/Zero Factor**:
    *   **Scenario**: Logic error allows scale calculation to reach <= 0.
    *   **Constraint**: UI should clamp scale factor to a safe minimum (e.g., `0.1`).

## 4. Race Conditions
*   **Worker Initialization**:
    *   **Scenario**: User inputs formula before Math Worker is ready.
    *   **Handling**: Events should be queued or the UI should be disabled (Loading state) until workers acknowledge readiness.
