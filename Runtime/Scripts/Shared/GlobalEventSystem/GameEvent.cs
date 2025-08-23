using System;

namespace Shared.Core
{
    public class GameEvent
    {
        private event Action _action = delegate { };
        private readonly object _lock = new object();

        public void Execute()
        {
            lock (_lock)
            {
                var listeners = _action.GetInvocationList();
                foreach (Action listener in listeners)
                {
                    try
                    {
                        listener.Invoke();
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"Exception in GameEvent listener: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    }
                }
            }
        }

        public void Register(Action listener)
        {
            if (listener == null) return;
            
            lock (_lock)
            {
                _action -= listener;
                _action += listener;
            }
        }

        public void Unregister(Action listener)
        {
            if (listener == null) return;
            
            lock (_lock)
            {
                _action -= listener;
            }
        }
    }

    public class GameEvent<T>
    {
        private event Action<T> _action = delegate { };
        private readonly object _lock = new object();

        public void Execute(T param)
        {
            lock (_lock)
            {
                var listeners = _action.GetInvocationList();
                foreach (Action<T> listener in listeners)
                {
                    try
                    {
                        listener.Invoke(param);
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"Exception in GameEvent<{typeof(T).Name}> listener: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    }
                }
            }
        }

        public void Register(Action<T> listener)
        {
            if (listener == null) return;
            
            lock (_lock)
            {
                _action -= listener;
                _action += listener;
            }
        }

        public void Unregister(Action<T> listener)
        {
            if (listener == null) return;
            
            lock (_lock)
            {
                _action -= listener;
            }
        }
    }

    public class GameEvent<T1, T2>
    {
        private event Action<T1, T2> _action = delegate { };
        private readonly object _lock = new object();

        public void Execute(T1 param1, T2 param2)
        {
            lock (_lock)
            {
                var listeners = _action.GetInvocationList();
                foreach (Action<T1, T2> listener in listeners)
                {
                    try
                    {
                        listener.Invoke(param1, param2);
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"Exception in GameEvent<{typeof(T1).Name}, {typeof(T2).Name}> listener: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    }
                }
            }
        }

        public void Register(Action<T1, T2> listener)
        {
            if (listener == null) return;
            
            lock (_lock)
            {
                _action -= listener;
                _action += listener;
            }
        }

        public void Unregister(Action<T1, T2> listener)
        {
            if (listener == null) return;
            
            lock (_lock)
            {
                _action -= listener;
            }
        }
    }

    public class GameEvent<T1, T2, T3>
    {
        private event Action<T1, T2, T3> _action = delegate { };
        private readonly object _lock = new object();

        public void Execute(T1 param1, T2 param2, T3 param3)
        {
            lock (_lock)
            {
                var listeners = _action.GetInvocationList();
                foreach (Action<T1, T2, T3> listener in listeners)
                {
                    try
                    {
                        listener.Invoke(param1, param2, param3);
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"Exception in GameEvent<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}> listener: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    }
                }
            }
        }

        public void Register(Action<T1, T2, T3> listener)
        {
            if (listener == null) return;
            
            lock (_lock)
            {
                _action -= listener;
                _action += listener;
            }
        }

        public void Unregister(Action<T1, T2, T3> listener)
        {
            if (listener == null) return;
            
            lock (_lock)
            {
                _action -= listener;
            }
        }
    }
}