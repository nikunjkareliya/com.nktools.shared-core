using System.Collections.Generic;
using UnityEngine;

namespace Shared.Core
{
    public class PanelNavigatorController : BaseController
    {
        // List of all registered screens        
        private Dictionary<GameState, BasePanel> _panels = new Dictionary<GameState, BasePanel>();

        // The currently active screen
        private BasePanel _currentPanel;

        protected override void Register()
        {
            SharedEvents.PanelRegistered.Register(HandlePanelRegistered);
            SharedEvents.PanelShow.Register(HandlePanelShow);
        }

        protected override void Deregister()
        {
            SharedEvents.PanelRegistered.Unregister(HandlePanelRegistered);
            SharedEvents.PanelShow.Unregister(HandlePanelShow);
        }

        // Register a screen with the manager
        // Alternatively take reference of each panel into array or list through serialized field
        private void HandlePanelRegistered(BasePanel panel)
        {
            if (!_panels.ContainsKey(panel.State))
            {
                _panels.Add(panel.State, panel);
                Debug.Log($"Panel - {panel.State} is registered");
            }
        }

        private void HandlePanelShow(GameState gameState)
        {
            if (_panels.ContainsKey(gameState))
            {
                BasePanel panel = _panels[gameState];
                //                 
                foreach (KeyValuePair<GameState, BasePanel> entry in _panels)
                {
                    BasePanel basePanel = entry.Value;
                    basePanel.HidePanel();
                }

                _currentPanel = panel;

                _currentPanel.ShowPanel(); // Show the new screen                
            }
            else
            {
                Debug.LogError($"Screen with ID {gameState} not found.");
            }
        }

    }
}