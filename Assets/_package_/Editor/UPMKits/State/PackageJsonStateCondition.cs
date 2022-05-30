using System.Threading.Tasks;
using PackageKits;
using StateMachineKits;

namespace UPMKits.State
{
    public class PackageJsonStateCondition : StateCondition
    {
        private UKDTContext Context;

        public PackageJsonStateCondition(UKDTContext context)
        {
            Context = context;
        }

        public override async Task<bool> Judge()
        {
            var hasPackageJson = Context.PackageJsonModel.HasPackageJson();
            if (hasPackageJson == false)
            {
                Context.StateMachine.ChangeState(UKDTState.NoPackageJson);
                return true;
            }

            Context.StateMachine.ChangeState(UKDTState.EditorPackageJson);

            return false;
        }
    }
}