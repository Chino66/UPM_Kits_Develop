using UnityEditor;
using UnityEngine;

namespace UPMKits
{
    public class UKDTWindow : EditorWindow
    {
        [MenuItem("Tools/UPM Kits/Develop Tool")]
        private static void ShowWindow()
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
    }
}