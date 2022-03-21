using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace UPMKits
{
    [Serializable]
    public class Repository
    {
        public string type;
        public string url;
    }

    [Serializable]
    public class Bugs
    {
        public string url;
    }

    [Serializable]
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

        [JsonIgnore] public List<Dependency> DependencyList;

        public PackageJsonInfo()
        {
            dependencies = new Dictionary<string, string>();
            DependencyList = new List<Dependency>();
            repository = new Repository();
            bugs = new Bugs();
            publishConfig = new PublishConfig();
        }

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

        private PJEContext Context;

        public Action DirtyAction;

        private bool _isDirty;

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                DirtyAction?.Invoke();
                _isDirty = value;
            }
        }

        public PackageJsonModel(PJEContext context)
        {
            Context = context;
            Load();
        }

        public bool HasPackageJson()
        {
            return File.Exists(PackageJsonPath);
        }

        private void Load()
        {
            if (HasPackageJson() == false)
            {
                return;
            }

            var json = File.ReadAllText(PackageJsonPath);
            PackageJsonInfo = JsonConvert.DeserializeObject<PackageJsonInfo>(json) ?? new PackageJsonInfo();

            // 使用SerializedObject与UIElements进行数据绑定,但SerializedObject不支持Dictionary,所以需要额外将Dictionary转成List
            foreach (var dependency in PackageJsonInfo.dependencies.Select(pair => new Dependency
                {key = pair.Key, value = pair.Value}))
            {
                PackageJsonInfo.DependencyList.Add(dependency);
            }

            IsDirty = false;
        }

        public void Create()
        {
            PackageJsonInfo = new PackageJsonInfo();
            Init(PackageJsonInfo);
            Save();
            Revert();
            IsDirty = false;
        }

        public void Update()
        {
            Init(PackageJsonInfo);
            IsDirty = false;
        }

        private void Init(PackageJsonInfo packageJsonInfo)
        {
            var developer = Context.NpmrcModel.GetDeveloper();
            var repositoryName = Context.GitRepositoryModel.RepositoryName;

            // Repository
            var repository = packageJsonInfo.repository;
            repository.type = "git";
            repository.url = $"git+https://github.com/{developer}/{repositoryName}.git";
            // packageJsonInfo.repository = repository;

            // Bugs
            var bugs = packageJsonInfo.bugs;
            bugs.url = $"https://github.com/{developer}/{repositoryName}/issues";
            // packageJsonInfo.bugs = bugs;

            // homepage
            packageJsonInfo.homepage = $"https://github.com/{developer}/{repositoryName}#readme";
            // packageJsonInfo.homepage = homepage;

            // publishConfig
            var publishConfig = packageJsonInfo.publishConfig;
            publishConfig.registry = $"https://npm.pkg.github.com/@{developer}";
            // packageJsonInfo.publishConfig = publishConfig;
        }

        public void Revert()
        {
            Load();
            IsDirty = false;
        }

        public void Save()
        {
            if (PackageJsonInfo == null)
            {
                Debug.LogError("PackageJsonInfo is null");
                return;
            }

            PackageJsonInfo.dependencies.Clear();
            foreach (var dependency in PackageJsonInfo.DependencyList)
            {
                PackageJsonInfo.dependencies.Add(dependency.key, dependency.value);
            }

            if (!HasPackageJson())
            {
                File.Create(PackageJsonPath).Close();
            }

            var json = File.ReadAllText(PackageJsonPath);
            var jObject = JsonConvert.DeserializeObject<JObject>(json) ?? new JObject();
            if (jObject.TryGetValue("dependencies", out var token))
            {
                jObject["dependencies"] = null;
            }

            var jToken = JToken.FromObject(PackageJsonInfo);
            jObject.Merge(jToken);

            var setting = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            var contents = JsonConvert.SerializeObject(jObject, Formatting.Indented, setting);

            // 使用这种方式,则不需要重新加载文本文件
            using var sw = new StreamWriter(PackageJsonPath, false, System.Text.Encoding.Default);
            sw.Write(contents);
            sw.Close();

            IsDirty = false;

            // 使用这种方式需要重新加载文本文件
            // File.WriteAllText(PackageJsonPath, contents);
        }
    }
}