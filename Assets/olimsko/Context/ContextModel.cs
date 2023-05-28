using Cysharp.Threading.Tasks;
using UnityEngine;

namespace olimsko
{
    public abstract class ContextModel : MonoBehaviour, IContextModel
    {
        public virtual UniTask InitializeStateAsync()
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask LoadDataStateAsync(GameState state)
        {
            return UniTask.CompletedTask;
        }

        public virtual void SaveDataState(GameState state)
        {

        }
    }
}
