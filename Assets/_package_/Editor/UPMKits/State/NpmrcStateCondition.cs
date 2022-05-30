using System.Threading.Tasks;
using PackageKits;
using StateMachineKits;

namespace UPMKits.State
{
    public class NpmrcStateCondition : StateCondition
    {
        private UKDTContext Context;

        public NpmrcStateCondition(UKDTContext context)
        {
            Context = context;
        }

        public override async Task<bool> Judge()
        {
            var hasNpmrc = Context.NpmrcModel.HasNpmrc();
            if (hasNpmrc == false)
            {
                var hasInstall = await PackageUtils.HasPackageAsync("com.chino.github.unity.uec");
                if (hasInstall)
                {
                    Context.StateMachine.ChangeState(UKDTState.ConfigUECTip, ".npmrc not config, please");
                    return true;
                }
                else
                {
                    Context.StateMachine.ChangeState(UKDTState.InstallUECTip, ".npmrc");
                    return true;
                }
            }

            return false;
        }
    }
}