# Math Context Test Plan

## Overview
This document outlines the testing strategy for the **Math Bounded Context**. 
As this context runs in a Web Worker and is purely computational, tests should focus on:
1.  **Correctness**: Accurate evaluation of mathematical formulas.
2.  **Performance**: Efficient generation of large coordinate grids.
3.  **Robustness**: Graceful handling of invalid inputs and edge cases.

## Test Scope
| Component | In Scope | Out of Scope |
| :--- | :--- | :--- |
| **Formula Parser** | Syntax validation, Operation precedence | Rendering, Colors |
| **Grid Generator** | Coordinate mapping, Float32Array layout | Interactions, Zooming |
| **Event Interface** | Schema compliance, Serialization | UI Integration |

## Test Levels
1.  **Unit Tests**: Logic verification (e.g., `calculateZ(x, y)`).
2.  **Contract Tests**: Verifying Event payloads match JSON Schemas.
3.  **Performance Tests**: Measuring calculation time for high-resolution grids.
