using System.Collections.Generic;
using System.IO;
using DataBinding;
using PackageKits;
using StateMachineKits;
using UIElementsKits;
using UIElementsKits.DataBinding;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
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
        private Binding _packageJsonBinding;

        private List<string> packages;


        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("package_editor_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            var uxmlPath = Path.Combine(PackagePath.MainPath, @"Resources/UIElement/dependency_item_uxml.uxml");
            var itemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            _pool = new VisualElementPool(itemAsset);
            _pool.SetCreateFunc(element =>
            {
                var packageNameField = element.Q<TextField>("key");
                var versionField = element.Q<TextField>("value");
                var packageNameAnchor = element.Q<VisualElement>("package_name");

                var choices = context.PackageModel.PackageNames;
                var normalField = new PopupField<string>("", choices, 0);
                packageNameAnchor.Add(normalField);
                normalField.RegisterCallback<ChangeEvent<string>>((evt) => { packageNameField.value = evt.newValue; });

                /*每次包名发生变化就需要重新生成版本popup*/
                packageNameField.RegisterCallback<ChangeEvent<string>>(evt =>
                {
                    if (string.IsNullOrEmpty(evt.newValue))
                    {
                        return;
                    }

                    /*清除旧的版本popup*/
                    var version = element.Q<VisualElement>("version");
                    var pf = version.Q<PopupField<string>>();
                    if (pf != null)
                    {
                        version.Remove(pf);
                    }

                    versionField.value = "";

                    var versions = context.PackageModel.GetVersionViaName(evt.newValue);

                    if (versions == null || versions.Count <= 0)
                    {
                        return;
                    }

                    /*根据包名生成版本popup*/
                    pf = new PopupField<string>("", versions, 0);
                    version.Add(pf);
                    pf.RegisterCallback<ChangeEvent<string>>((evt) => { versionField.value = evt.newValue; });
                    versionField.value = pf.value;
                });


                return element;
            });
            _pool.SetGetAction(element =>
            {
                var textField = element.Q<TextField>("key");
                textField.bindingPath = "key";
                textField = element.Q<TextField>("value");
                textField.bindingPath = "value";
            });
            _pool.SetReturnAction(element =>
            {
                element.UnBind();
                var textField = element.Q<TextField>("key");
                textField.value = "";
                textField = element.Q<TextField>("value");
                textField.value = "";
            });

            _detailViewRoot = _cache.Get("detail_view_root");
            _noTip = _cache.Get<Label>("no_tip");

            _dependenciesList = _cache.Get("dependencies_list").Q("items");

            var autoBtn = _cache.Get<Button>("auto_btn");
            autoBtn.clicked += () =>
            {
                /*
                 * todo 
                 * 1.找到所有插件目录下的.asmdef
                 * 2.解析所有.asmdef的引用的guid
                 * 3.找到guid所在的插件包包名,添加依赖
                 */
                var rst = AssetDatabase.FindAssets("t:asmdef", new string[] {"Assets/_package_"});
                foreach (var s in rst)
                {
                    var path = AssetDatabase.GUIDToAssetPath(s);
                    var ass = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                    Debug.Log(path);
                    Debug.Log(ass.text);
                }

                var p = AssetDatabase.GUIDToAssetPath("6af49b14aaf12014fa07178d3b986ed0");
                Debug.Log($"p is {p}");
                // p is Packages/com.chino.command.tool/Editor/CommandTool.Editor.asmdef
            };


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

                _packageJsonBinding = context.PackageJsonModel.PackageJsonInfo.GetBinding();
                _packageJsonBinding.RegisterPropertyChangeCallback((index) =>
                {
                    context.PackageJsonModel.IsDirty = true;
                    // Debug.Log($"when property change index is {index}");
                });

                Refresh();
            });
            stateHandler.AddOtherStateAction((args) => { Self.SetDisplay(true); });
            _stateMachine.AddHandler(stateHandler);
        }

        private void SetTextField(string query, string bindingPath)
        {
            var textField = _cache.Get(query).Q<TextField>();
            textField.bindingPath = bindingPath;
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
            Self.UnBind();
            _packageJsonBinding.Clear();
            RefreshTextField();
            RefreshDepend();
            RefreshFixInfo();
            RefreshNoTip();
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
            _packageJsonBinding.Bind(Self);
        }

        private void RefreshDepend()
        {
            while (_dependenciesList.childCount > 0)
            {
                // var item = _dependenciesList.ElementAt(0);
                // Debug.Log($"{item.Q<TextField>("key").text}");
                // _pool.Return(item);
                // _dependenciesList.RemoveAt(0);
                RemoveDependency(0);
            }

            var dependencies = context.PackageJsonModel.PackageJsonInfo.DependencyList;
            foreach (var dependency in dependencies)
            {
                NewDependency(dependency);
            }
        }


        private void RefreshFixInfo()
        {
            var repository = _packageJsonBinding.FindBinding("repository");
            var label = _cache.Get("repository_box").Q<Label>("url");
            label.bindingPath = "url";
            repository.Bind(label);

            var bugs = _packageJsonBinding.FindBinding("bugs");
            label = _cache.Get("bugs_box").Q<Label>("url");
            label.bindingPath = "url";
            bugs.Bind(label);

            label = _cache.Get("homepage_box").Q<Label>("url");
            label.bindingPath = "homepage";
            _packageJsonBinding.Bind(label);


            var publishConfig = _packageJsonBinding.FindBinding("publishConfig");
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
            dependencyBinding.RegisterPropertyChangeCallback(_packageJsonBinding.PropertyChangeCallback);
        }

        private void RemoveDependency(int index)
        {
            var list = context.PackageJsonModel.PackageJsonInfo.DependencyList;

            if (list == null || list.Count <= index)
            {
                return;
            }

            /*解除事件通知*/
            var dependency = context.PackageJsonModel.PackageJsonInfo.DependencyList[index];
            var dependencyBinding = dependency.GetBinding();
            dependencyBinding.UnregisterPropertyChangeCallback(_packageJsonBinding.PropertyChangeCallback);
            context.PackageJsonModel.PackageJsonInfo.DependencyList.RemoveAt(index);

            /*移除元素*/
            var item = _dependenciesList.ElementAt(index);
            _pool.Return(item);
            _dependenciesList.RemoveAt(index);

            context.PackageJsonModel.IsDirty = true;
        }

        private void RemoveDependency()
        {
            var list = context.PackageJsonModel.PackageJsonInfo.DependencyList;

            if (list == null || list.Count <= 0)
            {
                return;
            }

            var lastIndex = list.Count - 1;
            RemoveDependency(lastIndex);
        }

        private void RefreshNoTip()
        {
            _noTip.SetDisplay(_dependenciesList.childCount <= 0);
            _removeBtn.SetEnabled(_dependenciesList.childCount > 0);
        }
    }
}