using System.IO;
using PackageKits;
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

            AddView<CheckView>();
            AddView<DeveloperView>();
            AddView<PackageEditorView>();
            AddView<EditorOperateView>();
            AddView<PackageOperateView>();

            Start();
        }

        private async void Start()
        {
            var hasConfig = Context.UECConfigModel.HasConfig();
            if (hasConfig == false)
            {
                var hasInstall = await PackageUtils.HasPackageAsync("com.chino.github.unity.uec");
                if (hasInstall)
                {
                    Context.StateMachine.ChangeState(UKDTState.ConfigUECTip, ".uecconfig not config, please");
                    return;
                }
                else
                {
                    Context.StateMachine.ChangeState(UKDTState.InstallUECTip, ".uecconfig");
                    return;
                }
            }


            var hasNpmrc = Context.NpmrcModel.HasNpmrc();
            if (hasNpmrc == false)
            {
                var hasInstall = await PackageUtils.HasPackageAsync("com.chino.github.unity.uec");
                if (hasInstall)
                {
                    Context.StateMachine.ChangeState(UKDTState.ConfigUECTip, ".npmrc not config, please");
                    return;
                }
                else
                {
                    Context.StateMachine.ChangeState(UKDTState.InstallUECTip, ".npmrc");
                    return;
                }
            }

            var hasDeveloper = Context.NpmrcModel.GetDeveloper() != "* None *";
            if (hasDeveloper == false)
            {
                var hasInstall = await PackageUtils.HasPackageAsync("com.chino.github.unity.uec");
                if (hasInstall)
                {
                    Context.StateMachine.ChangeState(UKDTState.ConfigUECTip, "no developer, please");
                    return;
                }
                else
                {
                    Context.StateMachine.ChangeState(UKDTState.InstallUECTip, ".npmrc");
                    return;
                }
            }

            var hasPackageJson = Context.PackageJsonModel.HasPackageJson();
            if (hasPackageJson == false)
            {
                Context.StateMachine.ChangeState(UKDTState.NoPackageJson);
                return;
            }
            
            Context.StateMachine.ChangeState(UKDTState.EditorPackageJson);

            
            // _noDeveloperTip.SetDisplay(!hasDeveloper);

            // var has = hasConfig && context.PackageJsonModel.HasPackageJson() && hasDeveloper;
            // _selectBtn.SetEnabled(has);
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