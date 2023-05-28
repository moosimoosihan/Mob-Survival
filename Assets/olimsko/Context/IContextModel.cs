using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace olimsko
{
    public interface IContextModel
    {
        void SaveDataState(GameState state);
        UniTask LoadDataStateAsync(GameState state);
        UniTask InitializeStateAsync();
    }
}
