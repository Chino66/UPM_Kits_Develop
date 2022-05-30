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
            _applyBtn.clicked += () =>
            {
                context.PackageJsonModel.Apply();
                AssetDatabase.Refresh();
                UI.Refresh();
            };

            _revertBtn = _cache.Get<Button>("revert_btn");
            _revertBtn.clicked += () =>
            {
                context.PackageJsonModel.Revert();
                UI.Refresh();
            };

            var stateHandler = new StateHandler();
            stateHandler.AddStateAction(UKDTState.EditorPackageJson,
                (args) =>
                {
                    Self.SetDisplay(true);
                    context.PackageJsonModel.DirtyAction += (isDirty) =>
                    {
                        _applyBtn.SetEnabled(isDirty);
                        _revertBtn.SetEnabled(isDirty);
                    };
                    Refresh();
                });
            UI.Context.StateMachine.AddHandler(stateHandler);
            
            Self.SetDisplay(false);
        }

        public void Refresh()
        {
            var isDirty = context.PackageJsonModel.IsDirty;
            _applyBtn.SetEnabled(isDirty);
            _revertBtn.SetEnabled(isDirty);
        }
    }
}