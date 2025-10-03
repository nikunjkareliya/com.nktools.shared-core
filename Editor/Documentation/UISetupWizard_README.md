# UI Setup Wizard - User Guide

## Overview

The **UI Setup Wizard** is a powerful Unity Editor tool that generates and manages MainUI prefabs with a standardized panel navigation system. It supports flexible aspect ratios, dynamic panel configurations, and safe update modes.

---

## Features

✅ **Aspect Ratio Support** - Portrait, Landscape, and Custom presets
✅ **Dynamic Panel Management** - Add, remove, and configure panels
✅ **Custom Panel Scripts** - Support for project-specific panel types
✅ **Safe Update Mode** - Add new panels without affecting existing ones
✅ **Sync Mode** - Keep prefab in perfect sync with wizard configuration
✅ **Preset System** - Save and load configurations as JSON
✅ **Validation** - Prevents duplicate names, states, and invalid scripts
✅ **3-Layer Architecture** - Background (-1), Panels (0), Foreground (1)

---

## Getting Started

### Opening the Tool

**Menu:** `Tools > NK Tools > UI Setup Wizard`

The wizard window will open with default panels pre-configured (Home, Gameplay, LevelCompleted, LevelFailed).

---

## Canvas Settings

### Aspect Ratio Selection

Choose your target platform orientation:

#### **Portrait Mode** (Default - Mobile Vertical)
- **Phone 9:16** - 1080×1920 (Modern smartphones)
- **Tall Phone 9:20** - 1080×2400 (Tall displays)
- **Tablet 3:4** - 768×1024 (iPad portrait)
- **Custom** - Manual width × height input

#### **Landscape Mode** (Mobile Horizontal / Tablet)
- **Standard 16:9** - 1920×1080 (Standard landscape)
- **Wide 20:9** - 2400×1080 (Ultrawide displays)
- **Tablet 4:3** - 1024×768 (iPad landscape)
- **Custom** - Manual width × height input

#### **Custom Mode**
- Enter any resolution: width × height
- Useful for non-standard aspect ratios

### Match Width/Height Slider

Controls how Canvas Scaler adapts to different screen sizes:
- **0.0** - Favor Height (best for portrait)
- **0.5** - Balanced (best for landscape/square)
- **1.0** - Favor Width (best for ultrawide)

**💡 Tip:** Click **"Auto"** button to calculate optimal value based on your selected aspect ratio.

### Render Mode

- **Screen Space - Camera** (Recommended) - Renders UI in camera space
- **Screen Space - Overlay** - Renders on top of everything
- **World Space** - Renders UI in 3D world

---

## Layer Settings (Advanced)

Expand this section to customize sorting orders for the 3-layer system:

- **Background Sort Order** (default: -1) - Renders behind panels
- **Panels Sort Order** (default: 0) - Main panel layer
- **Foreground Sort Order** (default: 1) - Renders in front of panels

**Use Cases:**
- Add UI decorations to Background layer
- Add popups/dialogs to Foreground layer
- Panels stay in the middle layer

---

## Panels Configuration

### Adding Panels

1. Click **"+ Add Panel"** button
2. Configure the new panel:
   - **Name** - Unique identifier (e.g., "ShopPanel")
   - **Type** - Built-in or Custom
   - **State** - GameState enum value (e.g., Shop)

### Built-in Panel Types

The wizard includes scripts from the NK Tools package:
- **HomePanel**
- **GameplayPanel**
- **LevelCompletedPanel**
- **LevelFailedPanel**
- **LevelSelectPanel**
- **CharacterSelectPanel**

### Custom Panel Scripts

1. Set Type to **"Custom"**
2. Enter script name (e.g., "ShopPanel")
3. The wizard will validate if the script exists and inherits `BasePanel`

**Requirements for Custom Scripts:**
```csharp
using Shared.Core;

public class ShopPanel : BasePanel
{
    private void Awake()
    {
        SharedEvents.PanelRegistered.Execute(this);
    }

    public override void ShowPanel()
    {
        base.ShowPanel();
        // Your custom logic here
    }
}
```

### Removing Panels

Click the **"×"** button on any panel to remove it from the configuration.

**⚠️ Note:** This only removes from wizard. To remove from existing prefab, enable **Sync Mode** (see below).

---

## Generation Options

### Mode Selection

#### **1. Generate New Prefab** (Default)
- Creates a brand new MainUI prefab
- Overwrites existing file if it exists
- **Use when:** Starting a new project or recreating the prefab from scratch

#### **2. Update Existing Prefab**
- Modifies an existing MainUI prefab
- Two sub-modes available:

##### **Safe Mode** (Default - Unchecked ☐)
- ✅ **Adds** new panels from wizard
- ❌ **Never removes** existing panels from prefab
- **Use when:** You want to add panels without touching existing ones

**Example:**
```
Prefab has:  [Home] [Gameplay] [LevelCompleted] [LevelFailed]
Wizard has:  [Home] [Gameplay] [Shop]

Result:      [Home] [Gameplay] [LevelCompleted] [LevelFailed] [Shop]
             ↑ Existing panels preserved + Shop added
```

##### **Sync Mode** (Checked ☑)
- ✅ **Adds** new panels from wizard
- ✅ **Removes** panels from prefab that are NOT in wizard
- **Use when:** You want prefab to match wizard exactly

**Example:**
```
Prefab has:  [Home] [Gameplay] [LevelCompleted] [LevelFailed]
Wizard has:  [Home] [Gameplay] [Shop]

Result:      [Home] [Gameplay] [Shop]
             ↑ Removed LevelCompleted & LevelFailed, added Shop
```

**⚠️ Warning:** The wizard displays a warning when Sync Mode is enabled to prevent accidental deletions.

#### **3. Template Only**
- Generates prefab structure WITHOUT panel scripts
- Creates empty panels for manual script attachment
- **Use when:** You want to customize panel scripts manually in Unity Editor

### Output Path

Specify where to save the generated prefab.

**Default:** `Assets/Prefabs/UI/MainUIPrefab.prefab`

Click **"📁"** to browse and select a different location.

---

## Validation

The wizard automatically validates your configuration before generation:

### ✅ Valid Checks
- ✅ No duplicate panel names
- ✅ No duplicate GameState assignments
- ✅ All custom scripts exist and inherit BasePanel
- ✅ Resolution within recommended range (320-4096)
- ✅ Output path exists (for Update mode)

### ⚠️ Warnings
If validation fails, warnings appear with details:
- Duplicate panel name: PanelName
- Duplicate GameState: StateName
- Custom script 'ScriptName' not found
- Resolution out of range
- Prefab not found at path (for Update mode)

**💡 Tip:** The "Generate/Update" button is disabled until all warnings are resolved.

---

## Preset System

### Saving Presets

1. Configure canvas settings and panels
2. Click **"Save Preset"**
3. Choose location and filename (saves as `.json`)

**Use Cases:**
- Share configurations across team members
- Maintain different setups for mobile/tablet/desktop
- Quickly switch between project templates

### Loading Presets

1. Click **"Load Preset"**
2. Select a previously saved `.json` file
3. All settings and panels are restored

**Example Preset Structure:**
```json
{
  "canvasSettings": {
    "aspectMode": 0,
    "portraitPreset": 0,
    "customResolution": { "x": 1080, "y": 1920 },
    "matchWidthOrHeight": 0.2,
    "renderMode": 1
  },
  "panels": [
    {
      "panelName": "HomePanel",
      "panelType": 0,
      "gameState": 2
    }
  ],
  "mode": 0,
  "outputPath": "Assets/Prefabs/UI/MainUIPrefab.prefab"
}
```

---

## Common Workflows

### Workflow 1: New Portrait Mobile Game

1. Open wizard: `Tools > NK Tools > UI Setup Wizard`
2. Aspect: **Portrait** (default)
3. Preset: **1080×1920 (9:16)** (default)
4. Panels: Default 4 panels already loaded
5. Click **"Generate Prefab"**
6. ✅ Done! Prefab created at `Assets/Prefabs/UI/MainUIPrefab.prefab`

---

### Workflow 2: Landscape Tablet Game

1. Open wizard
2. Aspect: **Landscape**
3. Preset: **1024×768 (4:3)** for iPad
4. Match: Click **"Auto"** → sets to 0.5
5. Panels: Customize as needed
6. Click **"Generate Prefab"**
7. ✅ Done!

---

### Workflow 3: Add Shop Panel to Existing Prefab

1. Open wizard
2. Click **"+ Add Panel"**
3. Configure:
   - Name: "ShopPanel"
   - Type: Custom
   - Script: "ShopPanel"
   - State: Shop
4. Mode: **Update Existing Prefab**
5. Keep **Sync Mode UNCHECKED** (Safe Mode)
6. Click **"Update Prefab"**
7. ✅ ShopPanel added without affecting existing panels

**Console Output:**
```
➕ Added ShopPanel
✅ Updated prefab at Assets/Prefabs/UI/MainUIPrefab.prefab
```

---

### Workflow 4: Remove Panel from Existing Prefab

1. Open wizard
2. Find panel to remove (e.g., "LevelFailedPanel")
3. Click **"×"** to remove from wizard
4. Mode: **Update Existing Prefab**
5. **CHECK** ☑ **"Sync Mode (Remove Missing Panels)"**
6. Click **"Update Prefab"**
7. ✅ LevelFailedPanel removed from prefab

**Console Output:**
```
➖ Removed LevelFailedPanel
✅ Updated prefab at Assets/Prefabs/UI/MainUIPrefab.prefab
```

**⚠️ Important:** Without Sync Mode enabled, the panel will NOT be removed from the prefab.

---

### Workflow 5: Create Custom Aspect Ratio

1. Open wizard
2. Aspect: **Custom**
3. Resolution: **2560×1440** (ultrawide)
4. Click **"Auto"** to calculate match value
5. Configure panels
6. Click **"Save Preset"** (for future reuse)
7. Click **"Generate Prefab"**
8. ✅ Custom resolution prefab created

---

## Generated Prefab Structure

The wizard creates a prefab with this hierarchy:

```
MainUIPrefab (Canvas, CanvasScaler, GraphicRaycaster)
├── BackgroundPanels (Canvas, SortOrder: -1)
│   └── [Your background UI elements]
│
├── PanelsNavigator (PanelNavigatorController)
│   └── Container
│       ├── HomePanel (Canvas, CanvasGroup, GraphicRaycaster, HomePanel script)
│       ├── GameplayPanel (Canvas, CanvasGroup, GraphicRaycaster, GameplayPanel script)
│       ├── LevelCompletedPanel (Canvas, CanvasGroup, GraphicRaycaster, LevelCompletedPanel script)
│       ├── LevelFailedPanel (Canvas, CanvasGroup, GraphicRaycaster, LevelFailedPanel script)
│       └── [Your custom panels]
│
└── ForegroundPanels (Canvas, SortOrder: 1)
    └── [Your foreground UI elements like popups]
```

### Key Components

#### MainUIPrefab Root
- **Canvas** - Configured with your selected render mode
- **CanvasScaler** - Uses your resolution and match settings
- **GraphicRaycaster** - Handles UI input

#### Each Panel
- **Canvas** - Override sorting enabled (SortOrder: 0)
- **CanvasGroup** - For fade in/out transitions
- **GraphicRaycaster** - Panel-specific raycasting
- **Panel Script** - HomePanel, GameplayPanel, or your custom script
  - `State` property set to corresponding GameState
  - `_canvasGroup` reference wired automatically

#### PanelNavigatorController
- Listens to `SharedEvents.PanelShow`
- Hides all panels except the one matching the requested GameState
- Automatically registered by each panel's `Awake()` method

---

## Integration with Game Code

### Switching Between Panels

Use the global event system:

```csharp
using Shared.Core;

// Switch to Home panel
SharedEvents.GameStateChanged.Execute(GameState.Home);

// Switch to Gameplay panel
SharedEvents.GameStateChanged.Execute(GameState.Gameplay);

// Switch to LevelCompleted panel
SharedEvents.GameStateChanged.Execute(GameState.LevelCompleted);
```

### Panel Lifecycle

Each panel automatically:
1. Registers itself with PanelNavigatorController in `Awake()`
2. Shows/hides with fade transitions via BasePanel
3. Responds to `SharedEvents.PanelShow` events

### Custom Panel Implementation

```csharp
using Shared.Core;
using UnityEngine;

public class ShopPanel : BasePanel
{
    // Serialize your UI references
    [Header("UI References")]
    [SerializeField] private Button _buyButton;
    [SerializeField] private TextMeshProUGUI _coinsText;

    private void Awake()
    {
        // Required: Register with navigator
        SharedEvents.PanelRegistered.Execute(this);
    }

    private void Start()
    {
        // Wire up UI events
        _buyButton.onClick.AddListener(OnBuyButtonClicked);
    }

    public override void ShowPanel()
    {
        base.ShowPanel(); // Handles fade in

        // Your custom show logic
        UpdateCoinsDisplay();
    }

    protected override void OnShowCompleted()
    {
        // Called after fade in animation completes
        Debug.Log("Shop panel fully visible");
    }

    protected override void OnHideCompleted()
    {
        // Called after fade out animation completes
        Debug.Log("Shop panel fully hidden");
    }

    private void OnBuyButtonClicked()
    {
        // Handle purchase
    }

    private void UpdateCoinsDisplay()
    {
        // Update UI
    }
}
```

---

## Troubleshooting

### Issue: Custom Script Not Found

**Problem:** Warning shows "Custom script 'ScriptName' not found"

**Solution:**
1. Ensure the script exists in your project
2. Check the script name matches exactly (case-sensitive)
3. Verify the script inherits from `BasePanel`
4. Make sure the script is in one of these namespaces:
   - Global (no namespace)
   - `Shared.Core`
   - `MergeBlocks`
   - Or the wizard will search all assemblies

---

### Issue: Prefab Not Updating

**Problem:** Clicked "Update Prefab" but changes not applied

**Solution:**
- If **adding panels**: Should work in Safe Mode
- If **removing panels**: Must enable ☑ **Sync Mode**
- Check console for output messages (➕ Added / ➖ Removed)
- Verify output path points to correct prefab
- Check validation warnings

---

### Issue: Panels Not Switching

**Problem:** Panels don't show/hide when using `SharedEvents.GameStateChanged`

**Solution:**
1. Ensure GameStateController exists in scene
2. Verify PanelNavigatorController is in prefab hierarchy
3. Check that each panel has correct `State` value assigned
4. Confirm panels call `SharedEvents.PanelRegistered.Execute(this)` in `Awake()`

---

### Issue: Wrong Aspect Ratio on Device

**Problem:** UI looks stretched or squashed on target device

**Solution:**
1. Check Canvas Scaler reference resolution matches your target
2. Adjust Match Width/Height slider:
   - Portrait games: 0.0 - 0.3 (favor height)
   - Landscape games: 0.5 - 0.7 (favor width)
   - Square: 0.5 (balanced)
3. Click **"Auto"** to calculate recommended value
4. Test on multiple device aspect ratios

---

### Issue: Duplicate Panel Names

**Problem:** Validation warning about duplicate panel names

**Solution:**
- Each panel must have a unique name
- Check for typos (e.g., "HomePanel" vs "Homepanel")
- Remove or rename duplicate panels

---

### Issue: Missing CanvasGroup Reference

**Problem:** BasePanel.ShowPanel() errors about missing CanvasGroup

**Solution:**
- The wizard automatically wires CanvasGroup references
- If manually editing prefab, ensure each panel GameObject has:
  1. CanvasGroup component attached
  2. Panel script's `_canvasGroup` field assigned in Inspector

---

## Best Practices

### ✅ DO

- **Use Presets** for different platform configurations
- **Test aspect ratios** on target devices
- **Name panels descriptively** (e.g., "MainMenuPanel" not "Panel1")
- **Use Safe Mode** when adding panels to avoid accidents
- **Save presets** before major changes
- **Use built-in panels** from NK Tools when possible
- **Check validation** before generating

### ❌ DON'T

- **Don't use Sync Mode** unless you're certain about removing panels
- **Don't skip validation warnings** - they prevent errors
- **Don't manually edit** PanelNavigator structure (use wizard instead)
- **Don't forget** to call `SharedEvents.PanelRegistered.Execute(this)` in custom panels
- **Don't use duplicate** panel names or GameState values
- **Don't delete** BackgroundPanels or ForegroundPanels layers (they're for organization)

---

## Tips & Tricks

### Quick Setup for New Project

1. Load a saved preset matching your target platform
2. Add project-specific custom panels
3. Generate prefab
4. Done in 30 seconds!

### Organizing Large Panel Lists

- Use descriptive names: "Gameplay_MainHUD" instead of "Panel1"
- Group related panels by prefix: "Settings_", "Shop_", "Social_"
- Remove unused default panels immediately

### Multi-Platform Support

Save different presets:
- `Mobile_Portrait.json` - 1080×1920
- `Tablet_Portrait.json` - 768×1024
- `Tablet_Landscape.json` - 1024×768

Load appropriate preset based on target platform.

### Testing Panel Transitions

In GameStateController, use keyboard shortcuts to test:
```csharp
// Already in package:
if (Input.GetKeyDown(KeyCode.Alpha1))
    SharedEvents.GameStateChanged.Execute(GameState.Home);

if (Input.GetKeyDown(KeyCode.Alpha2))
    SharedEvents.GameStateChanged.Execute(GameState.Gameplay);
```

---

## Version History

### v1.1 (Current)
- ✅ Added Sync Mode for panel removal
- ✅ Improved console logging (➕/➖ symbols)
- ✅ Enhanced validation messages

### v1.0
- ✅ Initial release
- ✅ Aspect ratio support (Portrait/Landscape/Custom)
- ✅ Dynamic panel management
- ✅ Custom panel script support
- ✅ Preset save/load system
- ✅ Validation system
- ✅ Safe update mode

---

## Support

### Package Requirements
- **NK Tools Shared Core** package (com.nktools.shared-core v1.2.1+)
- Unity 2022.3+ recommended

### File Location
- Tool: `Assets/Editor/UISetupWizard.cs`
- Output: `Assets/Prefabs/UI/MainUIPrefab.prefab` (default)

### References
- NK Tools Package: https://github.com/nikunjkareliya/com.nktools.shared-core
- BasePanel documentation: See package source
- GameState enum: `Shared.Core.GameState`
- Event system: `Shared.Core.SharedEvents`

---

## Quick Reference Card

| Action | Steps |
|--------|-------|
| **New Prefab** | Mode: Generate New → Generate Prefab |
| **Add Panel** | Mode: Update Existing → Safe Mode → Update Prefab |
| **Remove Panel** | Mode: Update Existing → ☑ Sync Mode → Update Prefab |
| **Change Aspect** | Select aspect → Choose preset → Auto match → Generate |
| **Save Config** | Save Preset → Choose location |
| **Load Config** | Load Preset → Select .json file |
| **Custom Script** | Type: Custom → Enter script name → Validate |

---

## Keyboard Shortcuts

| Key | Action |
|-----|--------|
| `Tools > NK Tools > UI Setup Wizard` | Open wizard |
| (No shortcuts in wizard itself) | |

**💡 Tip:** Assign custom shortcut via Unity Preferences > Shortcuts > Tools > NK Tools

---

**End of Guide** - For questions or issues, check the Troubleshooting section or review package documentation.
