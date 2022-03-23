using System.Collections.Generic;
using GithubKits;

namespace UPMKits
{
    public class PackageModel
    {
        private PJEContext Context;

        public PackageOverview Overview;

        public PackageModel(PJEContext context)
        {
            Context = context;
        }
    }
}