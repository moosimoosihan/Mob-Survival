using UnityEngine;
using olimsko;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

[InitializeAtRuntime]
public class GlobalManager : IOSMEntity, IManagedState<GlobalState>
{
    private StateManager StateManager => OSManager.GetService<StateManager>();
    private UIManager UIManager => OSManager.GetService<UIManager>();

    private PlayerInventory m_PlayerInventory;
    private CharacterInfo m_CharacterInfo;
    private Dictionary<int, Dictionary<StatType, StatUpgradeTable>> m_DictionaryCharacterTable = new Dictionary<int, Dictionary<StatType, StatUpgradeTable>>();

    public PlayerInventory PlayerInventory { get => m_PlayerInventory; set => m_PlayerInventory = value; }
    public CharacterInfo CharacterInfo { get => m_CharacterInfo; set => m_CharacterInfo = value; }


    public UniTask InitializeAsync()
    {
        return UniTask.CompletedTask;
    }

    public UniTask LoadDataStateAsync(GlobalState state)
    {
        PlayerInventory = state.GetState<PlayerInventory>();
        CharacterInfo = state.GetState<CharacterInfo>();

        return UniTask.CompletedTask;
    }

    public void SaveDataState(GlobalState state)
    {
        state.SetState<PlayerInventory>(PlayerInventory);
        state.SetState<CharacterInfo>(CharacterInfo);
    }

    public async UniTask SaveData()
    {
        UIManager.GetUI<UIIndicatorView>().Show();
        await StateManager.SaveGlobalAsync();
        UIManager.GetUI<UIIndicatorView>().Hide();
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