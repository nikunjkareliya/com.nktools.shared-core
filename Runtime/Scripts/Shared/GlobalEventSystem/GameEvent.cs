using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace Shared.Core
{
    /// <summary>
    /// Wrapper for weak reference to Unity Components to prevent memory leaks
    /// </summary>
    internal class WeakListenerWrapper
    {
        public System.WeakReference Target { get; }
        public System.Delegate Listener { get; }
        public string TargetInfo { get; }

        public WeakListenerWrapper(System.Delegate listener)
        {
            Listener = listener;
            
            // Check if this is a Unity Component method
            if (listener.Target is UnityEngine.Component component)
            {
                Target = new System.WeakReference(component);
                TargetInfo = $"{component.GetType().Name} on {component.name}";
            }
            else if (listener.Target is UnityEngine.GameObject gameObject)
            {
                Target = new System.WeakReference(gameObject);
                TargetInfo = $"GameObject: {gameObject.name}";
            }
            else
            {
                // For non-Unity objects, still track but no weak reference needed
                Target = listener.Target != null ? new System.WeakReference(listener.Target) : null;
                TargetInfo = listener.Target?.GetType().Name ?? "Static Method";
            }
        }

        public bool IsAlive => Target?.IsAlive != false;
        
        public bool IsUnityObjectDestroyed()
        {
            if (Target?.Target is UnityEngine.Object unityObj)
                return unityObj == null; // Unity's null check handles destroyed objects
            return false;
        }
    }

    public abstract class GameEventBase<TDelegate> where TDelegate : Delegate
    {
        private TDelegate _action;
        private readonly object _lock = new object();
        private readonly List<WeakListenerWrapper> _weakListeners = new List<WeakListenerWrapper>();
        
#if UNITY_EDITOR
        [System.NonSerialized] private int _debugExecutionCount;
        [System.NonSerialized] private int _debugExceptionCount;
#endif

        protected void ExecuteInternal(Action<TDelegate> invoker, string eventTypeName)
        {
            // Clean up destroyed Unity objects and create snapshot
            (TDelegate[] listenerSnapshot, WeakListenerWrapper[] wrapperSnapshot) = PrepareExecution();
            if (listenerSnapshot.Length == 0) return;
            
#if UNITY_EDITOR
            _debugExecutionCount++;
#endif
            
            // Execute listeners outside the lock to prevent deadlocks
            for (int i = 0; i < listenerSnapshot.Length; i++)
            {
                var listener = listenerSnapshot[i];
                var wrapper = wrapperSnapshot[i];
                
                try
                {
                    // Skip if Unity object was destroyed
                    if (wrapper != null && wrapper.IsUnityObjectDestroyed())
                        continue;
                        
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
#if UNITY_EDITOR
                    _debugExceptionCount++;
#endif
                    string listenerInfo = wrapper?.TargetInfo ?? "Unknown listener";
                    string contextInfo = GetListenerContext(listener);
                    
                    UnityEngine.Debug.LogError(
                        $"Exception in {eventTypeName} listener ({listenerInfo}): {ex.Message}\n" +
                        $"Listener: {contextInfo}\n" +
                        $"StackTrace: {ex.StackTrace}"
                    );
                }
            }
        }
        
        private (TDelegate[], WeakListenerWrapper[]) PrepareExecution()
        {
            lock (_lock)
            {
                CleanupDestroyedListeners();
                
                if (_action == null)
                    return (new TDelegate[0], new WeakListenerWrapper[0]);
                    
                var listeners = _action.GetInvocationList();
                if (listeners.Length == 0)
                    return (new TDelegate[0], new WeakListenerWrapper[0]);
                
                var listenerSnapshot = new TDelegate[listeners.Length];
                var wrapperSnapshot = new WeakListenerWrapper[listeners.Length];
                
                Array.Copy(listeners, listenerSnapshot, listeners.Length);
                
                // Match listeners with their wrappers for context info
                for (int i = 0; i < listeners.Length; i++)
                {
                    wrapperSnapshot[i] = _weakListeners.FirstOrDefault(w => 
                        ReferenceEquals(w.Listener, listeners[i]));
                }
                
                return (listenerSnapshot, wrapperSnapshot);
            }
        }
        
        private void CleanupDestroyedListeners()
        {
            // Remove destroyed Unity objects
            for (int i = _weakListeners.Count - 1; i >= 0; i--)
            {
                var wrapper = _weakListeners[i];
                if (!wrapper.IsAlive || wrapper.IsUnityObjectDestroyed())
                {
                    _weakListeners.RemoveAt(i);
                    _action = (TDelegate)Delegate.Remove(_action, wrapper.Listener);
                }
            }
        }
        
        private string GetListenerContext(TDelegate listener)
        {
            if (listener.Method != null)
            {
                var method = listener.Method;
                var target = listener.Target;
                
                if (target != null)
                {
                    return $"{target.GetType().Name}.{method.Name}()";
                }
                else
                {
                    return $"{method.DeclaringType?.Name}.{method.Name}() [Static]";
                }
            }
            return "Unknown Method";
        }

        public void Register(TDelegate listener)
        {
            if (listener == null) return;
            
            lock (_lock)
            {
                // Check if already registered
                if (IsRegisteredInternal(listener)) return;
                
                _action = (TDelegate)Delegate.Combine(_action, listener);
                _weakListeners.Add(new WeakListenerWrapper(listener));
            }
        }

        public void Unregister(TDelegate listener)
        {
            if (listener == null) return;
            
            lock (_lock)
            {
                _action = (TDelegate)Delegate.Remove(_action, listener);
                
                // Remove from weak listeners list
                for (int i = _weakListeners.Count - 1; i >= 0; i--)
                {
                    if (ReferenceEquals(_weakListeners[i].Listener, listener))
                    {
                        _weakListeners.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Checks if a specific listener is already registered
        /// </summary>
        public bool IsRegistered(TDelegate listener)
        {
            if (listener == null) return false;
            
            lock (_lock)
            {
                return IsRegisteredInternal(listener);
            }
        }
        
        private bool IsRegisteredInternal(TDelegate listener)
        {
            if (_action == null) return false;
            
            var listeners = _action.GetInvocationList();
            return listeners.Any(l => ReferenceEquals(l, listener));
        }

        public void Clear()
        {
            lock (_lock)
            {
                _action = null;
                _weakListeners.Clear();
            }
        }

        /// <summary>
        /// Gets the number of registered listeners in a thread-safe manner
        /// </summary>
        public int ListenerCount
        {
            get
            {
                lock (_lock)
                {
                    CleanupDestroyedListeners();
                    return _action?.GetInvocationList().Length ?? 0;
                }
            }
        }

        /// <summary>
        /// Checks if there are any registered listeners in a thread-safe manner
        /// </summary>
        public bool HasListeners
        {
            get
            {
                lock (_lock)
                {
                    CleanupDestroyedListeners();
                    return _action != null && _action.GetInvocationList().Length > 0;
                }
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Debug information for Unity Inspector
        /// </summary>
        public string GetDebugInfo()
        {
            lock (_lock)
            {
                CleanupDestroyedListeners();
                var listenerCount = _action?.GetInvocationList().Length ?? 0;
                
                return $"Listeners: {listenerCount} | Executions: {_debugExecutionCount} | Exceptions: {_debugExceptionCount}";
            }
        }
        
        /// <summary>
        /// Gets detailed listener information for debugging
        /// </summary>
        public string[] GetListenerInfo()
        {
            lock (_lock)
            {
                CleanupDestroyedListeners();
                
                if (_action == null) return new string[0];
                
                var listeners = _action.GetInvocationList();
                var info = new string[listeners.Length];
                
                for (int i = 0; i < listeners.Length; i++)
                {
                    var wrapper = _weakListeners.FirstOrDefault(w => ReferenceEquals(w.Listener, listeners[i]));
                    var context = GetListenerContext((TDelegate)listeners[i]);
                    var targetInfo = wrapper?.TargetInfo ?? "Unknown";
                    
                    info[i] = $"{i}: {context} ({targetInfo})";
                }
                
                return info;
            }
        }
#endif
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