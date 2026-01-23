# Bounded Context: Math

## Description
The **Math Context** is the computational core of the application. It is responsible for interpreting algebraic relationships and generating raw spatial data. It has NO knowledge of how this data is visualized or how the user inputs it.

## Ubiquitous Language

*   **Formula**: An algebraic expression defining the relationship between $x$ and $y$. Currently limited to operations `*` (multiplication), `/` (division), and constant scalars.
    *   *Example*: `z = x * y`, `z = x * x`
*   **Coordinate Space**: The abstract mathematical bounds of the calculation.
    *   **Range**: The min/max values for axes. ZOOM IN reduces the **Domain Range** (smaller slice of math space) but SCALES UP the **Points** to fill the same visual bounds. Initial: -10 to +10.
    *   **Range**: The min/max values for axes. ZOOM IN reduces the **Domain Range** (smaller slice of math space) but SCALES UP the **Points** to fill the same visual bounds. Initial: -10 to +10.
    *   **Resolution**: The fixed number of segments/dots along an axis (e.g. 20).
    *   **Step**: Dynamic. Calculated as `Range / Resolution`. Ensures typically constant point density visually.
*   **Grid**: A collection of calculated **Points** representing the surface $z = f(x, y)$.
    *   **Note**: Coordinates are **Normalized View Space**. They are pre-scaled by the Zoom Factor to ensure the visual grid size remains constant while the domain coverage changes.
*   **Point**: A tuple $(x, y, z)$ in 3D space.

## Responsibilities
1.  **Parse** raw string computations from the Interaction Context.
2.  **Evaluate** the value in the Grid based on the current Formula.
3.  **Regenerate** the Grid when the Scale (Range) changes.

## Inbound Events
*   `FormulaChanged(expression: string)`
*   `ScaleChanged(factor: number)`

## Outbound Events
*   `GridUpdated(points: Float32Array)`: Emitted when calculation is complete. Returns a flat buffer of coordinates for performance.
