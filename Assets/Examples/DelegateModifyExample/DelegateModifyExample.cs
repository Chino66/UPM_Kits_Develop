using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DME
{
    public class DelegateModifyExample : MonoBehaviour
    {
        private Action<int> BaseAction;
        private Action<int> SubAction1;
        private Action<int> SubAction2;

        void Start()
        {
            BaseAction += value => { Debug.Log($"BaseAction 1 value is {value}"); };

            BaseAction += value => { Debug.Log($"BaseAction 2 value is {value}"); };

            SubAction1 += value => { Debug.Log($"SubAction1 1 value is {value}"); };

            SubAction1 += value => { Debug.Log($"SubAction1 2 value is {value}"); };

            SubAction2 += value => { Debug.Log($"SubAction2 1 value is {value}"); };

            SubAction2 += value => { Debug.Log($"SubAction2 2 value is {value}"); };
        }

        private void OnGUI()
        {
            if (GUILayout.Button("invoke BaseAction"))
            {
                BaseAction.Invoke(777);
            }

            if (GUILayout.Button("add SubAction1 to BaseAction"))
            {
                BaseAction += SubAction1;
            }

            if (GUILayout.Button("add SubAction2 to BaseAction"))
            {
                BaseAction += SubAction2;
            }

            if (GUILayout.Button("remove SubAction1 from BaseAction"))
            {
                BaseAction -= SubAction1;
            }

            if (GUILayout.Button("remove SubAction2 from BaseAction"))
            {
                BaseAction -= SubAction2;
            }
        }
    }
}