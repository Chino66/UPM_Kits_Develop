using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CommandTool;
using UnityEditor;

namespace NPMKits
{
    public static class NPM
    {
        public static void Publish(string arguments, CommandCallback callback)
        {
            var cmd = $"publish {arguments}";
            ProcessUtil.Run(NpmcmdPath, cmd, callback);
        }

        public static async Task PublishAsync(string packageJsonFolder, CommandCallback callback)
        {
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
    }
}