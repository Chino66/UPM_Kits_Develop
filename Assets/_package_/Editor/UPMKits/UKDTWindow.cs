using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UPMKits
{
    public class UKDTWindow : EditorWindow
    {
        [MenuItem("Tools/UPM Kits/Develop Tool")]
        public static void ShowWindow()
        {
            var window = GetWindow<UKDTWindow>();
            window.minSize = new Vector2(800, 520);
            window.titleContent = new GUIContent("UPM Kits Develop Tool");
            window.Show();
        }

        private void OnEnable()
        {
            var root = UKDTUI.CreateUI();
            rootVisualElement.Add(root.Self);
        }

        public static void OpenUEC()
        {
            Type type = null;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                type = assembly.GetType("UEC.UECWindow");
                if (type != null)
                {
                    break;
                }
            }

            if (type == null)
            {
                return;
            }

            var window = EditorWindow.GetWindow(type);
            window.minSize = new Vector2(400, 500);
            window.titleContent = new GUIContent("UEC");
            window.Show();
        }
    }
}