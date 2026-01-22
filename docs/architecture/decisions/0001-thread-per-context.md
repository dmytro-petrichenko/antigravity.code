# ADR-001: Thread-per-Context Architecture

## Status
Accepted

## Context
The application is a 3D visualization tool that requires:
1.  **Heavy Computation**: Generating dense coordinate grids from dynamic algebraic formulas.
2.  **Heavy Rendering**: Displaying complex 3D meshes at 30fps.
3.  **Responsive UI**: Immediate feedback to user input without freezing.

A conceptual decision was made to treat the system as a **Modular Monolith** with strict isolation between Bounded Contexts.

## Decision
We will execute each compute-heavy Bounded Context in its own **Web Worker**, communicating exclusively via asynchronous message passing (Events).

*   **Interaction Context**: Runs on the **Main Thread**.
*   **Math Context**: Runs in **Worker A**.
*   **Scene Context**: Runs in **Worker B** (rendering to `OffscreenCanvas`).

## Consequences

### Positive
*   **Performance Isolation**: Mathematical calculation of 1 million points will not frame-drop the UI or the 3D rotation.
*   **Strict Decoupling**: It is physically impossible for the Math domain to access the DOM or the Scene directly. This enforces the Domain-Driven Design boundaries.
*   **Scalability**: Logic can be easily moved to a backend service or WASM module in the future without changing the contract.

### Negative
*   **Complexity**: Asynchronous state management is harder than synchronous function calls.
*   **Data Serialization Overhead**: Passing large objects between threads can be slow.
    *   *Mitigation*: We MUST use **Transferable Objects** (`ArrayBuffer`) for the coordinate data to ensure zero-copy transfer.
*   **Input Latency**: Minimal delay added to interactions (e.g., rotation) due to message posting.
