using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;

public class UIPlayerInfoEnitity : UIViewEntity
{
    private int m_PlayerID;
    private int m_PlayerIdx;
    private CharacterTableSO CharacterTable => OSManager.GetService<DataManager>().GetData<CharacterTableSO>();
    private CharacterTable m_CharacterTable;
    private PlayerContext PlayerContext => OSManager.GetService<ContextManager>().GetContext<PlayerContext>();

    protected override void Awake()
    {
        base.Awake();
    }

    public async void SetPlayer(int id, int idx)
    {
        m_PlayerID = id;
        m_PlayerIdx = idx;

        PlayerContext.OnSelectedCharacterChanged -= OnSelectedCharacterChanged;
        PlayerContext.OnSelectedCharacterChanged += OnSelectedCharacterChanged;

        m_CharacterTable = CharacterTable.CharacterTable[id];
        Get<UIImage>("SDIcon").sprite = await m_CharacterTable.GetSDSprite();

        GameManager.instance.players[idx].OnHpChanged += OnHpChanged;

        OnHpChanged();
    }

    private void OnSelectedCharacterChanged()
    {
        if (m_PlayerIdx == PlayerContext.SelectedCharacterIdx)
        {
            Get<UIImage>("Selected").gameObject.SetActive(true);
        }
        else
        {
            Get<UIImage>("Selected").gameObject.SetActive(false);
        }
    }

    private void OnHpChanged()
    {
        Get<UISlider>("HPBar").maxValue = GameManager.instance.players[m_PlayerIdx].MaxHP;
        Get<UISlider>("HPBar").value = GameManager.instance.players[m_PlayerIdx].CurHP;

        Get<UITMPText>("HPText").SetText($"{GameManager.instance.players[m_PlayerIdx].CurHP}/{GameManager.instance.players[m_PlayerIdx].MaxHP}");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        PlayerContext.OnSelectedCharacterChanged -= OnSelectedCharacterChanged;
        GameManager.instance.players[m_PlayerIdx].OnHpChanged -= OnHpChanged;
    }
}
