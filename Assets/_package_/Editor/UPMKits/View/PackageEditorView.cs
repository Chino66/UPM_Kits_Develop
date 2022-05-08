using System.IO;
using DataBinding;
using StateMachineKits;
using UIElementsKits;
using UIElementsKits.DataBinding;
using UIElementsKits.UIFramework;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace UPMKits
{
    public class PackageEditorView : View<UKDTUI>
    {
        private VisualElementCache _cache;

        private UKDTContext context => UI.Context;
        private StateMachine _stateMachine => UI.Context.StateMachine;

        private VisualElementPool _pool;
        private VisualElement _dependenciesList;
        private VisualElement _detailViewRoot;
        private Label _noTip;
        private Button _operateBtn;
        private Button _removeBtn;

        // private SerializedObject _packageJson;
        private Binding _packageJson;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("package_editor_root");
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

            _detailViewRoot = _cache.Get("detail_view_root");
            _noTip = _cache.Get<Label>("no_tip");

            _dependenciesList = _cache.Get("dependencies_list").Q("items");

            var addBtn = _cache.Get<Button>("add_btn");
            addBtn.clicked += () =>
            {
                AddDependency();
                RefreshNoTip();
            };

            _removeBtn = _cache.Get<Button>("remove_btn");
            _removeBtn.clicked += () =>
            {
                RemoveDependency();
                RefreshNoTip();
            };

            var view = _cache.Get<Button>("view_package_json");
            view.clicked += () => { EditorUtility.RevealInFinder(PackageJsonModel.PackageJsonPath); };

            _operateBtn = _cache.Get<Button>("operate_package_json");
            _operateBtn.clickable = new Clickable(() =>
            {
                PackageStructureGenerator.Generate();
                context.PackageJsonModel.Create();
                AssetDatabase.Refresh();
                UI.Refresh();
                // Refresh();
            });

            var stateHandler = new StateHandler();
            var group = new StateGroup("DeveloperView");
            group.AddState(UKDTState.InstallUECTip);
            group.AddState(UKDTState.ConfigUECTip);

            stateHandler.AddStateGroupAction(group, (args) => { Self.SetDisplay(false); });
            stateHandler.AddStateAction(UKDTState.NoPackageJson, (args) =>
            {
                _operateBtn.SetDisplay(true);
                var view = _cache.Get<Button>("view_package_json");
                view.SetEnabled(false);
                _detailViewRoot.SetDisplay(false);
            });
            stateHandler.AddStateAction(UKDTState.EditorPackageJson, (args) =>
            {
                _operateBtn.SetDisplay(false);
                var view = _cache.Get<Button>("view_package_json");
                view.SetEnabled(true);
                _detailViewRoot.SetDisplay(true);
                Refresh();
            });
            stateHandler.AddOtherStateAction((args) => { Self.SetDisplay(true); });
            _stateMachine.AddHandler(stateHandler);

            // Refresh();
        }

        private void SetTextField(string query, string bindingPath)
        {
            var textField = _cache.Get(query).Q<TextField>();
            textField.bindingPath = bindingPath;
            textField.value = bindingPath;
        }

        private void DisplayNameChangeListen()
        {
            var textField = _cache.Get("displayName_box").Q<TextField>();
            textField.RegisterValueChangedCallback(evt =>
            {
                if ((string.IsNullOrEmpty(evt.previousValue) || evt.previousValue == "") && evt.newValue != "")
                {
                    // display name changed
                }
            });
        }

        public void Refresh()
        {
            RefreshDepend();
            RefreshNoTip();
            RefreshFixInfo();
        }

        private void RefreshDepend()
        {
            while (_dependenciesList.childCount > 0)
            {
                var item = _dependenciesList.ElementAt(0);
                Debug.Log($"{item.Q<TextField>("key").text}");
                _pool.Return(item);
                _dependenciesList.RemoveAt(0);
            }

            _packageJson = new Binding(context.PackageJsonModel.PackageJsonInfo);

            _packageJson.RegisterPostSetEvent<string>("name", (value) =>
            {
                Debug.Log($"value is {value}");
                Debug.Log(
                    $"context.PackageJsonModel.PackageJsonInfo.name is {context.PackageJsonModel.PackageJsonInfo.name}");
            });
            RefreshTextField();

            var dependencies = context.PackageJsonModel.PackageJsonInfo.DependencyList;
            for (int i = 0; i < dependencies.Count; i++)
            {
                NewDependency(dependencies[i]);
            }
        }

        private void RefreshTextField()
        {
            SetTextField("name_box", "name");
            SetTextField("displayName_box", "displayName");
            SetTextField("version_box", "version");
            SetTextField("unity_box", "unity");
            SetTextField("description_box", "description");
            SetTextField("type_box", "type");
            SetTextField("author_box", "author");
            SetTextField("license_box", "license");
            _packageJson.Bind(Self);
        }

        private void RefreshFixInfo()
        {
            var repository = _packageJson.FindBinding("repository");
            var label = _cache.Get("repository_box").Q<Label>("url");
            label.bindingPath = "url";
            repository.Bind(label);

            var bugs = _packageJson.FindBinding("bugs");
            label = _cache.Get("bugs_box").Q<Label>("url");
            label.bindingPath = "url";
            bugs.Bind(label);

            label = _cache.Get("homepage_box").Q<Label>("url");
            label.bindingPath = "homepage";
            _packageJson.Bind(label);


            var publishConfig = _packageJson.FindBinding("publishConfig");
            label = _cache.Get("registry_box").Q<Label>("url");
            label.bindingPath = "url";
            publishConfig.Bind(label);
        }

        private void AddDependency()
        {
            var dependency = new Dependency();
            context.PackageJsonModel.PackageJsonInfo.DependencyList.Add(dependency);
            NewDependency(dependency);
        }

        private void NewDependency(Dependency dependency)
        {
            var element = _pool.Get();
            _dependenciesList.Add(element);
            var dependencyBinding = dependency.GetBinding();
            dependencyBinding.Bind(element);
        }

        private void RemoveDependency()
        {
            var list = context.PackageJsonModel.PackageJsonInfo.DependencyList;

            if (list == null || list.Count <= 0)
            {
                return;
            }

            var lastIndex = list.Count - 1;
            list.RemoveAt(lastIndex);

            var item = _dependenciesList.ElementAt(lastIndex);
            _pool.Return(item);
            _dependenciesList.RemoveAt(lastIndex);
        }

        private void RefreshNoTip()
        {
            _noTip.SetDisplay(_dependenciesList.childCount <= 0);
            _removeBtn.SetEnabled(_dependenciesList.childCount > 0);
        }
    }
}