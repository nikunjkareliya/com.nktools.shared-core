using UnityEngine;

namespace Shared.Core
{
    [DefaultExecutionOrder(-10)]
    public abstract class BaseController : MonoBehaviour
    {        
        private void Awake()
        {
            Init();
            Register();
        }

        private void OnDestroy()
        {
            Deregister();
        }

        protected virtual void Init() { }
        protected virtual void Register() { }
        protected virtual void Deregister() { }
    }
}
