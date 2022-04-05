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
    public class CheckView : View<UKDTUI>
    {
        private VisualElementCache _cache;
        private StateMachine _stateMachine => UI.Context.StateMachine;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("check_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            var uxmlPath = Path.Combine(PackagePath.MainPath, @"Resources/UIElement/developer_item_uxml.uxml");
            var itemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);

            var installTip = _cache.Get("install_tip");
            installTip.SetDisplay(false);
            var btn = installTip.Q<Button>("install_btn");
            btn.clicked += InstallUECAsync;

            var openTip = _cache.Get("open_tip");
            openTip.SetDisplay(false);
            btn = openTip.Q<Button>("open_btn");
            btn.clicked += () => { UKDTWindow.OpenUEC(); };

            var stateHandler = new StateHandler();
            stateHandler.AddStateAction(UKDTState.InstallUECTip, (args) =>
            {
                Self.SetDisplay(true);
                installTip.SetDisplay(true);
                openTip.SetDisplay(false);

                var content = args as string;
                var label = installTip.Q<Label>("tip_lab");
                label.text = content;
            });

            stateHandler.AddStateAction(UKDTState.ConfigUECTip, (args) =>
            {
                Self.SetDisplay(true);
                installTip.SetDisplay(false);
                openTip.SetDisplay(true);

                var content = args as string;
                var label = openTip.Q<Label>("tip_lab");
                label.text = content;
            });

            stateHandler.AddOtherStateAction((args) =>
            {
                installTip.SetDisplay(false);
                openTip.SetDisplay(false);
                Self.SetDisplay(false);
            });

            _stateMachine.AddHandler(stateHandler);
        }

        private async void InstallUECAsync()
        {
            var installTip = _cache.Get("install_tip");
            var btn = installTip.Q<Button>("install_btn");
            btn.SetTextLoading();
            var rst = await PackageUtils.HasPackageAsync("com.chino.github.unity.uec");

            if (!rst)
            {
                await PackageUtils.AddPackageAsync("com.chino.github.unity.uec@file:G:/Github/UEC/Assets/_package_",
                    addRst => { });
            }

            btn.text = "Install UEC";
        }
    }
}