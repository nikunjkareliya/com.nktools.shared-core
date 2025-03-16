using System;

namespace Shared.Core
{
    public class GameEvent
    {
        private event Action _action = delegate { };

        public void Execute()
        {
            _action.Invoke();
        }

        public void Register(Action listener)
        {
            _action -= listener;
            _action += listener;
        }

        public void Unregister(Action listener)
        {
            _action -= listener;
        }
    }

    public class GameEvent<T>
    {
        private event Action<T> _action = delegate { };

        public void Execute(T param)
        {
            _action.Invoke(param);
        }

        public void Register(Action<T> listener)
        {
            _action -= listener;
            _action += listener;
        }

        public void Unregister(Action<T> listener)
        {
            _action -= listener;
        }
    }

    public class GameEvent<T1, T2>
    {
        private event Action<T1, T2> _action = delegate { };

        public void Execute(T1 param1, T2 param2)
        {
            _action.Invoke(param1, param2);
        }

        public void Register(Action<T1, T2> listener)
        {
            _action -= listener;
            _action += listener;
        }

        public void Unregister(Action<T1, T2> listener)
        {
            _action -= listener;
        }
    }

    public class GameEvent<T1, T2, T3>
    {
        private event Action<T1, T2, T3> _action = delegate { };

        public void Execute(T1 param1, T2 param2, T3 param3)
        {
            _action.Invoke(param1, param2, param3);
        }

        public void Register(Action<T1, T2, T3> listener)
        {
            _action -= listener;
            _action += listener;
        }

        public void Unregister(Action<T1, T2, T3> listener)
        {
            _action -= listener;
        }
    }
}