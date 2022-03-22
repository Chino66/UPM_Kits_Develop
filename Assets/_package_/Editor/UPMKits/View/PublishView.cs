using System.IO;
using CommandTool;
using UIElementsKits;
using UIElementsKits.UIFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UPMKits
{
    public class PublishView : View<PJEUI>
    {
        private VisualElementCache _cache;
        private PJEContext context => UI.Context;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("publish_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            var btn = _cache.Get<Button>("publish_btn");
            btn.clicked += () =>
            {
                System.Diagnostics.Process.Start("npm", @"publish G:\Github\UPM_Kits_Develop\Assets\_package_");
                // Command.RunAsync("npm", ctx =>
                // {
                //     foreach (var message in ctx.Messages)
                //     {
                //         Debug.Log(message);
                //     }
                // });

                // var command = $"call npm publish {context.PackageJsonModel.PackageJsonDirFullPath}";
                // Debug.Log($"click {command}");
                // Command.RunAsync(command, ctx =>
                // {
                //     foreach (var message in ctx.Messages)
                //     {
                //         Debug.Log(message);
                //     }
                // }, true);
            };
        }

        public void Refresh()
        {
        }
    }
}