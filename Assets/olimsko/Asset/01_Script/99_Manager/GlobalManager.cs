using UnityEngine;
using olimsko;
using Cysharp.Threading.Tasks;

[InitializeAtRuntime]
public class GlobalManager : IOSMEntity, IManagedState<GlobalState>
{
    private StateManager StateManager => OSManager.GetService<StateManager>();

    private PlayerInventory m_PlayerInventory;

    public PlayerInventory PlayerInventory { get => m_PlayerInventory; set => m_PlayerInventory = value; }

    public UniTask InitializeAsync()
    {
        return UniTask.CompletedTask;
    }

    public UniTask LoadDataStateAsync(GlobalState state)
    {
        PlayerInventory = state.GetState<PlayerInventory>();

        return UniTask.CompletedTask;
    }

    public void SaveDataState(GlobalState state)
    {
        state.SetState<PlayerInventory>(m_PlayerInventory);
    }

    public async UniTask SaveData()
    {
        await StateManager.SaveGlobalAsync();
    }

    public void Reset()
    {

    }

    public void DestroyThis()
    {

    }

    public UniTask InitializeStateAsync()
    {
        return UniTask.CompletedTask;
    }
}