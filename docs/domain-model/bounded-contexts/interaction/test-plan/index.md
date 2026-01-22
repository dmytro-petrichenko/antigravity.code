# Interaction Context Test Plan

## Overview
This document outlines the testing strategy for the **Interaction Bounded Context**.
As this context runs on the **Main Thread** and handles the User Interface, tests should focus on:
1.  **Responsiveness**: Immediate feedback to user actions.
2.  **Accuracy**: Correct translation of DOM events into Domain Events.
3.  **Usability**: Smooth handling of inputs (drag, type, click).

## Test Scope
| Component | In Scope | Out of Scope |
| :--- | :--- | :--- |
| **Formula Input** | Text entry, Change detection, Event dispatch | Formula syntax validation (Math Context) |
| **Scale Controls** | Button clicks, Event dispatch | Grid regeneration logic (Math Context) |
| **Viewport Interaction** | Mouse drag detection, Delta calculation | 3D Rendering (Scene Context) |
| **Orchestration** | Worker instantiation, Message passing | Worker internal logic |

## Test Levels
1.  **Component Tests**: React component rendering and state logic (e.g., Testing Library).
2.  **Integration Tests**: Verifying event flow from DOM -> Event Bus.
3.  **End-to-End Tests**: Full user flows (e.g., Cypress/Playwright) verifying side effects in other contexts.
