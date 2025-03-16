using Shared.Core;
using UnityEngine;

namespace GameXYZ
{
    public class LevelSelectPanel : BasePanel
    {
        private void Awake()
        {
            SharedEvents.OnPanelRegistered.Execute(this);
        }

        public override void ShowPanel()
        {
            base.ShowPanel();
            Debug.Log($"{State} is now visible");
        }
    }
}