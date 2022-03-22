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

        public void Copy(Repository repository)
        {
            type = repository.type;
            url = repository.url;
        }

        public bool Compare(Repository repository)
        {
            return type != repository.type || url != repository.url;
        }
    }

    [Serializable]
    public class Bugs
    {
        public string url;

        public void Copy(Bugs bugs)
        {
            url = bugs.url;
        }

        public bool Compare(Bugs bugs)
        {
            return url != bugs.url;
        }
    }

    [Serializable]
    public class PublishConfig
    {
        public string registry;

        public void Copy(PublishConfig publishConfig)
        {
            registry = publishConfig.registry;
        }

        public bool Compare(PublishConfig publishConfig)
        {
            return registry != publishConfig.registry;
        }
    }

    [Serializable]
    public class Dependency
    {
        public string key;
        public string value;

        public void Copy(Dependency dependency)
        {
            key = dependency.key;
            value = dependency.value;
        }

        public bool Compare(Dependency dependency)
        {
            return key != dependency.key || value != dependency.value;
        }
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

        public void Copy(PackageJsonInfo info)
        {
            name = info.name;
            displayName = info.displayName;
            version = info.version;
            unity = info.unity;
            description = info.description;
            type = info.type;
            author = info.author;
            license = info.license;
            repository.Copy(info.repository);
            bugs.Copy(info.bugs);
            homepage = info.homepage;
            publishConfig.Copy(info.publishConfig);
            dependencies = new Dictionary<string, string>(info.dependencies);
            DependencyList = new List<Dependency>(info.DependencyList);
        }

        public bool Compare(PackageJsonInfo info)
        {
            if (name != info.name)
            {
                return true;
            }

            if (displayName != info.displayName)
            {
                return true;
            }

            if (version != info.version)
            {
                return true;
            }

            if (unity != info.unity)
            {
                return true;
            }

            if (description != info.description)
            {
                return true;
            }

            if (type != info.type)
            {
                return true;
            }

            if (author != info.author)
            {
                return true;
            }

            if (license != info.license)
            {
                return true;
            }

            if (repository.Compare(info.repository))
            {
                return true;
            }

            if (bugs.Compare(info.bugs))
            {
                return true;
            }

            if (homepage != info.homepage)
            {
                return true;
            }

            if (publishConfig.Compare(info.publishConfig))
            {
                return true;
            }

            if (DependencyList.Count != info.DependencyList.Count)
            {
                return true;
            }

            for (var i = 0; i < info.DependencyList.Count; i++)
            {
                if (DependencyList[i].Compare(info.DependencyList[i]))
                {
                    return true;
                }
            }

            return false;
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

/*
 * todo 关于UIElements和SerializedObject数据绑定的一些问题
 * 优点:
 *      UIElements和SerializedObject数据绑定可以很好的实现双向改变,通过修改组件,对应绑定值会得到修改,修改值则组件会更新变化
 *      绑定过程较为简单,只需要让数据类继承自ScriptableObject即可封装成SerializedObject进行绑定
 * 缺点:
 *      SerializedObject对于属性,字典等字段不能识别,故属性,字典是不能数据绑定的,这对于数据绑定的泛用性大打折扣
 *      UIElements和SerializedObject数据绑定当值发生变化时,缺乏消息通知机制,比如当值改变时,需要通知数据类设置脏标记,这一步就办不到
 *          如果将数据类字段设为属性,则可以在值改变set时进行处理,但SerializedObject不支持属性,故这个方式不可行
 *          使用RegisterValueChangedCallback监听组件变化,但触发回调时,值已经是最新值了,不能通过前后状态对他们进行判断
 *      SerializedObject绑定后,数据更新不是实时的,而是在下一帧进行更新
 */

    public class PackageJsonModel
    {
        public const string PackageJsonPath = "Assets/_package_/package.json";

        public string PackageJsonDirFullPath;

        private PackageJsonInfo _previousPackageJsonInfo;
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
            IsDirty = false;
            _previousPackageJsonInfo = new PackageJsonInfo();
            _previousPackageJsonInfo.Copy(PackageJsonInfo);

            var dir = new DirectoryInfo("Assets/_package_");
            PackageJsonDirFullPath = dir.FullName;
            
            var dirPath = Path.GetDirectoryName(PackageJsonPath);
            PackageJsonDirFullPath = Path.GetFullPath(dirPath);
        }

        public bool CheckModify()
        {
            IsDirty = PackageJsonInfo.Compare(_previousPackageJsonInfo);
            return IsDirty;
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
        }

        public void Create()
        {
            PackageJsonInfo = new PackageJsonInfo();
            Init(PackageJsonInfo);
            Save();
            Revert();
            IsDirty = false;
            _previousPackageJsonInfo = new PackageJsonInfo();
            _previousPackageJsonInfo.Copy(PackageJsonInfo);
        }

        public void Update()
        {
            Init(PackageJsonInfo);
            IsDirty = false;
            _previousPackageJsonInfo.Copy(PackageJsonInfo);
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
            _previousPackageJsonInfo = new PackageJsonInfo();
            _previousPackageJsonInfo.Copy(PackageJsonInfo);
        }

        public void Apply()
        {
            Save();
            IsDirty = false;
            _previousPackageJsonInfo.Copy(PackageJsonInfo);
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

            // 使用这种方式需要重新加载文本文件
            // File.WriteAllText(PackageJsonPath, contents);
        }
    }
}