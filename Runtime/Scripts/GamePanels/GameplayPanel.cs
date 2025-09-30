using UnityEngine;

namespace Shared.Core
{
    public class GameplayPanel : BasePanel
    {
        private void Awake()
        {
            SharedEvents.PanelRegistered.Execute(this);
        }

        public override void ShowPanel()
        {
            base.ShowPanel();
            Debug.Log($"{State} is now visible");
        }
    }
}