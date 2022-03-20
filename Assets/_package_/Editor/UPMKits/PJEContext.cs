namespace UPMKits
{
    public class PJEContext
    {
        public UECConfigModel UECConfigModel;
        public PackageJsonModel PackageJsonModel;
        public NpmrcModel NpmrcModel;

        public PJEContext()
        {
            UECConfigModel = new UECConfigModel();

            PackageJsonModel = new PackageJsonModel();

            NpmrcModel = new NpmrcModel(this);
        }
    }
}