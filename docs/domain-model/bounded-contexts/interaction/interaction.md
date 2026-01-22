# Bounded Context: Interaction

## Description
The **Interaction Context** is the user's entry point. It runs on the **Main Thread** and is responsible for the User Interface, handling DOM events, and orchestrating the application lifecycle. It translates raw user actions into Domain Events.

## Ubiquitous Language

*   **View**: The visible UI controls overlaying the scene.
*   **Formula Input**: The text field where the user enters the algebraic expression.
*   **Scale Control**: The mechanism (buttons) to increase or decrease the coordinate range.
*   **Drag**: The action of pressing and moving the mouse to rotate the scene.
*   **Intent**: The user's desire to change the system state (captured as events).

## Responsibilities
1.  **Render UI**: Display HTML controls (Input, Buttons) via React.
2.  **Capture Input**: Listen for DOM events (`mousedown`, `mousemove`, `mouseup`, `input`).
3.  **Translate**: Convert DOM events into semantic Domain Events (e.g., `mousemove` -> `ViewRotated`).
4.  **Orchestrate**: Manage the lifecycle of Web Workers (Math and Scene contexts).

## Inbound Events
*   *None (Driven by User)*

## Outbound Events
*   `FormulaChanged(expression: string)`: Triggered when user updates the input.
*   `ScaleChanged(factor: positive | negative)`: Triggered by +/- buttons.
*   `ViewRotated(deltaX: number, deltaY: number)`: Triggered by mouse drag.
