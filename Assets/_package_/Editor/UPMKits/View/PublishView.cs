using System;
using System.Diagnostics;
using System.IO;
using CommandTool;
using GithubKits;
using NPMKits;
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
                var arguments = $"{context.PackageJsonModel.PackageJsonDirFullPath}";
                NPM.Publish(arguments, ctx => { Debug.Log(ctx.ToString()); });
            };

            btn = _cache.Get<Button>("version_list_btn");
            btn.clicked += () =>
            {
                var packageName = context.PackageJsonModel.PackageJsonInfo.name;
                var scope = context.NpmrcModel.GetDeveloper();
                var token = context.UECConfigModel.GetTokenByUsername(scope);

                GithubAPI.GetAllPackageVersions(scope, token, packageName, "npm",
                    overview => { Debug.Log(overview.ToString()); });
            };

            btn = _cache.Get<Button>("package_list_btn");
            btn.clicked += () =>
            {
                var scope = context.NpmrcModel.GetDeveloper();
                var token = context.UECConfigModel.GetTokenByUsername(scope);

                GithubAPI.GetPackageList(scope, token, "npm",
                    list =>
                    {
                        foreach (var overview in list)
                        {
                            Debug.Log(overview.ToString());
                        }
                    });
            };
        }


        public void Refresh()
        {
        }

        private void ExecuteCommand(string command)
        {
            var cmd = $"publish {context.PackageJsonModel.PackageJsonDirFullPath}";
            var processInfo = new ProcessStartInfo("npm.cmd", $@"{cmd} & exit")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

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