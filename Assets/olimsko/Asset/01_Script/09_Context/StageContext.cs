using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;
using System;

public class StageContext : ContextModel
{
    public Action OnSelectedHeroChanged;

    public int SelectedChapter { get; set; } = 1;
    public int SelectedStage { get; set; } = 1;
    public int SelectedDifficulty { get; set; } = 1;

    public List<int> ListSelectedHero { get; } = new List<int>();
    public List<int> ListSelectableHero { get; private set; } = new List<int>();

    private DataManager DataManager => OSManager.GetService<DataManager>();
    private List<CharacterTable> ListCharacterTable => OSManager.GetService<DataManager>().GetData<CharacterTableSO>().CharacterTable;

    private void Awake()
    {
        if (!OSManager.IsInitialized)
            OSManager.OnInitializeComplete += OnCompleteInitializedEngine;
        else
            Initialize();
    }

    private void OnCompleteInitializedEngine()
    {
        OSManager.OnInitializeComplete -= OnCompleteInitializedEngine;
        Initialize();
    }

    private void Initialize()
    {
        ListSelectedHero.Clear();
        ListSelectableHero.Clear();

        for (int i = 0; i < DataManager.GetData<CharacterTableSO>().CharacterTable.Count; i++)
        {
            ListSelectableHero.Add(i);
        }
    }

    public void AddSelectedHero(int index)
    {
        ListSelectedHero.Add(index);
        OnSelectedHeroChanged?.Invoke();
    }

    public void RemoveSelectedHero(int index)
    {
        if (ListSelectedHero.Contains(index))
            ListSelectedHero.Remove(index);
        OnSelectedHeroChanged?.Invoke();
    }

    public void ResetSelectedHero()
    {
        ListSelectedHero.Clear();
        OnSelectedHeroChanged?.Invoke();
    }

    public CharacterTable GetCharacterTable(int index)
    {
        return ListCharacterTable[index];
    }

}
