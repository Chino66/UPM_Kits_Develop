using UIElementsKits;
using UIElementsKits.UIFramework;
using UnityEngine;
using UnityEngine.UIElements;

namespace UPMKits
{
    public class DetailView : View<PJEUI>
    {
        private VisualElementCache _cache;
        
        private PJEContext context => UI.Context;
        

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("detail_view_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);
            
            

        }

        public void Refresh()
        {
        }
    }
}