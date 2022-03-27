using System.IO;
using UIElementsKits.UIFramework;
using UnityEditor;
using UnityEngine.UIElements;

namespace UPMKits
{
    /// <summary>
    /// UKDTUI=UPM Kits Develop Tool
    /// </summary>
    public class UKDTUI : UI
    {
        public static UKDTUI CreateUI(VisualElement parent = null)
        {
            var ui = new UKDTUI();
            ui.Initialize(parent);
            return ui;
        }

        public UKDTContext Context;

        private UKDTUI()
        {
            Context = new UKDTContext();
        }

        protected override void OnInitialize(VisualElement parent)
        {
            var uxmlPath = Path.Combine(PackagePath.MainPath, @"Resources/UIElement/package_json_editor_uxml.uxml");
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            var ussPath = Path.Combine(PackagePath.MainPath, @"Resources/UIElement/uss.uss");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
            var temp = asset.CloneTree();
            temp.style.flexGrow = 1;
            temp.styleSheets.Add(styleSheet);
            AddStyleSheet(styleSheet);
            Add(temp);

            AddView<DeveloperView>();
            AddView<PackageEditorView>();
            AddView<EditorOperateView>();
            AddView<PackageOperateView>();
        }

        public void Refresh()
        {
            GetView<DeveloperView>().Refresh();
            GetView<PackageEditorView>().Refresh();
            GetView<EditorOperateView>().Refresh();
            GetView<PackageOperateView>().Refresh();
        }
    }
}