# API Use Cases (Functional Tests)

## 1. Scene Initialization
*   **Scenario**: Worker startup with `OffscreenCanvas`.
*   **Expected**: 
    *   3D Context (WebGL2) is successfully created.
    *   Basic scene graph (lights, camera, axes) is established.
    *   Render loop starts (waiting for data or rendering empty grid).

## 2. Geometry Updates
*   **Scenario**: Receiving new Grid data.
*   **Action**: `GridUpdated(points: [x1,y1,z1, ...])`
*   **Verify**: 
    *   Existing mesh is disposed (memory freed).
    *   New mesh is created with correct vertex count (points.length / 3).
    *   Scene renders the new topology.

## 3. Camera Interaction
*   **Scenario**: User rotates the view.
*   **Action**: `ViewRotated(deltaX: 0.1, deltaY: 0.0)`
*   **Verify**: 
    *   Camera position updates along the orbit path.
    *   LookAt target remains $(0,0,0)$.
    *   Rendered frame reflects the new perspective.

## 4. Rendering Cycle
*   **Scenario**: Continuous frame loop.
*   **Verify**: 
    *   Scene is re-rendered at approximately 30fps.
    *   Animation loop continues even if no new data arrives (for camera movement).
