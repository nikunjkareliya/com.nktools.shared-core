# Changelog

All notable changes to this project will be documented in this file.

## [1.4.0] - 2025-01-08

### Added
- **Overlay Panel Support** - New `IsOverlay` flag in `BasePanel` allows panels to display on top of existing screens without hiding them (e.g., Popups, Pause Screens, Level Failed).
- **Non-Destructive Navigation** - Updated `PanelNavigatorController` to respect the `IsOverlay` flag, preserving the background UI when opening an overlay panel.

## [1.3.1] - 2025-01-03

### Added
- **Scene Switcher** - Editor window for quick scene navigation from Build Settings
  - Accessible via `Tools > NK Tools > Scene Switcher`
  - Displays all scenes from Build Settings in scrollable list
  - One-click scene switching with automatic save prompts
  - Shows scene index and name for easy reference

## [1.3.0] - 2025-01-02

### Added
- **UI Setup Wizard** - Editor tool for generating MainUI prefabs with panel navigation system
- **Editor Assembly Definition** - Proper package structure with Shared.Core.Editor.asmdef
- **Aspect Ratio Presets** - Portrait (9:16, 9:20, 3:4), Landscape (16:9, 20:9, 4:3), and Custom resolution support
- **Dynamic Panel Management** - Add, update, and remove panels via visual wizard interface
- **Safe Update Mode** - Modify existing prefabs without data loss (adds new panels, preserves existing)
- **Sync Mode** - Keep prefabs in perfect sync with wizard configuration (removes panels not in wizard)
- **Preset System** - Save/load wizard configurations as JSON files for team sharing
- **Custom Panel Support** - Use project-specific panel scripts that inherit from BasePanel
- **3-Layer Canvas Architecture** - Background (-1), Panels (0), Foreground (1) for organized UI hierarchy
- **Validation System** - Prevents duplicate names, states, and invalid script references

### Changed
- **Runtime Assembly** - Removed "Editor" from includePlatforms in Shared.Core.asmdef (now Android/iOS only)
- **Package Structure** - Added Editor folder with proper assembly definition for editor-only tools

### Technical Details
- Created Shared.Core.Editor.asmdef referencing Shared.Core Runtime assembly
- UI Setup Wizard accessible via `Tools > NK Tools > UI Setup Wizard` menu
- Wizard validates panel configurations before generation
- Supports MenuItem integration for seamless Unity Editor workflow
- Comprehensive documentation at `Editor/Documentation/UISetupWizard_README.md`

## [1.2.0] - 2024-12-19

### Fixed
- **DOTween Dependency**: Removed external DOTween dependency and replaced with Unity coroutines for fade animations
- **Event Declaration Error**: Fixed CS0066 compilation error in GameEvent.cs by correcting generic delegate field declaration
- **Missing Meta File**: Added README.md.meta file to prevent Unity import warnings
- **GUID Conflict**: Resolved asset GUID conflict in bg0.jpg.meta with unique identifier

### Changed
- **BasePanel.cs**: Replaced DOTween DOFade() with coroutine-based fade animations
- **BaseView.cs**: Replaced DOTween DOFade() with coroutine-based fade animations
- **Package Dependencies**: Removed all external dependencies for zero-dependency package

### Technical Details
- Animations now use `Time.unscaledDeltaTime` for consistent behavior during pause
- Maintained all existing API compatibility
- Improved package reliability with no external dependencies
- Enhanced thread safety in event system

## [1.1.0] - Previous Release
- Initial release with DOTween dependency
- Basic MVC architecture implementation
- Global event system
- UI panel management system
