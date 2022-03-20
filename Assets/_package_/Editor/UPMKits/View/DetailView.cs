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

        private Label _noTip;

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
                var textField = element.Q<TextField>("key");
                textField.bindingPath = "key";
                textField = element.Q<TextField>("value");
                textField.bindingPath = "value";
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

            _noTip = _cache.Get<Label>("no_tip");

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
            textField.multiline = true;

            textField = _cache.Get("type_box").Q<TextField>();
            textField.bindingPath = "type";

            textField = _cache.Get("author_box").Q<TextField>();
            textField.bindingPath = "author";

            textField = _cache.Get("license_box").Q<TextField>();
            textField.bindingPath = "license";

            _dependenciesList = _cache.Get("dependencies_list").Q("items");

            var addBtn = _cache.Get<Button>("add_btn");
            addBtn.clicked += () =>
            {
                AddDependency();
                RefreshNoTip();
            };

            var removeBtn = _cache.Get<Button>("remove_btn");
            removeBtn.clicked += () =>
            {
                RemoveDependency();
                RefreshNoTip();
            };

            var applyBtn = _cache.Get<Button>("apply_btn");
            applyBtn.clicked += () => { context.PackageJsonModel.Save(); };

            var revertBtn = _cache.Get<Button>("revert_btn");
            revertBtn.clicked += () =>
            {
                context.PackageJsonModel.Revert();
                Refresh();
            };

            var view = _cache.Get<Button>("view_package_json");
            view.clicked += () => { EditorUtility.RevealInFinder(PackageJsonModel.PackageJsonPath); };

            // var btn = new Button();
            // btn.text = "test";
            // Self.Add(btn);
            // btn.clicked += () => { Debug.Log(context.NpmrcModel.HasLocalNpmrc()); };

            Refresh();
        }

        public void Refresh()
        {
            while (_dependenciesList.childCount > 0)
            {
                var item = _dependenciesList.ElementAt(0);
                Debug.Log($"{item.Q<TextField>("key").text}");
                _pool.Return(item);
                _dependenciesList.RemoveAt(0);
            }

            Self.Unbind();
            packageJson = new SerializedObject(context.PackageJsonModel.PackageJsonInfo);
            Self.Bind(packageJson);
            var dependencies = packageJson.FindProperty("DependencyList");
            for (int i = 0; i < dependencies.arraySize; i++)
            {
                NewDependency(dependencies, i);
            }

            RefreshNoTip();
        }


        private void AddDependency()
        {
            context.PackageJsonModel.PackageJsonInfo.DependencyList.Add(new Dependency());
            packageJson.Update();
            var dependencies = packageJson.FindProperty("DependencyList");

            NewDependency(dependencies, dependencies.arraySize - 1);
        }

        private void NewDependency(SerializedProperty dependencies, int index)
        {
            var element = _pool.Get();
            _dependenciesList.Add(element);
            var dependency = dependencies.GetArrayElementAtIndex(index);
            BindDependency(element, dependency);
        }

        private void BindDependency(VisualElement element, SerializedProperty dependency)
        {
            var key = element.Q<TextField>("key");
            var value = element.Q<TextField>("value");
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

            packageJson.Update();

            var item = _dependenciesList.ElementAt(lastIndex);
            _pool.Return(item);
            _dependenciesList.RemoveAt(lastIndex);
        }

        private void RefreshNoTip()
        {
            _noTip.SetDisplay(_dependenciesList.childCount <= 0);
        }
    }
}