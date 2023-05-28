using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

namespace olimsko
{
    public abstract class SaveStateManager
    {
        public event Action OnBeforeSave;
        public event Action OnSaved;
        public event Action OnBeforeLoad;
        public event Action OnLoaded;

        public bool Loading { get; private set; }
        public bool Saving { get; private set; }

        public abstract bool SaveSlotExists(string slotId);
        public abstract bool AnySaveExists();
        public abstract void DeleteSaveSlot(string slotId);
        public abstract void RenameSaveSlot(string sourceSlotId, string destSlotId);

        protected abstract bool PrettifyJson { get; }
        protected abstract bool Binary { get; }
        protected abstract string Extension { get; }

        protected void InvokeOnBeforeSave()
        {
            Saving = true;
            OnBeforeSave?.Invoke();
        }

        protected void InvokeOnSaved()
        {
            Saving = false;
            OnSaved?.Invoke();
        }

        protected void InvokeOnBeforeLoad()
        {
            Loading = true;
            OnBeforeLoad?.Invoke();
        }

        protected void InvokeOnLoaded()
        {
            Loading = false;
            OnLoaded?.Invoke();
        }
    }

    public class SaveStateManager<TData> : SaveStateManager, ISaveStateManager<TData> where TData : new()
    {
        protected virtual string GameDataPath => GetGameDataPath();
        protected virtual string SaveDataPath => string.Concat(GameDataPath, "/SaveData");
        protected override bool PrettifyJson => Debug.isDebugBuild;
        protected override bool Binary => false;
        protected override string Extension => "json";

        private bool saveInProgress;

        public async UniTask SaveAsync(string slotId, TData data)
        {
            while (saveInProgress && Application.isPlaying)
                await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);

            saveInProgress = true;

            InvokeOnBeforeSave();

            await SerializeDataAsync(slotId, data);
            InvokeOnSaved();

            saveInProgress = false;
        }

        public async UniTask<TData> LoadAsync(string slotId)
        {
            InvokeOnBeforeLoad();

            if (!SaveSlotExists(slotId))
                throw new Exception($"Slot '{slotId}' not found when loading '{typeof(TData)}' data.");

            var data = await DeserializeDataAsync(slotId);
            InvokeOnLoaded();

            return data;
        }

        public async UniTask<TData> LoadOrDefaultAsync(string slotId)
        {
            if (!SaveSlotExists(slotId))
                await SerializeDataAsync(slotId, new TData());

            return await LoadAsync(slotId);
        }

        public override bool SaveSlotExists(string slotId) => File.Exists(SlotIdToFilePath(slotId));

        public override bool AnySaveExists()
        {
            if (!Directory.Exists(SaveDataPath)) return false;
            return Directory.GetFiles(SaveDataPath, $"*.{Extension}", SearchOption.TopDirectoryOnly).Length > 0;
        }

        public override void DeleteSaveSlot(string slotId)
        {
            if (!SaveSlotExists(slotId)) return;
            BaseUtil.IO.DeleteFile(SlotIdToFilePath(slotId));
        }

        public override void RenameSaveSlot(string sourceSlotId, string destSlotId)
        {
            if (!SaveSlotExists(sourceSlotId)) return;

            var sourceFilePath = SlotIdToFilePath(sourceSlotId);
            var destFilePath = SlotIdToFilePath(destSlotId);
            BaseUtil.IO.MoveFile(sourceFilePath, destFilePath);
        }

        protected virtual string SlotIdToFilePath(string slotId) => string.Concat(SaveDataPath, "/", slotId, $".{Extension}");

        protected virtual async UniTask SerializeDataAsync(string slotId, TData data)
        {
            var jsonData = OSManager.GetConfiguration<StateConfiguration>().UseNewtonsoftJson ? JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
            {
                // ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto
            }) : JsonUtility.ToJson(data, PrettifyJson);
            var filePath = SlotIdToFilePath(slotId);
            BaseUtil.IO.CreateDirectory(SaveDataPath);

            if (Binary)
            {
                var bytes = await BaseUtil.String.ZipStringAsync(jsonData);
                await BaseUtil.IO.WriteFileAsync(filePath, bytes);
            }
            else await BaseUtil.IO.WriteTextFileAsync(filePath, jsonData);
        }

        protected virtual async UniTask<TData> DeserializeDataAsync(string slotId)
        {
            var filePath = SlotIdToFilePath(slotId);
            var jsonData = default(string);

            if (Binary)
            {
                var bytes = await BaseUtil.IO.ReadFileAsync(filePath);
                jsonData = await BaseUtil.String.UnzipStringAsync(bytes);
            }
            else jsonData = await BaseUtil.IO.ReadTextFileAsync(filePath);

            TData data = OSManager.GetConfiguration<StateConfiguration>().UseNewtonsoftJson ? JsonConvert.DeserializeObject<TData>(jsonData, new JsonSerializerSettings
            {
                // ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto
            }) : JsonUtility.FromJson<TData>(jsonData);

            return data;
        }

        protected virtual string GetGameDataPath()
        {
#if UNITY_EDITOR
            return Application.dataPath;
#else
                return Application.persistentDataPath;
#endif
        }
    }
}
