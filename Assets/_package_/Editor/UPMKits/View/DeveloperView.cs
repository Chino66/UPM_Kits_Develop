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
        private Label _developer;

        private bool showSelect;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("developer_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

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
        }

        private void ShowSelectDeveloperList()
        {
            var temp = _developerScrollView;
            while (temp.childCount > 0)
            {
                var ev = temp.ElementAt(0);
                temp.RemoveAt(0);
            }

            var list = context.UECConfigModel.GetItems();

            foreach (var item in list)
            {
                if (item.IsDeveloper == false)
                {
                    continue;
                }

                var btn = new Button();
                btn.text = item.Username;
                _developerScrollView.Add(btn);

                btn.clicked += () => { Debug.Log(item.Username); };
            }
        }

        public void Refresh()
        {
        }
    }
}