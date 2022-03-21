using System.IO;
using UIElementsKits;
using UIElementsKits.UIFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UPMKits
{
    public class DeveloperView : View<PJEUI>
    {
        private VisualElementCache _cache;
        private PJEContext context => UI.Context;

        private ScrollView _developerScrollView;
        private VisualElementPool _pool;
        private Label _developer;

        private bool showSelect;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("developer_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            var uxmlPath = Path.Combine(PackagePath.MainPath, @"Resources/UIElement/developer_item_uxml.uxml");
            var itemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            _pool = new VisualElementPool(itemAsset);
            
            _developer = _cache.Get<Label>("developer_name");
            _developerScrollView = _cache.Get<ScrollView>("developer_list_sv");
            _developerScrollView.SetDisplay(showSelect);

            var btn = _cache.Get<Button>("select_developer");
            btn.clicked += () =>
            {
                showSelect = !showSelect;
                _developerScrollView.SetDisplay(showSelect);
                if (showSelect)
                {
                    ShowSelectDeveloperList();
                }
            };

            RefreshDeveloper();
        }

        private void RefreshDeveloper()
        {
            _developer.text = context.NpmrcModel.GetDeveloper();
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

                // button.clicked += () => { Debug.Log(item.Username); };
                button.clickable = new Clickable(() =>
                {
                    Debug.Log(item.Username);
                    context.NpmrcModel.ChangeDeveloper(item.Username);
                    RefreshDeveloper();
                });
            }
        }

        public void Refresh()
        {
        }
    }
}