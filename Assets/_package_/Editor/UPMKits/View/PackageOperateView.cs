using System.IO;
using GithubKits;
using UIElementsKits;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace UPMKits
{
    public class PackageOperateView : View<UKDTUI>
    {
        private VisualElementCache _cache;
        private UKDTContext context => UI.Context;

        private VisualElementPool _pool;
        private VisualElement _versionsRoot;
        private ScrollView _versionScrollView;
        private Button _publishBtn;
        private Label _publishLab;
        private Button _versionsBtn;


        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("package_operate_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            var uxmlPath = Path.Combine(PackagePath.MainPath, @"Resources/UIElement/version_item_uxml.uxml");
            var itemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            _pool = new VisualElementPool(itemAsset);
            _pool.SetGetAction(element => { element.SetDisplay(true); });

            _publishLab = _cache.Get<Label>("publish_lab");
            _publishBtn = _cache.Get<Button>("publish_btn");
            _publishBtn.clicked += PublishPackage;

            _versionsBtn = _cache.Get<Button>("versions_btn");
            _versionsBtn.clicked += RefreshVersions;

            // btn = _cache.Get<Button>("package_list_btn");
            // btn.clicked += () =>
            // {
            //     var scope = context.NpmrcModel.GetDeveloper();
            //     var token = context.UECConfigModel.GetTokenByUsername(scope);
            //
            //     GithubAPI.GetUserAllPackages(scope, token, "npm",
            //         list =>
            //         {
            //             foreach (var overview in list)
            //             {
            //                 Debug.Log(overview.ToString());
            //             }
            //         });
            // };

            _versionsRoot = _cache.Get("version_list_ev");
            _versionScrollView = _versionsRoot.Q<ScrollView>();
            _versionsRoot.SetDisplay(false);

            Refresh();
        }

        public void Refresh()
        {
            var has = context.UECConfigModel.HasConfig() && context.PackageJsonModel.HasPackageJson();
            Self.SetDisplay(has);
        }

        private async void PublishPackage()
        {
            _publishBtn.SetEnabled(false);
            if (EditorUtility.DisplayDialog("publish package", PublishPackageMessage(), "publish", "cancel"))
            {
                await context.PackageModel.PublishPackage();
                RefreshVersions();
            }

            _publishBtn.SetEnabled(true);
        }

        private string PublishPackageMessage()
        {
            var packageName = context.PackageJsonModel.PackageJsonInfo.name;
            var version = context.PackageJsonModel.PackageJsonInfo.version;
            var developer = context.NpmrcModel.GetDeveloper();
            var content = $"publish package:\"{packageName}\", version:\"{version}\" from developer:\"{developer}\" ";
            return content;
        }

        private async void RefreshVersions()
        {
            RefreshVersionsBtn(true);

            await context.PackageModel.GetPackageVersions();

            var overview = context.PackageModel.Overview;

            RefreshVersionsBtn(false);
            if (overview == null)
            {
                Debug.LogError("refresh versions fail");
                return;
            }

            CleanVersions();

            foreach (var version in overview.Versions)
            {
                var element = _pool.Get();
                _versionScrollView.Add(element);

                var lab = element.Q<Label>("version_lab");
                lab.text = version.Version;

                lab = element.Q<Label>("description_lab");
                lab.text = version.Description;

                var btn = element.Q<Button>("remove_btn");
                btn.clickable = new Clickable(() => { DeletePackageVersion(int.Parse(version.Id), version, element); });

                btn = element.Q<Button>("view_btn");
                btn.clickable = new Clickable(() => { Application.OpenURL(version.HtmlUrl); });
            }

            _versionsRoot.SetDisplay(true);
        }

        private void RefreshVersionsBtn(bool refresh)
        {
            _versionsBtn.SetEnabled(!refresh);
            _versionsBtn.text = refresh ? "↻" : "refresh";
        }

        private void CleanVersions()
        {
            while (_versionScrollView.childCount > 0)
            {
                var element = _versionScrollView.ElementAt(0);
                _pool.Return(element);
                _versionScrollView.RemoveAt(0);
            }
        }

        private async void DeletePackageVersion(int versionId, PackageVersion version, VisualElement element)
        {
            // todo 如果只剩最后一个版本,则不能删除包版本,而是删除包
            var button = element.Q<Button>("remove_btn");
            button.SetEnabled(false);
            button.text = "↻";
            bool ret = false;
            if (EditorUtility.DisplayDialog("delete package", version.ToString(), "delete", "cancel"))
            {
                ret = await context.PackageModel.DeletePackageVersion(versionId);
            }

            button.SetEnabled(true);
            button.text = "delete";

            if (ret)
            {
                element.SetDisplay(false);
                RefreshVersions();
            }
        }
    }
}