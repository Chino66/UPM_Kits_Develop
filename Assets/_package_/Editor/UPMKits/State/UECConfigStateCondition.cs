using System.Threading.Tasks;
using PackageKits;
using StateMachineKits;

namespace UPMKits.State
{
    public class UECConfigStateCondition : StateCondition
    {
        private UKDTContext Context;

        public UECConfigStateCondition(UKDTContext context)
        {
            Context = context;
        }

        public override async Task<bool> Judge()
        {
            var hasConfig = Context.UECConfigModel.HasConfig();
            if (hasConfig == false)
            {
                var hasInstall = await PackageUtils.HasPackageAsync("com.chino.github.unity.uec");
                if (hasInstall)
                {
                    Context.StateMachine.ChangeState(UKDTState.ConfigUECTip, ".uecconfig not config, please");
                    return true;
                }
                else
                {
                    Context.StateMachine.ChangeState(UKDTState.InstallUECTip, ".uecconfig");
                    return true;
                }
            }

            return false;
        }
    }
}