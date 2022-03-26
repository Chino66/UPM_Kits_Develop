using System.Collections.Generic;
using GithubKits;
using UnityEngine;
using UnityEngine.UIElements;

namespace UPMKits
{
    public class PackageModel
    {
        private readonly PJEContext _context;

        public PackageOverview Overview;

        public PackageModel(PJEContext context)
        {
            _context = context;

            GetPackageVersions();
        }

        private async void GetPackageVersions()
        {
            // todo 有效性检查
            var packageName = _context.PackageJsonModel.PackageJsonInfo.name;
            var scope = _context.NpmrcModel.GetDeveloper();
            var token = _context.UECConfigModel.GetTokenByUsername(scope);

            var message = await GithubAPI.GetThePackageAllVersions(scope, token, packageName, "npm",
                overview =>
                {
                    Debug.Log(overview.ToString());
                    _context.PackageModel.Overview = overview;
                });

            if (message != null)
            {
                Debug.Log(message.ToString());
                return;
            }

            var overview = _context.PackageModel.Overview;
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

                btn = element.Q<Button>("view_btn");
                btn.clickable = new Clickable(() => { Application.OpenURL(version.HtmlUrl); });
            }

            _versionsRoot.SetDisplay(true);
        }
    }
}