using System.IO;
using UIElementsKits.UIFramework;
using UnityEditor;
using UnityEngine.UIElements;

namespace UPMKits
{
    /// <summary>
    /// PJE=PackageJsonEditor
    /// </summary>
    public class PJEUI : UI
    {
        public static PJEUI CreateUI(VisualElement parent = null)
        {
            var ui = new PJEUI();
            if (parent == null)
            {
                parent = new VisualElement();
            }

            ui.Initialize(parent);
            return ui;
        }

        public PJEContext Context;

        private PJEUI()
        {
            Context = new PJEContext();
        }

        protected override void OnInitialize(VisualElement parent)
        {
            var uxmlPath = Path.Combine(PackagePath.MainPath, @"Resources/UIElement/package_json_editor_uxml.uxml");
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            var ussPath = Path.Combine(PackagePath.MainPath, @"Resources/UIElement/uss.uss");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
            var temp = asset.CloneTree();
            temp.styleSheets.Add(styleSheet);
            AddStyleSheet(styleSheet);
            Add(temp);

            AddView<DeveloperView>();
            // AddView<DetailView>();
            // AddView<OperateView>();
            // AddView<TipView>();
            // AddView<UpdateView>();
            // AddView<InstallView>();
        }

        public void Refresh()
        {
            // GetView<OverviewView>().Refresh();
            // GetView<DetailView>().Refresh();
            // GetView<OperateView>().Refresh();
            // GetView<TipView>().Refresh();
            // GetView<UpdateView>().Refresh();
            // GetView<InstallView>().Refresh();
        }
    }
}