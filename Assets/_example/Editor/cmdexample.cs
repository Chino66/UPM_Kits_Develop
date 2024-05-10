using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandTool;
using UnityEditor;
using UnityEngine;

public class cmdexample
{
    [MenuItem("Test/PublishTest")]
    static void PublishTest()
    {
        var cmd = @"G:\Github\Game_ECS_Develop\Assets\_package_\ & exit";
        PublishAsync(cmd, ctx => { Debug.Log(ctx.ToString()); });
    }
    
    public static async Task PublishAsync(string packageJsonFolder, CommandCallback callback)
    {
        var cmd = $"publish {packageJsonFolder}";
        await ProcessUtil.RunAsync("npm.cmd", cmd, callback);
    }
    
    [MenuItem("Test/Echo")]
    static void Echo()
    {
        var cmd = @"echo 66";
        ProcessUtil.RunAsync("cmd", cmd, ctx =>
        {
            Debug.Log(ctx.ToString());
        });
    }
    
    [MenuItem("Test/Npmcmd")]
    static void Npmcmd()
    {
        var cmd = $" -v";
        ProcessUtil.RunAsync("npm.cmd", cmd, ctx =>
        {
            Debug.Log(ctx.ToString());
        });
    }
    
    [MenuItem("Test/Npm")]
    static void Npm()
    {
        var cmd = $" -v";
        ProcessUtil.RunAsync("npm", cmd, ctx =>
        {
            Debug.Log(ctx.ToString());
        });
    }

    
    [MenuItem("Test/Java")]
    static void Java()
    {
        var cmd = $" -version";
        ProcessUtil.RunAsync("java", cmd, ctx =>
        {
            Debug.Log(ctx.ToString());
        });
    }
    
    [MenuItem("Test/WhereNpmcmd")]
    static void WhereNpmcmd()
    {
        var cmd = $" npm.cmd";
        ProcessUtil.RunAsync("where", cmd, ctx =>
        {
            Debug.Log(ctx.ToString());
        });
    }
    
    [MenuItem("Test/RunNpmcmdFullPath")]
    static void RunNpmcmdFullPath()
    {
        var cmd = $" -v";
        ProcessUtil.RunAsync(@"D:\Program Files\nodejs\npm.cmd", cmd, ctx =>
        {
            Debug.Log(ctx.ToString());
        });
    }
    
    [MenuItem("Test/RunNpmcmdPublishFullPath")]
    static void RunNpmcmdPublishFullPath()
    {
        var cmd = @"publish G:\Github\Game_ECS_Develop\Assets\_package_";
        // ProcessUtil.RunAsync(@"D:\Program Files\nodejs\npm.cmd", cmd, ctx =>
        ProcessUtil.RunAsync("npm.cmd", cmd, ctx =>
        {
            Debug.Log(ctx.ToString());
        });
    }
}