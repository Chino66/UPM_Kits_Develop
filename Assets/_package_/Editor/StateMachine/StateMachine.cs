using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace StateMachineKits
{
    public class StateMachine
    {
        public string LastState;

        public string CurrentState;

        private List<StateHandler> _handlers;

        private List<StateCondition> _conditions;

        public StateMachine()
        {
            _handlers = new List<StateHandler>();
            _conditions = new List<StateCondition>();
        }

        public void AddHandler(StateHandler handler)
        {
            _handlers.Add(handler);
        }

        public void ExecuteState(string state, object args = null)
        {
            foreach (var handler in _handlers)
            {
                handler.ChangeState(state, args);
            }
        }

        public void ChangeState(string state, object args = null)
        {
            // Debug.Log(state);
            LastState = CurrentState;
            CurrentState = state;
            ExecuteState(CurrentState, args);
        }

        public async Task JudgeState()
        {
            foreach (var condition in _conditions)
            {
                var ret = await condition.Judge();
                if (ret)
                {
                    return;
                }
            }
        }

        public void AddCondition(StateCondition condition)
        {
            _conditions.Add(condition);
        }
    }
}