# Bounded Context: Scene

## Description
The **Scene Context** is responsible for the visual representation of the data. It manages the 3D environment, geometry generation, and rendering loop using **Three.js**. It runs inside a `WebWorker` via `OffscreenCanvas` to prevent UI blocking.

## Ubiquitous Language

*   **Scene Graph**: The hierarchy of visual objects (Meshes, Lights, Cameras).
*   **Mesh**: The 3D geometric object representing the data surface. Constructed from the *Grid*.
    *   **Vertices**: The points in 3D space (mapped from Math Context).
    *   **Faces**: The triangles connecting vertices.
*   **Camera**: The user's viewport into the scene.
    *   **Orbit**: The action of rotating the camera around the center (0,0,0).
*   **Axes**: Visual guides for X, Y, and Z directions.

## Responsibilities
1.  **Render** the 3D scene at 30fps.
2.  **Construct Geometry**: Convert raw `Grid` data (Points) into a renderable `Mesh`.
3.  **Update Camera**: Apply rotation deltas received from the Interaction Context.

## Inbound Events
*   `GridUpdated(points: Float32Array)`: Source data for the Mesh.
*   `ViewRotated(deltaX: number, deltaY: number)`: Request to orbit the camera.

## Outbound Events
*   *None (Visual Output only)*
