# Changelog

All notable changes to this project will be documented in this file.

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
