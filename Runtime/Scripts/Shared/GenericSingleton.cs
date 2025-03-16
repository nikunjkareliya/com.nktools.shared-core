using UnityEngine;

namespace Shared.Core
{    
    public class GenericSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _applicationIsQuitting = false;

        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    Debug.LogWarning($"[Singleton] Instance of {typeof(T)} is being accessed after application quit. Returning null.");
                    return null;
                }

                if (_instance == null)
                {
                    // Find existing instance in the scene
                    _instance = FindObjectOfType<T>();

                    // If no instance exists, create a new one
                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(T).Name);
                        _instance = singletonObject.AddComponent<T>();
                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return _instance;                
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"[Singleton] Duplicate instance of {typeof(T)} detected. Destroying the new one.");
                Destroy(gameObject);
            }
        }

        private void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }
    }

}