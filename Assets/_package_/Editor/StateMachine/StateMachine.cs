using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachineKits
{
    public class StateMachine
    {
        public string LastState;

        public string CurrentState;

        private List<StateHandler> _stateHandlers;

        public StateMachine()
        {
            _stateHandlers = new List<StateHandler>();
        }

        public void AddHandler(StateHandler handler)
        {
            _stateHandlers.Add(handler);
        }

        public void ExecuteState(string state, object args = null)
        {
            foreach (var handler in _stateHandlers)
            {
                handler.ChangeState(state, args);
            }
        }

        public void ChangeState(string state, object args = null)
        {
            Debug.Log(state);
            LastState = CurrentState;
            CurrentState = state;
            ExecuteState(CurrentState, args);
        }
    }
}