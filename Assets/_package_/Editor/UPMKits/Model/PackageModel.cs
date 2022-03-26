using System.Collections.Generic;
using System.Threading.Tasks;
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
            Overview = await GetPackageVersionsAsync();
        }

        private async Task<PackageOverview> GetPackageVersionsAsync()
        {
            // todo 有效性检查
            var packageName = _context.PackageJsonModel.PackageJsonInfo.name;
            var scope = _context.NpmrcModel.GetDeveloper();
            var token = _context.UECConfigModel.GetTokenByUsername(scope);

            PackageOverview packageOverview = null;
            var message = await GithubAPI.GetThePackageAllVersions(scope, token, packageName, "npm",
                overview =>
                {
                    Debug.Log(overview.ToString());
                    packageOverview = overview;
                });

            if (message != null)
            {
                Debug.Log(message.ToString());
                return null;
            }

            return packageOverview;
        }
    }
}