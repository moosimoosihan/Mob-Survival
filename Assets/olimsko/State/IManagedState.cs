using Cysharp.Threading.Tasks;
using UnityEngine;

namespace olimsko
{
    public interface IManagedState<TState> : IOSMEntity where TState : StateMap
    {
        void SaveDataState(TState state);

        UniTask LoadDataStateAsync(TState state);

        UniTask InitializeStateAsync();
    }
}
