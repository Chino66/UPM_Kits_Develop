using System;
using System.IO;
using PackageKits;
using StateMachineKits;
using UIElementsKits;
using UIElementsKits.UIFramework;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

namespace UPMKits
{
    public class DeveloperView : View<UKDTUI>
    {
        private VisualElementCache _cache;
        private UKDTContext context => UI.Context;
        private StateMachine _stateMachine => UI.Context.StateMachine;

        private ScrollView _developerScrollView;
        private VisualElementPool _pool;
        private Label _developer;
        private bool _showSelect;
        private VisualElement _developerList;
        private Button _selectBtn;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("developer_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            var uxmlPath = Path.Combine(PackagePath.MainPath, @"Resources/UIElement/developer_item_uxml.uxml");
            var itemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            _pool = new VisualElementPool(itemAsset);
            _pool.SetCreateFunc(element =>
            {
                element.style.flexGrow = 1;
                return element;
            });

            _developer = _cache.Get<Label>("developer_name");
            _developerScrollView = _cache.Get<ScrollView>("developer_list_sv");
            _developerList = _developerScrollView.parent;
            _developerList.SetDisplay(_showSelect);

            _selectBtn = _cache.Get<Button>("select_developer");
            _selectBtn.clicked += ToggleSelectList;

            var stateHandler = new StateHandler();
            var group = new StateGroup("DeveloperView");
            group.AddState(UKDTState.InstallUECTip);
            group.AddState(UKDTState.ConfigUECTip);

            stateHandler.AddStateGroupAction(group, (args) => { Self.SetDisplay(false); });
            stateHandler.AddOtherStateAction((args) => { Self.SetDisplay(true); });
            _stateMachine.AddHandler(stateHandler);

            Refresh();
        }

        public void Refresh()
        {
            RefreshDeveloper();
        }

        private void RefreshDeveloper()
        {
            _developer.text = context.NpmrcModel.GetDeveloper();
        }

        private void ToggleSelectList()
        {
            _showSelect = !_showSelect;
            _developerList.SetDisplay(_showSelect);
            if (_showSelect)
            {
                ShowSelectDeveloperList();
            }
        }

        private void ShowSelectDeveloperList()
        {
            var temp = _developerScrollView;

            while (temp.childCount > 0)
            {
                var element = temp.ElementAt(0);
                _pool.Return(element);
                temp.RemoveAt(0);
            }

            var list = context.UECConfigModel.GetItems();

            foreach (var item in list)
            {
                if (item.IsDeveloper == false)
                {
                    continue;
                }

                var element = _pool.Get();
                var button = element.Q<Button>();
                button.text = item.Username;
                _developerScrollView.Add(button);

                button.clickable = new Clickable(() =>
                {
                    context.NpmrcModel.ChangeDeveloper(item.Username);
                    context.PackageJsonModel.Update();
                    ToggleSelectList();
                    RefreshDeveloper();
                });
            }
        }
    }
}