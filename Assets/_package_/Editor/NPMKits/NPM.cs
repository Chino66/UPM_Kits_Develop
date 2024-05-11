using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CommandTool;
using UnityEditor;

namespace NPMKits
{
    public static class NPM
    {
        static NPM()
        {
            FindNpmcmd();
        }

        public static void Publish(string arguments, CommandCallback callback)
        {
            var cmd = $"publish {arguments}";
            ProcessUtil.Run(NpmcmdPath, cmd, callback);
        }

        public static async Task PublishAsync(string packageJsonFolder, CommandCallback callback)
        {
            await FindNpmcmd();
            var cmd = $"publish {packageJsonFolder}";
            await ProcessUtil.RunAsync(NpmcmdPath, cmd, callback);
        }

        private static string _npmcmdPath;

        private static string NpmcmdPath
        {
            get
            {
                if (_npmcmdPath == null)
                {
                    var appPath = EditorApplication.applicationPath;
                    var appDir = Path.GetDirectoryName(appPath);
                    _npmcmdPath = Path.Combine(appDir, @"Data\Tools\nodejs\npm.cmd");
                }

                return _npmcmdPath;
            }
        }

        private static async Task FindNpmcmd()
        {
            if (string.IsNullOrEmpty(_npmcmdPath) == false)
            {
                return;
            }

            // cmd中添加/c可以执行命令后关闭窗口并且没有如Microsoft Windows [版本 10.0.19042.867]这样的信息
            var cmd = $"/c where npm.cmd";
            await ProcessUtil.RunAsync(@"C:\Windows\System32\cmd.exe", cmd,
                ctx =>
                {
                    foreach (var message in ctx.Messages)
                    {
                        if (string.IsNullOrEmpty(message) == false && message.Contains("npm.cmd"))
                        {
                            _npmcmdPath = message;
                            UnityEngine.Debug.Log($"find npm.cmd:{_npmcmdPath}");
                            break;
                        }
                    }
                });
        }
    }
}