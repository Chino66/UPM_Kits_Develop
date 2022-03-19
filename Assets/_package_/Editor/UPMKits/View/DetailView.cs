using System.IO;
using UIElementsKits;
using UIElementsKits.UIFramework;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace UPMKits
{
    public class DetailView : View<PJEUI>
    {
        private VisualElementCache _cache;

        private PJEContext context => UI.Context;

        private VisualElementPool _pool;

        private VisualElement _dependenciesList;

        private SerializedObject packageJson;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("package_json_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            var uxmlPath = Path.Combine(PackagePath.MainPath, @"Resources/UIElement/dependency_item_uxml.uxml");
            var itemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            _pool = new VisualElementPool(itemAsset);
            _pool.SetGetAction(element =>
            {
                var key = element.Q<TextField>("key");
                key.bindingPath = "key";
                var value = element.Q<TextField>("value");
                value.bindingPath = "value";
            });

            _pool.SetReturnAction(element =>
            {
                var textField = element.Q<TextField>("key");
                textField.value = "";
                textField.Unbind();
                textField = element.Q<TextField>("value");
                textField.value = "";
                textField.Unbind();
            });

            var textField = _cache.Get("name_box").Q<TextField>();
            textField.bindingPath = "name";

            textField = _cache.Get("displayName_box").Q<TextField>();
            textField.bindingPath = "displayName";

            textField = _cache.Get("version_box").Q<TextField>();
            textField.bindingPath = "version";

            textField = _cache.Get("unity_box").Q<TextField>();
            textField.bindingPath = "unity";

            textField = _cache.Get("description_box").Q<TextField>();
            textField.bindingPath = "description";

            textField = _cache.Get("type_box").Q<TextField>();
            textField.bindingPath = "type";

            textField = _cache.Get("author_box").Q<TextField>();
            textField.bindingPath = "author";

            textField = _cache.Get("license_box").Q<TextField>();
            textField.bindingPath = "license";

            _dependenciesList = _cache.Get("dependencies_list").Q("items");

            packageJson = new SerializedObject(context.PackageJsonModel.PackageJsonInfo);
            Self.Bind(packageJson);

            var dependencies = packageJson.FindProperty("DependencyList");

            for (int i = 0; i < dependencies.arraySize; i++)
            {
                var ele = _pool.Get();
                var key = ele.Q<TextField>("key");
                var value = ele.Q<TextField>("value");
                _dependenciesList.Add(ele);

                var dependency = dependencies.GetArrayElementAtIndex(i);
                var property = dependency.FindPropertyRelative("key");
                key.BindProperty(property);
                property = dependency.FindPropertyRelative("value");
                value.BindProperty(property);
            }

            var addBtn = _cache.Get<Button>("add_btn");
            addBtn.clicked += () => { AddDependency(); };

            var removeBtn = _cache.Get<Button>("remove_btn");
            removeBtn.clicked += () => { RemoveDependency(); };

            var applyBtn = _cache.Get<Button>("apply_btn");
            applyBtn.clicked += () => { context.PackageJsonModel.Save(); };

            // var btn = new Button();
            // btn.text = "test";
            // Self.Add(btn);
            // btn.clicked += () =>
            // {
            //     context.PackageJsonModel.PackageJsonInfo.name = "66";
            //
            //     context.PackageJsonModel.PackageJsonInfo.DependencyList[0].key += "666";
            //     
            //     Debug.Log(context.PackageJsonModel.PackageJsonInfo.ToString());
            // };
        }

        public void Refresh()
        {
        }

        private void AddDependency()
        {
            context.PackageJsonModel.PackageJsonInfo.DependencyList.Add(new Dependency());

            var element = _pool.Get();
            var key = element.Q<TextField>("key");
            var value = element.Q<TextField>("value");
            _dependenciesList.Add(element);

            packageJson.Update();

            var dependencies = packageJson.FindProperty("DependencyList");
            var dependency = dependencies.GetArrayElementAtIndex(dependencies.arraySize - 1);
            var property = dependency.FindPropertyRelative("key");
            key.BindProperty(property);
            property = dependency.FindPropertyRelative("value");
            value.BindProperty(property);
        }

        private void RemoveDependency()
        {
            var list = context.PackageJsonModel.PackageJsonInfo.DependencyList;
            var lastIndex = list.Count - 1;
            list.RemoveAt(lastIndex);

            var item = _dependenciesList.ElementAt(lastIndex);
            _pool.Return(item);
            _dependenciesList.RemoveAt(lastIndex);

            packageJson.Update();
        }
    }
}