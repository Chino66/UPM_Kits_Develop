using System.Collections.Generic;

namespace StateMachineKits
{
    public class State
    {
        public string Value;

        public Dictionary<string /*state group name*/, bool> _stateGroups;

        public State(string value)
        {
            Value = value;
            _stateGroups = new Dictionary<string, bool>();
        }

        public bool IsInGroup(string group)
        {
            return _stateGroups.ContainsKey(group);
        }

        public void AddToGroup(StateGroup group)
        {
            if (IsInGroup(group.Name))
            {
                return;
            }

            _stateGroups.Add(group.Name, false);
        }
    }
}