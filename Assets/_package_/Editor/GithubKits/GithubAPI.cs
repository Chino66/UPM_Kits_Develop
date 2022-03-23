using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandTool;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GithubKits
{
    public static class GithubAPI
    {
        /*
         * 获取用户所有包列表
         * https://docs.github.com/cn/rest/reference/packages#list-packages-for-a-user
         */

        #region list-packages-for-a-user

        public static async Task GetPackageList(string username, string token,
            string packageType,
            Action<List<PackageOverview>> callback)
        {
            var command = GetPackageListCommand(username, token, packageType);

            await Command.RunAsync(command, (ctx) =>
            {
                var builder = new StringBuilder();
                foreach (var msg in ctx.Messages)
                {
                    builder.Append(msg);
                }

                var content = builder.ToString();

                if (content == "")
                {
                    return;
                }

                var jArray = JArray.Parse(content);

                var list = new List<PackageOverview>();

                foreach (var child in jArray.Children())
                {
                    var overview = new PackageOverview
                    {
                        Name = child["name"].ToString(),
                        Url = child["url"].ToString(),
                        Description = child["repository"]["description"].ToString(),
                        Visibility = child["visibility"].ToString()
                    };

                    list.Add(overview);
                }

                callback?.Invoke(list);
            });
        }

        /// <summary>
        /// https://docs.github.com/cn/rest/reference/packages#list-packages-for-a-user
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="token"></param>
        /// <param name="packageType"></param>
        /// <returns></returns>
        private static string GetPackageListCommand(string scope, string token, string packageType)
        {
            var command =
                $"curl -u {scope}:{token} -H \"Accept: application/vnd.github.v3+json\" https://api.github.com/users/{scope}/packages?package_type={packageType}";
            return command;
        }

        #endregion

        /**
         * 删除指定版本的包
         * https://docs.github.com/cn/rest/reference/packages#delete-package-version-for-a-user
         */

        #region delete-package-version-for-a-user

        public static async Task DeletePackageVersion(string username, string token,
            string packageName,
            string packageType,
            int packageVersionId,
            Action<bool> callback)
        {
            var command = GetDeletePackageVersionCommand(username, token, packageName, packageType, packageVersionId);
            await Command.RunAsync(command, ctx =>
            {
                var builder = new StringBuilder();
                foreach (var msg in ctx.Messages)
                {
                    Debug.Log(msg);
                    builder.Append(msg);
                }

                var content = builder.ToString();
                if (string.IsNullOrEmpty(content))
                {
                    callback?.Invoke(true);
                }
                else
                {
                    callback?.Invoke(false);
                }
            });
        }

        /// <summary>
        /// https://docs.github.com/cn/rest/reference/packages#delete-package-version-for-a-user
        /// </summary>
        private static string GetDeletePackageVersionCommand(string username, string token,
            string package_name,
            string package_type,
            int package_version_id)
        {
            var command =
                $"curl -u {username}:{token} -X DELETE -H \"Accept: application/vnd.github.v3+json\" https://api.github.com/users/{username}/packages/{package_type}/{package_name}/versions/{package_version_id}";
            return command;
        }

        #endregion

        /*
         * 获取这个包的所有版本
         * https://docs.github.com/cn/rest/reference/packages#get-all-package-versions-for-a-package-owned-by-a-user
         */

        #region get-all-package-versions-for-a-package-owned-by-a-user

        public static async Task GetAllPackageVersions(string username, string token,
            string packageName,
            string packageType,
            Action<PackageOverview> callback)
        {
            var command = GetAllPackageVersionsCommand(username, token, packageName, packageType);
            await Command.RunAsync(command, ctx =>
            {
                var builder = new StringBuilder();
                foreach (var msg in ctx.Messages)
                {
                    builder.Append(msg);
                }

                var content = builder.ToString();

                if (content == "")
                {
                    return;
                }

                var jArray = JArray.Parse(content);

                var overview = new PackageOverview
                {
                    Name = packageName
                };
                var versions = overview.Versions;
                versions.AddRange(jArray.Children().Select(child => new PackageVersion
                {
                    Id = child["id"].ToString(),
                    Version = child["name"].ToString(),
                    Description = child["description"].ToString()
                }));

                callback?.Invoke(overview);
            });
        }

        /// <summary>
        /// https://docs.github.com/cn/rest/reference/packages#get-all-package-versions-for-a-package-owned-by-a-user
        /// </summary>
        private static string GetAllPackageVersionsCommand(string scope, string token,
            string packageName,
            string packageType = "npm")
        {
            var command =
                $"curl -u {scope}:{token} -H \"Accept: application/vnd.github.v3+json\" https://api.github.com/users/{scope}/packages/{packageType}/{packageName}/versions";
            return command;
        }

        #endregion
    }
}