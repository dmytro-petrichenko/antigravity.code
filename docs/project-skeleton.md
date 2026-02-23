# Project Skeleton (Quick Context)

Small reference file to avoid re-scanning the whole repo for common UI/tasks.

## Repo Top Level

```text
antigravity.code/
├─ antigravity.code.sln
├─ nutrition-blazor-app/           # Blazor WebAssembly app (UI + mock data)
├─ nutrition-blazor-app.Tests/     # xUnit + bUnit tests
└─ docs/                           # Project notes / quick docs (this file)
```

## App Structure (`nutrition-blazor-app`)

```text
nutrition-blazor-app/
├─ Program.cs                      # DI setup + app boot
├─ App.razor                       # Root app component / router host
├─ _Imports.razor                  # Shared using/imports for Razor
├─ Layout/
│  ├─ MainLayout.razor             # Global page shell
│  └─ MainLayout.razor.css         # Layout-level styles
├─ Pages/
│  ├─ Home.razor                   # Main dashboard page composition
│  └─ Home.razor.css               # Page layout/grid styles
├─ Components/
│  ├─ NutritionFactsTestWidget.*   # Top summary card (nutrition facts)
│  ├─ MacroBarWidget.*             # Horizontal macro bars + legends
│  ├─ MacroBarWidgetLogic.cs       # Validation + CSS class mapping for macro bars
│  ├─ MacronutrientPieWidget.*     # Donut chart + legend
│  └─ MicronutrientPanelWidget.*   # Vitamins/minerals grids with mini bars
├─ Models/
│  └─ NutritionDashboardData.cs    # DTOs/records for dashboard data
├─ Services/
│  └─ NutritionDataService.cs      # `INutritionDataService` + mock dashboard data
└─ wwwroot/
   ├─ index.html                   # HTML shell, font preload, CSS includes
   ├─ fonts/                       # Self-hosted web fonts (currently Roboto)
   └─ css/
      ├─ fonts.css                 # `@font-face` definitions
      ├─ tokens.css                # Theme, spacing, radius, typography tokens
      ├─ components.css            # Shared DS utilities + text role classes
      └─ app.css                   # Global reset/base shell styles
```

## Main Page Tree (`Pages/Home.razor`)

```text
Home page
├─ NutritionFactsTestWidget
├─ Macronutrients card
│  └─ MacroBarWidget (xN, one per macro section)
├─ Macronutrient pie card
│  └─ MacronutrientPieWidget
└─ Micronutrient row
   ├─ MicronutrientPanelWidget (Vitamins)
   └─ MicronutrientPanelWidget (Minerals)
```

## Responsibilities (Short)

- `Program.cs`
  - Registers services and starts Blazor WebAssembly app.
  - Current data source is mock (`MockNutritionDataService`).

- `Services/NutritionDataService.cs`
  - Builds dashboard data for UI.
  - Contains macro sub-bar color ordering logic used by the demo/mock data.

- `Models/NutritionDashboardData.cs`
  - Defines records/enums for all dashboard data passed into components.

- `Pages/Home.razor`
  - Page composition only (layout + data wiring).
  - Loads `Dashboard` from `INutritionDataService`.

- `Components/*`
  - Render-focused UI modules.
  - Each component has local `.razor.css` for layout/details.
  - Shared colors/typography/radius should come from top-level CSS tokens/components.

- `wwwroot/css/tokens.css`
  - Single source of truth for theme palette, spacing, radius, and typography tokens.

- `wwwroot/css/components.css`
  - Shared utility classes (`ds-card`, `ds-meter-track`, text roles, etc.).

## Tests (`nutrition-blazor-app.Tests`)

```text
Tests cover:
├─ Component rendering (bUnit) for widgets
└─ MacroBarWidget logic + mock service/model expectations
```

Use tests when changing:
- widget markup / CSS class names (bUnit selectors can break)
- macro segment behavior / validation
- mock data structure

## Fast Navigation (Common Edit Targets)

- Theme/colors/typography/radius: `nutrition-blazor-app/wwwroot/css/tokens.css`
- Shared UI classes: `nutrition-blazor-app/wwwroot/css/components.css`
- Main page composition: `nutrition-blazor-app/Pages/Home.razor`
- Macro bar behavior/colors: `nutrition-blazor-app/Services/NutritionDataService.cs`
- Pie rendering/separators: `nutrition-blazor-app/Components/MacronutrientPieWidget.razor`
- Tests: `nutrition-blazor-app.Tests/`

## Color + Text System (Quick Note)

This page uses:

- `5` main theme colors (global palette)
- `5` text variations (global typography roles/tokens)

Where they are described:

- Colors (palette + semantic mappings + chart colors):
  - `nutrition-blazor-app/wwwroot/css/tokens.css`
  - Look for `Theme palette` and `Semantic color tokens`

- Text variations (sizes/weights/roles):
  - Tokens: `nutrition-blazor-app/wwwroot/css/tokens.css`
  - Shared classes: `nutrition-blazor-app/wwwroot/css/components.css`

Text variations used on the page:

1. Section Title (`.ds-text-section-title`)
2. Metric Highlight (`.ds-text-metric`)
3. Label / Descriptor (`.ds-text-label`)
4. Secondary / Supporting (`.ds-text-secondary`)
5. Base body/app text (global `body` font/color from `app.css` + `tokens.css`)
