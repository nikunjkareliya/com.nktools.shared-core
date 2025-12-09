# Rule Conflicts Analysis

This document tracks conflicts between the established coding rules and the current project's source files.

## **Conflicts Between Rules and Project Source Files**

### **1. GameEvent Naming Convention Violation**
**Rule**: GameEvent declarations should have NO "On" prefix (e.g., `GameStart`, not `OnGameStart`)
**Conflict**: All events in `SharedEvents.cs` use "On" prefix:
- `OnGameStateChanged`
- `OnGameStateEntered` 
- `OnGameStateExited`
- `OnPanelRegistered`
- `OnPanelShow`

**Files Affected**: `Runtime/Scripts/Shared/GlobalEventSystem/SharedEvents.cs`

### **2. Event Handler Naming Convention Violation**
**Rule**: GameEvent handlers should use `Handle` + exact event name (e.g., `HandleGameStateChanged`)
**Conflict**: In `GameStateController.cs` and `PanelNavigatorController.cs`, handlers use different naming:
- `ChangeState()` instead of `HandleGameStateChanged()`
- `HandleRegisterPanel()` instead of `HandlePanelRegistered()`
- `HandleShowPanel()` instead of `HandlePanelShow()`

**Files Affected**: 
- `Runtime/Scripts/Shared/GameState/GameStateController.cs`
- `Runtime/Scripts/Shared/Panels/PanelNavigatorController.cs`

### **3. Singleton Pattern Usage (Against Rules)**
**Rule**: "Avoid using singleton pattern, as it causes tight coupling"
**Conflict**: `GenericSingleton.cs` implements a singleton pattern, which the rules explicitly discourage

**Files Affected**: `Runtime/Scripts/Shared/GenericSingleton.cs`

### **4. Missing C# Event Communication Pattern**
**Rule**: Views should communicate with Controllers via C# events, not direct method calls
**Conflict**: `BasePanel` and panel implementations don't follow the View-Controller C# event pattern. They directly call `SharedEvents.OnPanelRegistered.Execute(this)` instead of raising C# events that Controllers subscribe to.

**Files Affected**: 
- `Runtime/Scripts/Shared/Panels/BasePanel.cs`
- `Runtime/Scripts/GamePanels/HomePanel.cs`
- `Runtime/Scripts/GamePanels/GameplayPanel.cs`
- `Runtime/Scripts/GamePanels/LevelSelectPanel.cs`
- `Runtime/Scripts/GamePanels/LevelCompletedPanel.cs`
- `Runtime/Scripts/GamePanels/LevelFailedPanel.cs`
- `Runtime/Scripts/GamePanels/CharacterSelectPanel.cs`

### **5. Direct GameEvent Calls from Views**
**Rule**: Views should never call GameEvents directly
**Conflict**: All panel classes (`HomePanel`, `GameplayPanel`, etc.) directly call `SharedEvents.OnPanelRegistered.Execute(this)` in their `Awake()` methods, violating the rule that only Controllers should trigger GameEvents.

**Files Affected**: All panel classes in `Runtime/Scripts/GamePanels/`

### **6. Missing View-Controller Separation**
**Rule**: Controllers should hold serialized references to Views and subscribe to their C# events
**Conflict**: The current panel system doesn't separate View and Controller - panels are both Views and handle their own registration logic.

**Files Affected**: All panel classes and the panel system architecture

### **7. Inconsistent Naming Convention for UI Callbacks**
**Rule**: UI callbacks should follow `[Action][Element][Clicked]` pattern (no "On" prefix)
**Conflict**: The codebase uses Unity's standard `OnComplete()` callbacks from DOTween, which is acceptable, but the rule suggests avoiding "On" prefix for custom UI callbacks.

**Files Affected**: 
- `Runtime/Scripts/Shared/Base/BaseView.cs`
- `Runtime/Scripts/Shared/Panels/BasePanel.cs`

### **8. Missing Model Registration in BootLoader**
**Rule**: "When new model class created, that must register to ModelStore registry when game starts. That is handled by BootLoader.cs component"
**Conflict**: No `BootLoader.cs` file exists in the project, and models are registered manually in controllers.

**Files Affected**: Missing `BootLoader.cs` file

### **9. Inconsistent Field Naming**
**Rule**: Private fields should use `_camelCase` with underscore prefix
**Conflict**: Most fields follow this correctly, but some inconsistencies exist in naming conventions across files.

**Files Affected**: Various files across the project

### **10. Missing Event Registration Lifecycle Management**
**Rule**: Event registration should happen in `Awake()` and cleanup in `OnDestroy()`
**Conflict**: While the base classes handle this correctly, the specific event registration patterns don't follow the complete naming conventions specified in the rules.

**Files Affected**: All controller classes

## **Priority Levels**

### **High Priority (Architecture Breaking)**
1. GameEvent naming convention violations
2. Direct GameEvent calls from Views
3. Missing View-Controller separation
4. Singleton pattern usage

### **Medium Priority (Naming Convention Issues)**
5. Event handler naming convention violations
6. Missing C# event communication pattern
7. Missing Model registration in BootLoader

### **Low Priority (Style Issues)**
8. Inconsistent field naming
9. Inconsistent UI callback naming
10. Event registration lifecycle management

## **Recommended Action Plan**

1. **Phase 1**: Fix GameEvent naming (remove "On" prefix)
2. **Phase 2**: Implement proper View-Controller separation for panels
3. **Phase 3**: Create BootLoader for model registration
4. **Phase 4**: Update event handler naming conventions
5. **Phase 5**: Remove singleton pattern usage
6. **Phase 6**: Standardize field naming conventions

## **Notes**

- The `GameEvent.cs` implementation itself is well-designed and follows good practices
- The base classes (`BaseController`, `BaseView`, `BaseViewController`) provide good foundations
- The `ModelStore` implementation is correct
- Most conflicts are related to naming conventions and architectural patterns rather than fundamental design issues
