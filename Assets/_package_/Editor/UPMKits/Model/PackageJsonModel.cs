using System;
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

    [Serializable]
    public class Dependency
    {
        public string key;
        public string value;
    }


    public class PackageJsonInfo : ScriptableObject
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

        [JsonIgnore]
        public List<Dependency> DependencyList;

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
            context.Append(repository.url).Append("\n");
            context.Append(bugs.url).Append("\n");
            context.Append(publishConfig.registry).Append("\n");
            context.Append("dependencies").Append("\n");
            foreach (var dependency in dependencies)
            {
                context.Append("  ").Append(dependency.Key).Append(":").Append(dependency.Value).Append("\n");
            }

            return context.ToString();
        }
    }

    public class PackageJsonModel
    {
        public const string PackageJsonPath = "Assets/_package_/package.json";

        public PackageJsonInfo PackageJsonInfo { get; private set; }

        public PackageJsonModel()
        {
            Load();
        }

        private void Load()
        {
            var json = File.ReadAllText(PackageJsonPath);
            PackageJsonInfo = JsonConvert.DeserializeObject<PackageJsonInfo>(json) ?? new PackageJsonInfo();

            // 使用SerializedObject与UIElements进行数据绑定,但SerializedObject不支持Dictionary,所以需要额外将Dictionary转成List
            PackageJsonInfo.DependencyList = new List<Dependency>();
            foreach (var pair in PackageJsonInfo.dependencies)
            {
                var dependency = new Dependency {key = pair.Key, value = pair.Value};
                PackageJsonInfo.DependencyList.Add(dependency);
            }
        }

        public void Save()
        {
            PackageJsonInfo.dependencies.Clear();
            foreach (var dependency in PackageJsonInfo.DependencyList)
            {
                PackageJsonInfo.dependencies.Add(dependency.key, dependency.value);
            }

            var json = File.ReadAllText(PackageJsonPath);
            var jToken = JToken.FromObject(PackageJsonInfo);
            var jObject = JsonConvert.DeserializeObject<JObject>(json);
            jObject["dependencies"] = null;
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