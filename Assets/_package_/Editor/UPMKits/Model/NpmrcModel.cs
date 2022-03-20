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

        private PJEContext Context;

        public NpmrcModel(PJEContext context)
        {
            Context = context;
        }

        public bool HasLocalNpmrc()
        {
            return File.Exists(NpmrcLocalPath);
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

            var ret = items.Select(item => item).Where(item => item.IsDeveloper);

            return ret.Any() ? ret.First().Username : null;
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

        //
        // public void Update()
        // {
        //     var lines = File.ReadLines(NpmrcPath);
        //     var configLines = new List<string>();
        //
        //     var startRead = true;
        //     foreach (var line in lines)
        //     {
        //         if (line.Contains(TagContent))
        //         {
        //             startRead = !startRead;
        //             continue;
        //         }
        //
        //         if (!startRead)
        //         {
        //             continue;
        //         }
        //
        //         configLines.Add(line);
        //     }
        //
        //     var config = GetNpmrcConfig();
        //     foreach (var item in config.Items)
        //     {
        //         configLines.Add(TagContent);
        //         configLines.Add(item.registry);
        //         configLines.Add(item.token);
        //         configLines.Add(TagContent);
        //     }
        //
        //     foreach (var line in configLines)
        //     {
        //         Debug.Log(line);
        //     }
        //
        //     File.WriteAllLines(NpmrcPath, configLines);
        // }
        //
        // private NpmrcConfig GetNpmrcConfig()
        // {
        //     var config = new NpmrcConfig();
        //     var items = Context.UECConfigModel.GetItems();
        //
        //     foreach (var npmrcItem in from item in items
        //         where !string.IsNullOrEmpty(item.Token)
        //         select new NpmrcConfigItem
        //         {
        //             registry = $"@{item.Username}:registry=https://npm.pkg.github.com",
        //             token = $"//npm.pkg.github.com/:_authToken={item.Token}",
        //         })
        //     {
        //         config.Items.Add(npmrcItem);
        //     }
        //
        //     return config;
        // }
    }
}