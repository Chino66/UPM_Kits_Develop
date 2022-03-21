namespace UPMKits
{
    public class PJEContext
    {
        public UECConfigModel UECConfigModel;
        public PackageJsonModel PackageJsonModel;
        public NpmrcModel NpmrcModel;
        public GitRepositoryModel GitRepositoryModel;

        public PJEContext()
        {
            UECConfigModel = new UECConfigModel();

            PackageJsonModel = new PackageJsonModel(this);

            NpmrcModel = new NpmrcModel(this);

            GitRepositoryModel = new GitRepositoryModel();
        }
    }
}