using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace UPMKits
{
    public class Repository
    {
        public string type;
        public string url;
    }

    public class Bugs
    {
        public string url;
    }

    public class PublishConfig
    {
        public string registry;
    }


    public class PackageJsonInfo
    {
        public string name;
        public string displayName;
        public string version;
        public string unity;
        public string description;
        public string type;
        public string author;
        public string license;
        public Repository repository;
        public Bugs bugs;
        public string homepage;
        public PublishConfig publishConfig;
        public Dictionary<string, string> dependencies;

        public override string ToString()
        {
            var context = new StringBuilder();
            context.Append(name).Append("\n");
            context.Append(displayName).Append("\n");
            context.Append(version).Append("\n");
            context.Append(unity).Append("\n");
            context.Append(description).Append("\n");
            context.Append(type).Append("\n");
            context.Append(author).Append("\n");
            context.Append(license).Append("\n");
            context.Append(homepage).Append("\n");
            if (repository != null)
            {
                context.Append(repository.url).Append("\n");
            }
            else
            {
                context.Append("repository is null").Append("\n");
            }

            if (bugs != null)
            {
                context.Append(bugs.url).Append("\n");
            }
            else
            {
                context.Append("bugs is null").Append("\n");
            }

            if (publishConfig != null)
            {
                context.Append(publishConfig.registry).Append("\n");
            }
            else
            {
                context.Append("publishConfig is null").Append("\n");
            }

            if (dependencies != null)
            {
                foreach (var dependency in dependencies)
                {
                    context.Append("  ").Append(dependency.Key).Append("-").Append(dependency.Value).Append("\n");
                }
            }
            else
            {
                context.Append("dependencies is null").Append("\n");
            }

            return context.ToString();
        }
    }

    public class PackageJsonModel
    {
        public const string PackageJsonPath = "Assets/_package_/package.json";


        private PackageJsonInfo _packageJsonInfo;

        public PackageJsonModel()
        {
            Load();
        }

        private void Load()
        {
            var json = File.ReadAllText(PackageJsonPath);
            _packageJsonInfo = JsonConvert.DeserializeObject<PackageJsonInfo>(json) ?? new PackageJsonInfo();
        }

        private void Save()
        {
            var json = File.ReadAllText(PackageJsonPath);
            var jToken = JToken.FromObject(_packageJsonInfo);
            var jObject = JsonConvert.DeserializeObject<JObject>(json);
            jObject.Merge(jToken);

            var setting = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            var contents = JsonConvert.SerializeObject(jObject, Formatting.Indented, setting);

            // 使用这种方式,则不需要重新加载文本文件
            using var sw = new StreamWriter(PackageJsonPath, false, System.Text.Encoding.Default);
            sw.Write(contents);
            sw.Close();

            // 使用这种方式需要重新加载文本文件
            // File.WriteAllText(PackageJsonPath, contents);
        }
    }
}