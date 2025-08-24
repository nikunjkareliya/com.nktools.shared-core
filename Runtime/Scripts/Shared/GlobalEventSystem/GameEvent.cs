using System;

namespace Shared.Core
{
    public abstract class GameEventBase<TDelegate> where TDelegate : class
    {
        private event TDelegate _action;
        private readonly object _lock = new object();

        protected void ExecuteInternal(Action<TDelegate> invoker, string eventTypeName)
        {
            Delegate[] listeners;
            lock (_lock)
            {
                if (_action == null) return;
                listeners = ((Delegate)(object)_action).GetInvocationList();
                if (listeners.Length == 0) return;
            }
            
            foreach (TDelegate listener in listeners)
            {
                try
                {
                    invoker(listener);
                }
                catch (OutOfMemoryException)
                {
                    throw;
                }
                catch (StackOverflowException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"Exception in {eventTypeName} listener: {ex.Message}\nStackTrace: {ex.StackTrace}");
                }
            }
        }

        public void Register(TDelegate listener)
        {
            if (listener == null) return;
            
            lock (_lock)
            {
                _action = (TDelegate)(object)Delegate.Combine((Delegate)(object)_action, (Delegate)(object)listener);
            }
        }

        public void Unregister(TDelegate listener)
        {
            if (listener == null) return;
            
            lock (_lock)
            {
                _action = (TDelegate)(object)Delegate.Remove((Delegate)(object)_action, (Delegate)(object)listener);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _action = null;
            }
        }
    }

    public class GameEvent : GameEventBase<Action>
    {
        public void Execute()
        {
            ExecuteInternal(action => action.Invoke(), "GameEvent");
        }
    }

    public class GameEvent<T> : GameEventBase<Action<T>>
    {
        public void Execute(T param)
        {
            ExecuteInternal(action => action.Invoke(param), $"GameEvent<{typeof(T).Name}>");
        }
    }

    public class GameEvent<T1, T2> : GameEventBase<Action<T1, T2>>
    {
        public void Execute(T1 param1, T2 param2)
        {
            ExecuteInternal(action => action.Invoke(param1, param2), $"GameEvent<{typeof(T1).Name}, {typeof(T2).Name}>");
        }
    }

    public class GameEvent<T1, T2, T3> : GameEventBase<Action<T1, T2, T3>>
    {
        public void Execute(T1 param1, T2 param2, T3 param3)
        {
            ExecuteInternal(action => action.Invoke(param1, param2, param3), $"GameEvent<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}>");
        }
    }
}