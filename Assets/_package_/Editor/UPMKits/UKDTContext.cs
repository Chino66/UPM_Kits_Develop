using StateMachineKits;

namespace UPMKits
{
    public class UKDTContext
    {
        public StateMachine StateMachine;
        public UECConfigModel UECConfigModel;
        public PackageJsonModel PackageJsonModel;
        public NpmrcModel NpmrcModel;
        public GitRepositoryModel GitRepositoryModel;
        public PackageModel PackageModel;

        public UKDTContext()
        {
            StateMachine = new StateMachine();
            
            UECConfigModel = new UECConfigModel();

            PackageJsonModel = new PackageJsonModel(this);

            NpmrcModel = new NpmrcModel(this);

            GitRepositoryModel = new GitRepositoryModel();

            PackageModel = new PackageModel(this);
        }
    }
}