using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachineKits
{
    public class StateHandler
    {
        public const string OtherState = "OtherState";
        public string LastState;
        public string CurrentState;


        private Dictionary<string /*state name*/, List<string /*state group name*/>> _stateWhereInGroups;
        private Dictionary<string /*state group name*/, StateGroup> _stateGroups;
        private Dictionary<string /*state group name*/, List<Action<object>>> _stateGroupActions;

        public StateHandler()
        {
            _stateGroupActions = new Dictionary<string, List<Action<object>>>();
            _stateWhereInGroups = new Dictionary<string, List<string>>();
            _stateGroups = new Dictionary<string, StateGroup>();
        }

        public void AddStateAction(string state, Action<object> action)
        {
            if (_stateWhereInGroups.TryGetValue(state, out var groups) == false)
            {
                groups = new List<string>();
                /*如果state没有group,则默认自身就是一个group*/
                groups.Add(state);
                _stateWhereInGroups.Add(state, groups);
            }

            foreach (var groupName in groups)
            {
                if (_stateGroups.TryGetValue(groupName, out var stateGroup) == false)
                {
                    stateGroup = new StateGroup(groupName);
                    stateGroup.AddState(state);
                    _stateGroups.Add(groupName, stateGroup);
                }

                if (_stateGroupActions.TryGetValue(groupName, out var actions) == false)
                {
                    actions = new List<Action<object>>();
                    _stateGroupActions.Add(groupName, actions);
                }

                actions.Add(action);
            }
        }

        public void AddStateGroupAction(StateGroup group, Action<object> action)
        {
            var groupName = group.Name;
            if (_stateGroups.TryGetValue(groupName, out var stateGroup))
            {
                Debug.LogError($"StateGroup : {groupName} already exist");
                return;
            }

            _stateGroups.Add(groupName, group);

            if (_stateGroupActions.TryGetValue(groupName, out var actions) == false)
            {
                actions = new List<Action<object>>();
                _stateGroupActions.Add(groupName, actions);
            }

            actions.Add(action);

            foreach (var pair in group.States)
            {
                var stateName = pair.Key;
                if (_stateWhereInGroups.TryGetValue(stateName, out var list) == false)
                {
                    list = new List<string>();
                    _stateWhereInGroups.Add(stateName, list);
                }

                if (list.Contains(groupName) == false)
                {
                    list.Add(groupName);
                }
            }
        }

        public void ExecuteState(string state, object args = null)
        {
            if (_stateWhereInGroups.TryGetValue(state, out var groups) == false)
            {
                if (state == OtherState)
                {
                    return;
                }

                // Debug.LogError($"no state {state}");
                ExecuteOtherState(args);
                return;
            }

            foreach (var groupName in groups)
            {
                if (_stateGroupActions.TryGetValue(groupName, out var actions) == false)
                {
                    continue;
                }

                Debug.Log($"{groupName}, {state}");
                foreach (var action in actions)
                {
                    action?.Invoke(args);
                }
            }
        }

        public void AddOtherStateAction(Action<object> action)
        {
            AddStateAction(OtherState, action);
        }

        public void ExecuteOtherState(object args)
        {
            ExecuteState(OtherState, args);
        }

        public void ChangeState(string state, object args = null)
        {
            LastState = CurrentState;
            CurrentState = state;
            ExecuteState(CurrentState, args);
        }
    }
}