using System;
using System.Diagnostics;
using System.IO;
using CommandTool;
using GithubKits;
using NPMKits;
using UIElementsKits;
using UIElementsKits.UIFramework;
using UnityEditor;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace UPMKits
{
    public class OperateView : View<PJEUI>
    {
        private VisualElementCache _cache;
        private PJEContext context => UI.Context;

        private VisualElementPool _pool;

        private VisualElement _versionsRoot;

        private ScrollView _versionScrollView;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("operate_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            var uxmlPath = Path.Combine(PackagePath.MainPath, @"Resources/UIElement/version_item_uxml.uxml");
            var itemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            _pool = new VisualElementPool(itemAsset);

            var btn = _cache.Get<Button>("publish_btn");
            btn.clicked += () =>
            {
                var arguments = $"{context.PackageJsonModel.PackageJsonDirFullPath}";
                NPM.Publish(arguments, ctx => { Debug.Log(ctx.ToString()); });
            };

            btn = _cache.Get<Button>("version_list_btn");
            btn.clicked += RefreshVersions;

            btn = _cache.Get<Button>("package_list_btn");
            btn.clicked += () =>
            {
                var scope = context.NpmrcModel.GetDeveloper();
                var token = context.UECConfigModel.GetTokenByUsername(scope);

                GithubAPI.GetUserAllPackages(scope, token, "npm",
                    list =>
                    {
                        foreach (var overview in list)
                        {
                            Debug.Log(overview.ToString());
                        }
                    });
            };

            _versionsRoot = _cache.Get("version_list_ev");
            _versionScrollView = _versionsRoot.Q<ScrollView>();
            _versionsRoot.SetDisplay(false);
        }


        private async void RefreshVersions()
        {
            var packageName = context.PackageJsonModel.PackageJsonInfo.name;
            var scope = context.NpmrcModel.GetDeveloper();
            var token = context.UECConfigModel.GetTokenByUsername(scope);

            await GithubAPI.GetThePackageAllVersions(scope, token, packageName, "npm",
                overview =>
                {
                    Debug.Log(overview.ToString());
                    context.PackageModel.Overview = overview;
                });

            var overview = context.PackageModel.Overview;
            foreach (var version in overview.Versions)
            {
                var element = _pool.Get();
                _versionScrollView.Add(element);

                var lab = element.Q<Label>("version_lab");
                lab.text = version.Version;

                lab = element.Q<Label>("description_lab");
                lab.text = version.Description;

                var btn = element.Q<Button>("remove_btn");
                btn.clickable = new Clickable(() => { DeletePackageVersion(int.Parse(version.Id)); });
            }

            _versionsRoot.SetDisplay(true);
        }

        private async void DeletePackageVersion(int versionId)
        {
            Debug.Log(versionId);
            var packageName = context.PackageJsonModel.PackageJsonInfo.name;
            var scope = context.NpmrcModel.GetDeveloper();
            var token = context.UECConfigModel.GetTokenByUsername(scope);

            await GithubAPI.DeletePackageVersion(scope, token, packageName, "npm", versionId,
                overview => { Debug.Log(overview.ToString()); });
        }

        public void Refresh()
        {
        }
    }
}