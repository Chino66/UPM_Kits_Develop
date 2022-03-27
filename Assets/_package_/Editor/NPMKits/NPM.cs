using System.Diagnostics;
using System.Threading.Tasks;
using CommandTool;

namespace NPMKits
{
    public static class NPM
    {
        public static void Publish(string arguments, CommandCallback callback)
        {
            var cmd = $"publish {arguments}";
            ProcessUtil.Run("npm.cmd", cmd, callback);
        }

        public static async Task PublishAsync(string packageJsonFolder, CommandCallback callback)
        {
            var cmd = $"publish {packageJsonFolder}";
            await ProcessUtil.RunAsync("npm.cmd", cmd, callback);
        }
    }
}