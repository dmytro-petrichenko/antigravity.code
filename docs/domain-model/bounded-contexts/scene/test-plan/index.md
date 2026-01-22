# Scene Context Test Plan

## Overview
This document outlines the testing strategy for the **Scene Bounded Context**. 
As this context governs the 3D visualization and runs within a Web Worker (using `OffscreenCanvas`), tests should focus on:
1.  **Visual Correctness**: Proper mesh generation from grid data and camera behavior.
2.  **Performance**: Maintaining a steady 30fps target under load.
3.  **Stability**: Handling WebGL context lifecycle and efficient memory management (geometry disposal).

## Test Scope
| Component | In Scope | Out of Scope |
| :--- | :--- | :--- |
| **Mesh Builder** | Vertex construction, Normal calculation | Formula calculation (Math Context) |
| **Renderer** | WebGL context setup, Scene graph management | HTML/DOM UI Overlay |
| **Camera** | Orbit logic, Projection matrix updates | Touch gesture interpretation (Interaction Context) |

## Test Levels
1.  **Unit Tests**: Logic for camera math and scene graph hierarchy.
2.  **Visual Regression**: Comparing rendered frames against baseline images (snapshot testing).
3.  **Performance Tests**: Monitoring frame time (target < 33ms) and memory usage during mesh updates.
