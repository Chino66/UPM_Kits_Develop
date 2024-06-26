using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandTool;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GithubKits
{
    public static class GithubAPI
    {
        private const string Message = "message";
        private const string DocumentationUrl = "documentation_url";

        private static bool IsMessage(JToken jToken)
        {
            if (jToken.Type != JTokenType.Object)
            {
                return false;
            }

            if (jToken[Message] != null && jToken[DocumentationUrl] != null)
            {
                return true;
            }

            return false;
        }
        /*
         * 获取用户所有包列表
         * https://docs.github.com/cn/rest/reference/packages#list-packages-for-a-user
         */

        #region list-packages-for-a-user

        public static async Task<ResponseMessage> GetUserAllPackages(string username, string token,
            string packageType,
            Action<List<PackageOverview>> callback)
        {
            ResponseMessage message = null;
            var command = GetUserAllPackagesCommand(username, token, packageType);
            await Command.RunAsync(command, (ctx) =>
            {
                var builder = new StringBuilder();
                foreach (var msg in ctx.Messages)
                {
                    builder.Append(msg);
                }

                var content = builder.ToString();

                if (content == "" || string.IsNullOrEmpty(content))
                {
                    message = new ResponseMessage {Message = "Response Message is Empty"};
                    return;
                }

                var jToken = JToken.Parse(content);

                if (IsMessage(jToken))
                {
                    message = jToken.ToObject<ResponseMessage>();
                    return;
                }

                var jArray = JArray.Parse(content);

                var list = jArray.Children().Select(child => new PackageOverview
                {
                    Name = child["name"].ToString(), Url = child["url"].ToString(),
                    Description = child["repository"]["description"].ToString(),
                    Visibility = child["visibility"].ToString()
                }).ToList();

                callback?.Invoke(list);
            });

            return message;
        }

        /// <summary>
        /// https://docs.github.com/cn/rest/reference/packages#list-packages-for-a-user
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="token"></param>
        /// <param name="packageType"></param>
        /// <returns></returns>
        private static string GetUserAllPackagesCommand(string scope, string token, string packageType)
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

        public static async Task<ResponseMessage> DeletePackageVersion(string username, string token,
            string packageName,
            string packageType,
            int packageVersionId,
            Action<bool> callback)
        {
            ResponseMessage message = null;
            var command = DeletePackageVersionCommand(username, token, packageName, packageType, packageVersionId);
            await Command.RunAsync(command, ctx =>
            {
                var builder = new StringBuilder();
                foreach (var msg in ctx.Messages)
                {
                    builder.Append(msg);
                }

                var content = builder.ToString();

                // 没有返回值说明操作成功
                if (string.IsNullOrEmpty(content))
                {
                    callback?.Invoke(true);
                    return;
                }

                var jToken = JToken.Parse(content);
                if (IsMessage(jToken))
                {
                    message = jToken.ToObject<ResponseMessage>();
                    callback?.Invoke(false);
                    return;
                }
            });

            return message;
        }

        /// <summary>
        /// https://docs.github.com/cn/rest/reference/packages#delete-package-version-for-a-user
        /// </summary>
        private static string DeletePackageVersionCommand(string username, string token,
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

        public static async Task<ResponseMessage> GetThePackageAllVersions(string username, string token,
            string packageName,
            string packageType,
            Action<PackageOverview> callback)
        {
            ResponseMessage message = null;
            var command = GetThePackageAllVersionsCommand(username, token, packageName, packageType);
            await Command.RunAsync(command, ctx =>
            {
                var builder = new StringBuilder();

                foreach (var msg in ctx.Messages)
                {
                    builder.Append(msg);
                }

                var content = builder.ToString();

                if (content == "" || string.IsNullOrEmpty(content))
                {
                    message = new ResponseMessage {Message = "Response Message is Empty"};
                    return;
                }

                var jToken = JToken.Parse(content);

                if (IsMessage(jToken))
                {
                    message = jToken.ToObject<ResponseMessage>();
                    return;
                }

                var jArray = JArray.Parse(content);
                var overview = new PackageOverview
                {
                    Name = packageName
                };
                var versions = overview.Versions;

                try
                {
                    versions.AddRange(jArray.Children().Select(child =>
                    {
                        var packageVersion = new PackageVersion
                        {
                            Id = child["id"].ToString(),
                            HtmlUrl = child["html_url"].ToString()
                        };

                        if (child["name"] != null)
                        {
                            packageVersion.Version = child["name"].ToString();
                        }
                        if (child["description"] != null)
                        {
                            packageVersion.Description = child["description"].ToString();
                        }
                        return  packageVersion;
                    }));
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }

                callback?.Invoke(overview);
            });

            return message;
        }

        /// <summary>
        /// https://docs.github.com/cn/rest/reference/packages#get-all-package-versions-for-a-package-owned-by-a-user
        /// </summary>
        private static string GetThePackageAllVersionsCommand(string scope, string token,
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