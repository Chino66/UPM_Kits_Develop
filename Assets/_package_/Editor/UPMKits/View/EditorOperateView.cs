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
    public class EditorOperateView : View<UKDTUI>
    {
        private VisualElementCache _cache;

        private UKDTContext context => UI.Context;
        
        private Button _applyBtn;
        private Button _revertBtn;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("editor_operate_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            _applyBtn = _cache.Get<Button>("apply_btn");
            _applyBtn.clicked += () => { context.PackageJsonModel.Apply(); };

            _revertBtn = _cache.Get<Button>("revert_btn");
            _revertBtn.clicked += () => { context.PackageJsonModel.Revert(); };

            var stateHandler = new StateHandler();
            stateHandler.AddStateAction(UKDTState.EditorPackageJson, (args) =>
            {
                Self.SetDisplay(true);
            });
            UI.Context.StateMachine.AddHandler(stateHandler);
            
            Refresh();
        }

        public void Refresh()
        {
            // var has = context.UECConfigModel.HasConfig() && context.PackageJsonModel.HasPackageJson();
            // Self.SetDisplay(has);
        }
    }
}