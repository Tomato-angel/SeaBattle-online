using System;
using System.Collections.Generic;

namespace Scripts.FSM
{
    public class FSM
    {
        public FSMState CurrentState { get; private set; }
        private Dictionary<Type, FSMState> _states = new Dictionary<Type, FSMState>();
        
        // Устаревшее
        /*
        public void AddState(FSMState state)
        {
            _states.Add(state.GetType(), state);
        }*/
        public void AddState(params FSMState[] states)
        {
            foreach (var state in states) _states.Add(state.GetType(), state); ;
        }

        public void SetState<T>() where T : FSMState
        {
            var type = typeof(T);
            if (CurrentState?.GetType() == type)
            {
                return;
            }
            if (_states.TryGetValue(type, out var newState))
            {
                CurrentState?.Exit();
                CurrentState = newState;
                CurrentState.Enter();
            }
        }

        public void Update()
        {
            CurrentState?.Update();
        }
    }
}
