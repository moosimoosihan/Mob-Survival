using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace olimsko
{
    public interface ISaveStateManager
    {
        event Action OnBeforeSave;
        event Action OnSaved;
        event Action OnBeforeLoad;
        event Action OnLoaded;

        bool Loading { get; }
        bool Saving { get; }

        bool SaveSlotExists(string slotId);

        bool AnySaveExists();

        void DeleteSaveSlot(string slotId);

        void RenameSaveSlot(string sourceSlotId, string destSlotId);
    }

    public interface ISaveStateManager<TData> : ISaveStateManager where TData : new()
    {
        UniTask SaveAsync(string slotId, TData data);
        UniTask<TData> LoadAsync(string slotId);
        UniTask<TData> LoadOrDefaultAsync(string slotId);
    }
}