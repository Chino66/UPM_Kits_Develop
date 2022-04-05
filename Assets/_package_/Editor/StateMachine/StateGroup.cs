using System.Collections.Generic;

namespace StateMachineKits
{
    public class StateGroup
    {
        public string Name;
        public Dictionary<string /*state name*/, bool> States => _states;
        private Dictionary<string /*state name*/, bool> _states;

        public StateGroup(string name)
        {
            Name = name;
            _states = new Dictionary<string, bool>();
        }

        public void AddState(string state)
        {
            if (_states.ContainsKey(state))
            {
                return;
            }

            _states.Add(state, false);
        }

        // public string Name;
        // private Dictionary<string /*state name*/, State> _states;
        //
        // public StateGroup(string name)
        // {
        //     Name = name;
        //     _states = new Dictionary<string, State>();
        // }
        //
        // public void AddState(string state)
        // {
        //     if (_states.ContainsKey(state))
        //     {
        //         return;
        //     }
        //
        //     var s = new State(state);
        //     _states.Add(state, s);
        //     s.AddToGroup(this);
        // }
    }
}