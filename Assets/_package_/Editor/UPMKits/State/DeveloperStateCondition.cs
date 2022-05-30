using System.Threading.Tasks;
using PackageKits;
using StateMachineKits;

namespace UPMKits.State
{
    public class DeveloperStateCondition : StateCondition
    {
        private UKDTContext Context;

        public DeveloperStateCondition(UKDTContext context)
        {
            Context = context;
        }

        public override async Task<bool> Judge()
        {
            var hasDeveloper = Context.NpmrcModel.GetDeveloper() != "* None *";
            if (hasDeveloper == false)
            {
                var hasInstall = await PackageUtils.HasPackageAsync("com.chino.github.unity.uec");
                if (hasInstall)
                {
                    Context.StateMachine.ChangeState(UKDTState.ConfigUECTip, "no developer, please");
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