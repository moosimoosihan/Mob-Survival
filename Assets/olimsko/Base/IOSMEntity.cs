using UnityEngine;
using Cysharp.Threading.Tasks;

namespace olimsko
{
    public interface IOSMEntity
    {
        UniTask InitializeAsync();

        void Reset();

        void DestroyThis();
    }

    public interface IOSMEntity<TConfig> : IOSMEntity
        where TConfig : Configuration
    {
        TConfig Configuration { get; }
    }
}
