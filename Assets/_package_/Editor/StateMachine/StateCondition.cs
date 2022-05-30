using System.Threading.Tasks;

namespace StateMachineKits
{
    public abstract class StateCondition
    {
        public virtual async Task<bool> Judge()
        {
            return false;
        }
    }
}