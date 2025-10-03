# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity package (com.nktools.shared-core v1.3.0) providing MVC architecture, state management, event system, and UI Setup Wizard for Unity 2022.3+ games. Zero external dependencies.

## Core Architecture

### MVC Pattern

**Models** - Pure C# classes registered in `ModelStore`, accessible only by Controllers
- Models store data and handle business logic
- Never communicate directly with Views
- Register models in `ModelStore` at initialization

**Views** - MonoBehaviour classes handling UI, animations, and visual updates
- Communicate with Controllers via C# events (e.g., `public event Action OnButtonClicked`)
- Never hold Controller references or call `SharedEvents` directly
- Expose public methods for Controllers to trigger visual changes

**Controllers** - MonoBehaviour classes managing game logic and inter-system communication
- Hold serialized references to Views: `[SerializeField] private PlayerView _playerView`
- Subscribe to View's C# events and `SharedEvents` (GameEvent system)
- Execute `SharedEvents` to notify other systems
- Inherit from `BaseController` with lifecycle: Init → Register → Deregister
- Execution order: -10

**Module Naming**:
- UI modules: `ModuleNameViewController.cs` + `ModuleNameView.cs`
- Non-UI modules: `ModuleNameController.cs` + `ModuleNameView.cs`

### Event System Rules

Two event types with distinct naming:

**1. GameEvents (Global)** - Inter-system communication via `SharedEvents`
- Declaration: NO "On" prefix, simple action names (`GameStateChanged`, `PlayerDied`)
- Handlers: `Handle` + event name (`HandleGameStateChanged`, `HandlePlayerDied`)
- Only Controllers execute GameEvents
- Example:
```csharp
// In Controller
SharedEvents.GameStateChanged.Execute(GameState.Gameplay);
SharedEvents.GameStateChanged.Register(HandleGameStateChanged);

private void HandleGameStateChanged(GameState newState) { }
```

**2. C# Events** - View-to-Controller communication
- Declaration: "On" prefix (`public event Action OnButtonClicked`)
- Handlers: "On" + ViewName + Action (`OnPlayerViewButtonClicked`)
- Only Views declare and raise these events
- Example:
```csharp
// In View
public event Action<int> OnAddScoreRequested;
private void AddButtonClicked() => OnAddScoreRequested?.Invoke(100);

// In Controller
_playerView.OnAddScoreRequested += OnPlayerViewAddScoreRequested;
```

**UI Callbacks**: No "On" prefix, use `[Action][Element]Clicked` pattern
- `StartButtonClicked()`, `AddScoreButtonClicked()`

### State Management

- `GameState` enum defines all states (Init, Loading, Home, Gameplay, LevelCompleted, etc.)
- `GameStateController` manages transitions via events
- `GameStateModel` stores current/previous state in `ModelStore`
- UI panels bind to states and auto show/hide via `PanelNavigatorController`

### ModelStore

Centralized data repository:
```csharp
ModelStore.Get<PlayerDataModel>()  // Auto-creates if not found
```

## Key Systems

- **Base Classes**: `BaseController`, `BaseView`, `BaseViewController` (lifecycle management)
- **Panel System**: `BasePanel` with coroutine-based fade animations
- **Singleton**: `GenericSingleton<T>` for thread-safe MonoBehaviour singletons
- **UI Setup Wizard**: Editor tool (`Tools > NK Tools > UI Setup Wizard`) generates MainUI prefabs with aspect ratio presets and panel management
- **Scene Switcher**: Editor tool (`Tools > NK Tools > Scene Switcher`) for quick scene navigation from Build Settings
- **Assemblies**: `Shared.Core.asmdef` (Runtime: Android, iOS), `Shared.Core.Editor.asmdef` (Editor only)

## C# Style (Unity Conventions)

**Naming**:
- Public fields: `camelCase` (Unity convention: `public float moveSpeed`)
- Private fields: `_camelCase` (`private int _health`)
- Methods/Properties: `PascalCase` (`CalculateDamage()`, `Score { get; }`)
- GameEvent handlers: `HandleEventName()` (`HandleGameStateChanged`)
- C# event handlers: `OnViewNameAction()` (`OnPlayerViewDied`)
- UI callbacks: `ButtonNameClicked()` (`StartButtonClicked`)

**Declaration Order**:
1. Constants
2. Static fields
3. Events
4. Serialized fields (grouped by `[Header]`)
5. Public fields
6. Private fields
7. Properties
8. Unity lifecycle (Awake, Start, Update, OnDestroy)
9. Public methods
10. Private methods
11. Event handlers
12. Coroutines

**Formatting**:
- Braces on own line (Allman style)
- 4 spaces indentation
- Always use braces for if/loops
- US English spelling

## Important Rules

- Controllers never directly reference other Controllers (use GameEvents)
- Views never call GameEvents (use C# events to notify Controllers)
- Models only accessed via `ModelStore`, never passed between systems
- Avoid singleton pattern (causes tight coupling)
- Use factories for prefab instantiation: `BallFactory`, `PowerupFactory`
- Keep event handlers lightweight (delegate heavy operations to separate methods)
- Register/unregister events in Awake/OnDestroy (or OnEnable/OnDisable for pooled objects)

## Testing & Debugging

- Demo scene: `Runtime/Scenes/SharedCoreDemo.unity`
- `GameStateController` keyboard shortcuts: 1/2 (state changes), Space (log current state)
- Debug logs color-coded cyan for state changes
- Use `GetDebugInfo()` on GameEvents to inspect listeners
- UI Setup Wizard: Validation system prevents errors, preset system for configuration reuse
- Scene Switcher: Quick navigation tool for testing different scenes during development