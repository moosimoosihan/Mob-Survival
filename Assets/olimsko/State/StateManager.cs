using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace olimsko
{
    [InitializeAtRuntime(int.MinValue)]
    public class StateManager : IOSMEntity<StateConfiguration>
    {
        public event Action<GameStateArgs> OnGameLoadStarted;
        public event Action<GameStateArgs> OnGameLoadFinished;
        public event Action<GameStateArgs> OnGameSaveStarted;
        public event Action<GameStateArgs> OnGameSaveFinished;

        public virtual SettingState SettingState { get; private set; }
        public virtual GlobalState GlobalState { get; private set; }
        public virtual GameState GameState { get; private set; }

        public virtual ISaveStateManager<SettingState> SettingSlotManager { get; set; }
        public virtual ISaveStateManager<GlobalState> GlobalSlotManager { get; set; }
        public virtual ISaveStateManager<GameState> GameSlotManager { get; set; }

        public virtual bool QuickLoadAvailable => GameSlotManager.SaveSlotExists(LastQuickSaveSlotId);
        public virtual bool AnyGameSaveExists => GameSlotManager.AnySaveExists();
        protected virtual string LastQuickSaveSlotId => Configuration.IndexToQuickSaveSlotId(1);

        private readonly List<Action<GameState>> onGameSerializeTasks = new List<Action<GameState>>();
        private readonly List<Func<GameState, UniTask>> onGameDeserializeTasks = new List<Func<GameState, UniTask>>();

        public virtual void AddOnGameSerializeTask(Action<GameState> task) => onGameSerializeTasks.Insert(0, task);

        public virtual void RemoveOnGameSerializeTask(Action<GameState> task) => onGameSerializeTasks.Remove(task);

        public virtual void AddOnGameDeserializeTask(Func<GameState, UniTask> task) => onGameDeserializeTasks.Insert(0, task);

        public virtual void RemoveOnGameDeserializeTask(Func<GameState, UniTask> task) => onGameDeserializeTasks.Remove(task);

        public StateConfiguration Configuration { get; }

        public StateManager(StateConfiguration configuration)
        {
            Configuration = configuration;

            var savesFolderPath = BaseUtil.CombinePath("olimskoData", configuration.SaveFolderName);
            GameSlotManager = (ISaveStateManager<GameState>)Activator.CreateInstance(typeof(GameStateHandler), configuration, savesFolderPath);
            GlobalSlotManager = (ISaveStateManager<GlobalState>)Activator.CreateInstance(typeof(GlobalStateHandler), configuration, savesFolderPath);
            SettingSlotManager = (ISaveStateManager<SettingState>)Activator.CreateInstance(typeof(SettingStateHandler), configuration, savesFolderPath);

            OSManager.AddPostInitializationTask(PerformPostEngineInitializationTasks);
        }

        public UniTask InitializeAsync()
        {

            return UniTask.CompletedTask;
        }

        public virtual async UniTask<GameState> SaveGameAsync(string slotId)
        {
            var quick = slotId.StartsWithFast(Configuration.QuickSaveSlotFormat.GetBefore("{"));
#if UNITY_EDITOR
            Stopwatch watch = new Stopwatch();
            watch.Start();
            if (Configuration.UseDebugLog)
            {
                UnityEngine.Debug.Log($"Saving Game to slot {slotId} (quick: {quick})");
            }
#endif
            OnGameSaveStarted?.Invoke(new GameStateArgs(slotId, quick));

            var state = new GameState();

            await DoSaveAfterSync();

            OnGameSaveFinished?.Invoke(new GameStateArgs(slotId, quick));

#if UNITY_EDITOR
            watch.Stop();

            if (Configuration.UseDebugLog)
            {
                UnityEngine.Debug.Log($"Game Saved to slot {slotId} (quick: {quick}) took {watch.ElapsedMilliseconds}ms");
            }
#endif

            return state;

            async UniTask DoSaveAfterSync()
            {
                state.SaveDateTime = DateTime.Now;
                state.Thumbnail = OSManager.GetService<CameraManager>().CaptureThumbnail();

                SaveAllServicesToState<IManagedState<GameState>, GameState>(state);
                PerformOnGameSerializeTasks(state);

                await GameSlotManager.SaveAsync(slotId, state);

                await SaveGlobalAsync();
            }
        }

        public virtual async UniTask<GameState> QuickSaveAsync()
        {
            for (int i = Configuration.MaxQuickSaveSlotLimit; i > 0; i--)
            {
                var curSlotId = Configuration.IndexToQuickSaveSlotId(i);
                var prevSlotId = Configuration.IndexToQuickSaveSlotId(i + 1);
                GameSlotManager.RenameSaveSlot(curSlotId, prevSlotId);
            }

            var outOfLimitSlotId = Configuration.IndexToQuickSaveSlotId(Configuration.MaxQuickSaveSlotLimit + 1);
            if (GameSlotManager.SaveSlotExists(outOfLimitSlotId))
                GameSlotManager.DeleteSaveSlot(outOfLimitSlotId);

            var firstSlotId = string.Format(Configuration.QuickSaveSlotFormat, 1);
            return await SaveGameAsync(firstSlotId);
        }

        public virtual async UniTask<GameState> LoadGameAsync(string slotId)
        {
            if (string.IsNullOrEmpty(slotId) || !GameSlotManager.SaveSlotExists(slotId))
                throw new Exception($"Slot '{slotId}' not found when loading '{typeof(GameState)}' data.");

            var quick = slotId.EqualsFast(LastQuickSaveSlotId);

#if UNITY_EDITOR
            Stopwatch watch = new Stopwatch();
            watch.Start();
            if (Configuration.UseDebugLog)
            {
                UnityEngine.Debug.Log($"Load Game to slot {slotId} (quick: {quick})");
            }
#endif

            OnGameLoadStarted?.Invoke(new GameStateArgs(slotId, quick));

            await Resources.UnloadUnusedAssets();

            var state = await GameSlotManager.LoadAsync(slotId);
            await LoadAllServicesFromStateAsync<IManagedState<GameState>, GameState>(state);

            await PerformOnGameDeserializeTasksAsync(state);

            OnGameLoadFinished?.Invoke(new GameStateArgs(slotId, quick));

#if UNITY_EDITOR
            watch.Stop();

            if (Configuration.UseDebugLog)
            {
                UnityEngine.Debug.Log($"Game Loaded to slot {slotId} (quick: {quick}) took {watch.ElapsedMilliseconds}ms");
            }
#endif

            return state;
        }

        public virtual async UniTask<GameState> QuickLoadAsync() => await LoadGameAsync(LastQuickSaveSlotId);

        public virtual async UniTask NewGameAsync()
        {
            await Resources.UnloadUnusedAssets();

            await InitializeAllServicesFromStateAsync<IManagedState<GameState>, GameState>();
        }

        protected virtual void PerformOnGameSerializeTasks(GameState state)
        {
            for (int i = onGameSerializeTasks.Count - 1; i >= 0; i--)
                onGameSerializeTasks[i](state);
        }

        protected virtual async UniTask PerformOnGameDeserializeTasksAsync(GameState state)
        {
            for (int i = onGameDeserializeTasks.Count - 1; i >= 0; i--)
                await onGameDeserializeTasks[i](state);
        }

        public virtual async UniTask SaveGlobalAsync()
        {
            SaveAllServicesToState<IManagedState<GlobalState>, GlobalState>(GlobalState);
            await GlobalSlotManager.SaveAsync(Configuration.GlobalSlotId, GlobalState);
        }

        public virtual async UniTask SaveSettingsAsync()
        {
            SaveAllServicesToState<IManagedState<SettingState>, SettingState>(SettingState);
            await SettingSlotManager.SaveAsync(Configuration.SettingSlotId, SettingState);
        }

        private async UniTask PerformPostEngineInitializationTasks()
        {
            await LoadSettingsAsync();
            if (!OSManager.IsInitializing) return;
            await LoadGlobalAsync();
            if (!OSManager.IsInitializing) return;

            async UniTask LoadSettingsAsync()
            {
                SettingState = await SettingSlotManager.LoadOrDefaultAsync(Configuration.SettingSlotId);
                await LoadAllServicesFromStateAsync<IManagedState<SettingState>, SettingState>(SettingState);
            }

            async UniTask LoadGlobalAsync()
            {
                GlobalState = await GlobalSlotManager.LoadOrDefaultAsync(Configuration.GlobalSlotId);
                await LoadAllServicesFromStateAsync<IManagedState<GlobalState>, GlobalState>(GlobalState);
            }
        }

        protected virtual void SaveAllServicesToState<TService, TState>(TState state)
            where TService : class, IManagedState<TState>
            where TState : StateMap, new()
        {
            foreach (var managedState in OSManager.FindAllServices<TService>())
                managedState.SaveDataState(state);
        }

        protected virtual async UniTask LoadAllServicesFromStateAsync<TService, TState>(TState state)
            where TService : class, IManagedState<TState>
            where TState : StateMap, new()
        {
            foreach (var managedState in OSManager.FindAllServices<TService>())
                await managedState.LoadDataStateAsync(state);
        }

        protected virtual async UniTask InitializeAllServicesFromStateAsync<TService, TState>()
            where TService : class, IManagedState<TState>
            where TState : StateMap, new()
        {
            foreach (var managedState in OSManager.FindAllServices<TService>())
                await managedState.InitializeStateAsync();

        }

        public void DestroyThis()
        {
            OSManager.RemovePostInitializationTask(PerformPostEngineInitializationTasks);
        }

        public void Reset()
        {

        }
    }
}
