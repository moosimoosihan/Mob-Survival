using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;
using UnityEngine.Localization.Settings;
using System.Threading.Tasks;

public class UIStatUpgradeItem : MonoBehaviour
{
    [SerializeField] private StatType m_StatType;
    [SerializeField] private UITMPText m_StatNameText;
    [SerializeField] private UITMPText m_StatCountText;
    [SerializeField] private GameObject m_StatCountImage;
    [SerializeField] private UIImage[] m_ListStatCountImage;
    [SerializeField] private UIButton m_StatUpgradeButton;
    [SerializeField] private UITMPText m_StatUpgradeCostText;
    [SerializeField] private UIImage m_StatUpgradeStoneImage;
    [SerializeField] private UITMPText m_StatUpgradeStoneText;

    private int m_CharID = 0;

    private GlobalManager GlobalManager => OSManager.GetService<GlobalManager>();
    private StatUpgradeTableSO StatUpgradeTableSO => OSManager.GetService<DataManager>().GetData<StatUpgradeTableSO>();

    public async void SetCharacterStatAsync(int charId)
    {
        m_CharID = charId;
        LocalizationSettings.StringDatabase.GetTableAsync("LanguageTable").Completed += (table) =>
        {
            m_StatNameText.text = table.Result.GetEntry($"Str_StatNameKey_{m_StatType.ToString()}").GetLocalizedString();
        };

        m_StatUpgradeButton.interactable = true;

        int currentLevel = GlobalManager.CharacterInfo.DicCharUpgradedStat[charId][m_StatType];
        StatUpgradeTable statUpgradeTable = StatUpgradeTableSO.GetStatTable(charId, m_StatType, currentLevel + 1);

        if (statUpgradeTable == null)
        {
            statUpgradeTable = StatUpgradeTableSO.GetStatTable(charId, m_StatType, currentLevel);
            m_StatUpgradeButton.interactable = false;
        }

        if (m_StatType == StatType.Passive1 || m_StatType == StatType.Passive2 || m_StatType == StatType.Passive3)
        {
            m_StatCountText.gameObject.SetActive(false);
            m_StatCountImage.SetActive(true);
            for (int i = 0; i < m_ListStatCountImage.Length; i++)
            {
                m_ListStatCountImage[i].gameObject.SetActive(i < currentLevel);
            }
        }
        else
        {
            m_StatCountText.gameObject.SetActive(true);
            m_StatCountImage.SetActive(false);
            m_StatCountText.text = GetStatValueString(m_StatType, statUpgradeTable.UpgradeValue);
        }

        m_StatUpgradeCostText.text = statUpgradeTable.UpgradeGold.ToString();
        m_StatUpgradeStoneText.text = statUpgradeTable.UpgradeStoneCount == 0 ? "" : statUpgradeTable.UpgradeStoneCount.ToString();
        m_StatUpgradeStoneImage.sprite = await GlobalManager.PlayerInventory.GetUpgradeStoneSprite(statUpgradeTable.UpgradeStoneIndex);

        if (m_StatUpgradeStoneImage.sprite == null)
        {
            m_StatUpgradeStoneImage.sprite = OSManager.GetService<UIManager>().GetTransparentImage();
        }
    }

    public async void UpgradeStat()
    {
        int currentLevel = GlobalManager.CharacterInfo.DicCharUpgradedStat[m_CharID][m_StatType];
        StatUpgradeTable statUpgradeTable = StatUpgradeTableSO.GetStatTable(m_CharID, m_StatType, currentLevel + 1);

        if (statUpgradeTable == null)
        {
            return;
        }

        if (GlobalManager.PlayerInventory.Gold < statUpgradeTable.UpgradeGold)
        {
            return;
        }
        if (statUpgradeTable.UpgradeStoneCount > 0)
        {
            if (GlobalManager.PlayerInventory.RemoveUpgradeStone(statUpgradeTable.UpgradeStoneIndex, statUpgradeTable.UpgradeStoneCount) == false)
            {
                return;
            }
        }


        GlobalManager.PlayerInventory.Gold -= statUpgradeTable.UpgradeGold;

        GlobalManager.CharacterInfo.DicCharUpgradedStat[m_CharID][m_StatType] += 1;
        await GlobalManager.SaveData();
        SetCharacterStatAsync(m_CharID);
    }

    public string GetStatValueString(StatType statType, float value)
    {
        switch (statType)
        {
            case StatType.Damage: return $"{value * 10}";
            case StatType.AttSpeed: return $"{Mathf.RoundToInt(60 / value)}";
            case StatType.CritRate: return $"{value}%";
            case StatType.CritDamage: return $"{value}%";
            case StatType.ActiveDamage: return $"{value * 10}";
            case StatType.ActiveCooldown: return $"{value}";
            case StatType.HP: return $"{value * 10}";
            case StatType.Def: return $"{value * 10}";
            case StatType.MoveSpeed: return $"{value * 10}";
            default:
                return value.ToString();
        }
    }
}
