namespace UPMKits
{
    public class PJEContext
    {
        public UECConfigModel UECConfigModel;
        public PackageJsonModel PackageJsonModel;

        public PJEContext()
        {
            UECConfigModel = new UECConfigModel();
            
            PackageJsonModel = new PackageJsonModel();
        }
    }
}