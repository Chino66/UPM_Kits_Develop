using System.Collections.Generic;
using System.Threading.Tasks;
using CommandTool;
using GithubKits;
using NPMKits;
using UnityEngine;
using UnityEngine.UIElements;

namespace UPMKits
{
    public class PackageModel
    {
        private readonly UKDTContext _context;

        public PackageOverview Overview;

        public PackageModel(UKDTContext context)
        {
            _context = context;

            GetPackageVersions();
        }

        public async Task GetPackageVersions()
        {
            Overview = await GetPackageVersionsAsync();
        }

        public async Task<CommandContext> PublishPackage()
        {
            var arguments = $"{_context.PackageJsonModel.PackageJsonDirFullPath}";
            CommandContext commandContext = null;
            await NPM.PublishAsync(arguments, ctx =>
            {
                Debug.Log(ctx.ToString());
                commandContext = ctx;
            });

            return commandContext;
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

        public async Task<bool> DeletePackageVersion(int versionId)
        {
            var packageName = _context.PackageJsonModel.PackageJsonInfo.name;
            var scope = _context.NpmrcModel.GetDeveloper();
            var token = _context.UECConfigModel.GetTokenByUsername(scope);

            var message = await GithubAPI.DeletePackageVersion(scope, token, packageName, "npm", versionId,
                overview => { Debug.Log(overview.ToString()); });

            if (message != null)
            {
                Debug.Log(message.ToString());
                return false;
            }

            return true;
        }
    }
}