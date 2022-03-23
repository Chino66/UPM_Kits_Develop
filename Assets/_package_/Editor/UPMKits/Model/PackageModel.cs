using System.Collections.Generic;
using GithubKits;

namespace UPMKits
{
    public class PackageModel
    {
        private PJEContext Context;

        public Dictionary<string /*package name*/, PackageData> PackageDatas;

        public string Developer => Context.NpmrcModel.GetDeveloper();

        public PackageModel(PJEContext context)
        {
            Context = context;

            PackageDatas = new Dictionary<string, PackageData>();
        }
    }
}