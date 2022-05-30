using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DataBinding;
using DataBinding.BindingExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace UPMKits
{
    [Serializable]
    public class Repository
    {
        public string type { get; set; }
        public string url { get; set; }
    }

    [Serializable]
    public class Bugs
    {
        public string url { get; set; }
    }

    [Serializable]
    public class PublishConfig
    {
        public string registry { get; set; }
    }

    [Serializable]
    public class Dependency
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class PackageJsonInfo
    {
        public string name { get; set; }
        public string displayName { get; set; }
        public string version { get; set; }
        public string unity { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string author { get; set; }
        public string license { get; set; }
        public Repository repository { get; set; }
        public Bugs bugs { get; set; }
        public string homepage { get; set; }
        public PublishConfig publishConfig { get; set; }
        public Dictionary<string, string> dependencies { get; set; }

        [JsonIgnore]
        public List<Dependency> DependencyList;

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

        public string PackageJsonDirFullPath;

        public PackageJsonInfo PackageJsonInfo { get; private set; }

        private UKDTContext Context;

        public Action<bool> DirtyAction;

        private bool _isDirty;

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                DirtyAction?.Invoke(_isDirty);
            }
        }

        public PackageJsonModel(UKDTContext context)
        {
            Context = context;
            Load();
            IsDirty = false;
            // _previousPackageJsonInfo = new PackageJsonInfo();
            // _previousPackageJsonInfo.Copy(PackageJsonInfo);

            // var dir = new DirectoryInfo("Assets/_package_");
            // PackageJsonDirFullPath = dir.FullName;

            var dirPath = Path.GetDirectoryName(PackageJsonPath);
            PackageJsonDirFullPath = Path.GetFullPath(dirPath);
        }

        // public bool CheckModify()
        // {
        //     IsDirty = PackageJsonInfo.Compare(_previousPackageJsonInfo);
        //     return IsDirty;
        // }

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
        }

        public void Create()
        {
            PackageJsonInfo = new PackageJsonInfo();
            Init(PackageJsonInfo);
            Save();
            Revert();
            IsDirty = false;
            // _previousPackageJsonInfo = new PackageJsonInfo();
            // _previousPackageJsonInfo.Copy(PackageJsonInfo);
        }

        public void Update()
        {
            if (HasPackageJson() == false)
            {
                return;
            }

            Init(PackageJsonInfo);
            IsDirty = false;
            // _previousPackageJsonInfo.Copy(PackageJsonInfo);
        }

        private void Init(PackageJsonInfo packageJsonInfo)
        {
            var developer = Context.NpmrcModel.GetDeveloper();
            var repositoryName = Context.GitRepositoryModel.RepositoryName;

            // Repository
            var repository = packageJsonInfo.repository;
            repository.type = "git";
            repository.url = $"git+https://github.com/{developer}/{repositoryName}.git";

            // Bugs
            var bugs = packageJsonInfo.bugs;
            bugs.url = $"https://github.com/{developer}/{repositoryName}/issues";

            // homepage
            packageJsonInfo.homepage = $"https://github.com/{developer}/{repositoryName}#readme";

            // publishConfig
            var publishConfig = packageJsonInfo.publishConfig;
            publishConfig.registry = $"https://npm.pkg.github.com/@{developer}";
        }

        public void Revert()
        {
            Load();
            IsDirty = false;
            // _previousPackageJsonInfo = new PackageJsonInfo();
            // _previousPackageJsonInfo.Copy(PackageJsonInfo);
        }

        public void Apply()
        {
            Save();
            IsDirty = false;
            // _previousPackageJsonInfo.Copy(PackageJsonInfo);
        }

        private void Save()
        {
            if (PackageJsonInfo == null)
            {
                Debug.LogError("PackageJsonInfo is null");
                return;
            }

            PackageJsonInfo.dependencies.Clear();
            foreach (var dependency in PackageJsonInfo.DependencyList)
            {
                if (string.IsNullOrEmpty(dependency.key))
                {
                    continue;
                }

                if (PackageJsonInfo.dependencies.ContainsKey(dependency.key))
                {
                    Debug.LogError($"package name {dependency.key} already exist!");
                    continue;
                }

                PackageJsonInfo.dependencies.Add(dependency.key, dependency.value);
            }


            if (!HasPackageJson())
            {
                CreateFile(PackageJsonPath);
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

            // 使用这种方式需要重新加载文本文件
            // File.WriteAllText(PackageJsonPath, contents);
        }

        private void CreatePackageStructure()
        {
        }

        private void CreateFile(string path)
        {
            if (File.Exists(path))
            {
                return;
            }

            var dir = Path.GetDirectoryName(path);
            if (CreateDirectory(dir))
            {
                Debug.Log($"create file {path}");
                File.Create(path).Close();
            }
        }

        private bool CreateDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return true;
            }

            var dir = Path.GetDirectoryName(path);
            if (CreateDirectory(dir))
            {
                Debug.Log($"create dir {path}");
                Directory.CreateDirectory(path);
                return true;
            }

            return false;
        }
    }
}