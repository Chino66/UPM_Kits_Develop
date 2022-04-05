using System.IO;
using UnityEngine;

namespace UPMKits
{
    /// <summary>
    /// 插件包文件结构生成器
    /// </summary>
    public static class PackageStructureGenerator
    {
        /*
         * 文件结构:
         * Assets
         *   _package_
         *     Editor
         *       _generate_
         *         PackagePath.cs
         *       {PackageName}.Editor.asmdef
         *     CHANGELOG.md
         *     package.json
         *     README.md
         */


        // public const string PackageDirPath = "Assets/_package_";
        // public const string EditorDirPath = "Assets/_package_/Editor";
        // public const string GenerateDirPath = "Assets/_package_/Editor/_generate_";
        // public const string ResourcesDirPath = "Assets/_package_/Resources";
        // public const string RuntimeDirPath = "Assets/_package_/Runtime";


        public const string PackageJsonPath = "Assets/_package_/package.json";
        public const string PackagePathCsPath = "Assets/_package_/Editor/_generate_/PackagePath.cs";
        public const string ChangeLogMdPath = "Assets/_package_/CHANGELOG.md";
        public const string ReadmeMdPath = "Assets/_package_/README.md";

        public static void Generate()
        {
            CreateFile(PackageJsonPath);
            CreateFile(ChangeLogMdPath);
            CreateFile(ReadmeMdPath);
        }


        public static void CreateFile(string path)
        {
            if (File.Exists(path))
            {
                return;
            }

            var dir = Path.GetDirectoryName(path);
            if (CreateDirectory(dir))
            {
                File.Create(path).Close();
            }
        }

        public static bool CreateDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return true;
            }

            var dir = Path.GetDirectoryName(path);
            if (!CreateDirectory(dir))
            {
                return false;
            }

            Directory.CreateDirectory(path);
            return true;
        }
    }
}