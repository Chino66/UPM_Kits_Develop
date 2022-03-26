using UnityEditor;
using UnityEngine;

namespace UPMKits
{
    public class PJEWindow : EditorWindow
    {
        [MenuItem("Tools/UPM Kits/Package Json Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<PJEWindow>();
            window.minSize = new Vector2(400, 520);
            window.titleContent = new GUIContent("PackageJsonEditor");
            window.Show();
        }

        private void OnEnable()
        {
            var root = PJEUI.CreateUI();
            rootVisualElement.Add(root.Self);
        }
    }
}