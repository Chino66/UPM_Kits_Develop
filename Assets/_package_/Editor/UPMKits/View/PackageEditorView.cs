using System.IO;
using StateMachineKits;
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

        private SerializedObject _packageJson;

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

            SetTextField("name_box", "name");
            SetTextField("displayName_box", "displayName");
            SetTextField("version_box", "version");
            SetTextField("unity_box", "unity");
            SetTextField("description_box", "description");
            SetTextField("type_box", "type");
            SetTextField("author_box", "author");
            SetTextField("license_box", "license");

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
            });
            
            var stateHandler = new StateHandler();
            var group = new StateGroup("DeveloperView");
            group.AddState(UKDTState.InstallUECTip);
            group.AddState(UKDTState.ConfigUECTip);
            
            stateHandler.AddStateGroupAction(group, (args) =>
            {
                Self.SetDisplay(false);
            });
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
            stateHandler.AddOtherStateAction((args) =>
            {
                Self.SetDisplay(true);
            });
            _stateMachine.AddHandler(stateHandler);

            // Refresh();
        }

        private void SetTextField(string query, string bindingPath)
        {
            var textField = _cache.Get(query).Q<TextField>();
            textField.bindingPath = bindingPath;
            // textField.RegisterValueChangedCallback(evt =>
            // {
            //     context.PackageJsonModel.CheckModify();
            // });
        }

        // private void RefreshOperate()
        // {
        //     var has = context.PackageJsonModel.HasPackageJson();
        //     _operateBtn.SetDisplay(!has);
        //
        //     var view = _cache.Get<Button>("view_package_json");
        //     view.SetEnabled(has);
        // }

        public void Refresh()
        {
            // var has = context.UECConfigModel.HasConfig();
            //
            // Self.SetDisplay(has);
            //
            // if (!has)
            // {
            //     return;
            // }

            // RefreshOperate();

            // if (context.PackageJsonModel.HasPackageJson() == false)
            // {
            //     _detailViewRoot.SetDisplay(false);
            //     return;
            // }
            //
            // _detailViewRoot.SetDisplay(true);

            while (_dependenciesList.childCount > 0)
            {
                var item = _dependenciesList.ElementAt(0);
                Debug.Log($"{item.Q<TextField>("key").text}");
                _pool.Return(item);
                _dependenciesList.RemoveAt(0);
            }

            Self.Unbind();
            _packageJson = new SerializedObject(context.PackageJsonModel.PackageJsonInfo);
            Self.Bind(_packageJson);

            var dependencies = _packageJson.FindProperty("DependencyList");
            for (int i = 0; i < dependencies.arraySize; i++)
            {
                NewDependency(dependencies, i);
            }

            RefreshNoTip();
            RefreshFixInfo();
        }

        private void RefreshFixInfo()
        {
            var repository = _packageJson.FindProperty("repository");
            var sp = repository.FindPropertyRelative("url");
            var label = _cache.Get("repository_box").Q<Label>("url");
            label.bindingPath = "url";
            label.BindProperty(sp);

            var bugs = _packageJson.FindProperty("bugs");
            sp = bugs.FindPropertyRelative("url");
            label = _cache.Get("bugs_box").Q<Label>("url");
            label.bindingPath = "url";
            label.BindProperty(sp);

            var homepage = _packageJson.FindProperty("homepage");
            label = _cache.Get("homepage_box").Q<Label>("url");
            label.bindingPath = "url";
            label.BindProperty(homepage);

            var publishConfig = _packageJson.FindProperty("publishConfig");
            sp = publishConfig.FindPropertyRelative("registry");
            label = _cache.Get("registry_box").Q<Label>("url");
            label.bindingPath = "url";
            label.BindProperty(sp);
        }

        private void AddDependency()
        {
            context.PackageJsonModel.PackageJsonInfo.DependencyList.Add(new Dependency());
            _packageJson.Update();
            var dependencies = _packageJson.FindProperty("DependencyList");

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

            if (list == null || list.Count <= 0)
            {
                return;
            }

            var lastIndex = list.Count - 1;
            list.RemoveAt(lastIndex);

            _packageJson.Update();

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