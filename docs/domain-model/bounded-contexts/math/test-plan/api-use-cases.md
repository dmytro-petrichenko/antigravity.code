# API Use Cases (Functional Tests)

## 1. Zero State
*   **Scenario**: Initialize Worker without prior input.
*   **Expected**: Should produce a flat grid at $z=0$ OR wait for first formula event.

## 2. Basic Arithmetic
*   **Formula**: `z = x + y`
    *   **Input**: `FormulaChanged("z = x + y")`, Range: `[-10, 10]`
    *   **Verify**: For point $(0,0)$, $z=0$. For $(1,1)$, $z=2$.
*   **Formula**: `z = x * y`
    *   **Input**: `FormulaChanged("z = x * y")`
    *   **Verify**: For point $(2,3)$, $z=6$.

## 3. Constant Scalars
*   **Formula**: `z = 5`
    *   **Verify**: All points have $z=5$.
*   **Formula**: `z = x * 2`
    *   **Verify**: Linear slope in X, constant in Y.

## 4. Complex Expressions (Precedence)
*   **Formula**: `z = x * x + y * y` (Paraboloid)
    *   **Verify**: Center $(0,0)$ is minimum $z=0$. Values increase radially.

## 5. Scale Adjustment
*   **Scenario**: User changes scale factor.
*   **Action**: Send `ScaleChanged(factor: 2.0)`
*   **Verify**: 
    *   Grid logic recalculates based on new range.
    *   Message `GridUpdated` is emitted with new data.
