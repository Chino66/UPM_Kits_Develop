using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UPMKits
{
    public class NpmrcConfigItem
    {
        public string registry;
        public string token;
    }

    public class NpmrcConfig
    {
        public List<NpmrcConfigItem> Items = new List<NpmrcConfigItem>();
    }

    public class NpmrcModel
    {
        private const string NpmrcFile = ".npmrc";

        private const string TagContent = "#[uecconfig]";

        public const string NpmrcLocalPath = "Assets/_package_/.npmrc";
        public static string NpmrcPath => Path.Combine(UserProfilePath(), NpmrcFile);

        public static string UserProfilePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        private UKDTContext Context;

        public NpmrcModel(UKDTContext context)
        {
            Context = context;
        }

        public bool HasLocalNpmrc()
        {
            return File.Exists(NpmrcLocalPath);
        }

        public bool HasGlobalNpmrc()
        {
            return File.Exists(NpmrcPath);
        }

        public bool HasNpmrc()
        {
            return HasGlobalNpmrc() || HasLocalNpmrc();
        }

        public string GetDeveloper()
        {
            if (HasLocalNpmrc())
            {
                var lines = File.ReadLines(NpmrcLocalPath);
                foreach (var line in lines)
                {
                    if (!line.Contains(TagContent))
                    {
                        continue;
                    }

                    if (line.Length == TagContent.Length)
                    {
                        continue;
                    }

                    var username = line.Substring(TagContent.Length, line.Length - TagContent.Length);
                    username = username.Replace("@", "");
                    return username;
                }
            }

            var items = Context.UECConfigModel.GetItems();

            if (items == null)
            {
                return "* None *";
            }

            var ret = items.Select(item => item).Where(item => item.IsDeveloper);

            return ret.Any() ? ret.First().Username : "* None *";
        }

        public void ChangeDeveloper(string username)
        {
            var items = Context.UECConfigModel.GetItems();
            var ret = items.Select(item => item).Where(item => item.Username == username);
            var item = ret.First();
            var lines = new List<string>();
            lines.Add($"{TagContent}@{item.Username}");
            lines.Add($"@{item.Username}:registry=https://npm.pkg.github.com");
            lines.Add($"//npm.pkg.github.com/:_authToken={item.Token}");
            lines.Add($"{TagContent}");
            File.WriteAllLines(NpmrcLocalPath, lines);
        }
    }
}