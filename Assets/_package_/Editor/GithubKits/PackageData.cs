using System.Collections.Generic;
using System.Text;
using UnityEditor.PackageManager;

namespace GithubKits
{
    public class PackageVersion
    {
        public string Id;
        public string Version;
        public string Description;

        public override string ToString()
        {
            return $"Id:{Id}, Version:{Version}, Description:{Description}";
        }
    }

    public class PackageOverview
    {
        public PackageOverview()
        {
            Versions = new List<PackageVersion>();
        }

        public string Name;
        public string Url;
        public string Description;
        public string Visibility;
        public List<PackageVersion> Versions;

        public override string ToString()
        {
            var content = new StringBuilder();
            content.Append("Name:").Append(Name).Append("\n");
            content.Append("Url:").Append(Url).Append("\n");
            content.Append("Description:").Append(Description).Append("\n");
            content.Append("Visibility:").Append(Visibility).Append("\n");
            content.Append("Versions:").Append("\n");
            foreach (var version in Versions)
            {
                content.Append("  ").Append(version.ToString()).Append("\n");
            }

            return content.ToString();
        }
    }

    public class PackageData
    {
        public PackageData()
        {
            Overview = new PackageOverview();
        }

        public string Name => Overview.Name;
        public PackageOverview Overview { get; }
        public PackageInfo PackageInfo;
        public bool IsInstalled = false;
        public bool IsDependencies = false;

        public override string ToString()
        {
            return Overview.ToString();
        }
    }
}