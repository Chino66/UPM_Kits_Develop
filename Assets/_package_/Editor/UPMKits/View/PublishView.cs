using System;
using System.Diagnostics;
using System.IO;
using CommandTool;
using UIElementsKits;
using UIElementsKits.UIFramework;
using UnityEditor;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

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
                // run();
                ExecuteCommand("");
                // System.Diagnostics.Process.Start("npm", @"publish G:\Github\UPM_Kits_Develop\Assets\_package_");
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

        private void ExecuteCommand(string command)
        {
            var processInfo =
                new ProcessStartInfo("npm.cmd", @"publish G:\Github\UPM_Kits_Develop\Assets\_package_ & exit");
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            var process = Process.Start(processInfo);

            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                Debug.Log("output>>" + e.Data);
            };
            process.BeginOutputReadLine();

            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => { Debug.Log("error>>" + e.Data); };
            process.BeginErrorReadLine();

            process.WaitForExit();

            Debug.Log($"ExitCode: {process.ExitCode}");
            process.Close();
        }
    }
}