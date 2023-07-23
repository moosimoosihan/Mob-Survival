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

    private GlobalManager GlobalManager => OSManager.GetService<GlobalManager>();

    public async void SetCharacterStatAsync(int charId)
    {
        LocalizationSettings.StringDatabase.GetTableAsync("LanguageTable").Completed += (table) =>
        {
            m_StatNameText.text = table.Result.GetEntry($"Str_StatNameKey_{m_StatType.ToString()}").GetLocalizedString();
        };

        StatUpgradeTable statUpgradeTable = OSManager.GetService<GlobalManager>().CharacterInfo.ListCharUpgradeStat[charId][m_StatType];

        if (m_StatType == StatType.Passive1 || m_StatType == StatType.Passive2 || m_StatType == StatType.Passive3)
        {
            m_StatCountText.gameObject.SetActive(false);
            m_StatCountImage.SetActive(true);
            for (int i = 0; i < m_ListStatCountImage.Length; i++)
            {
                m_ListStatCountImage[i].gameObject.SetActive(i < statUpgradeTable.UpgradeCount);
            }
        }
        else
        {
            m_StatCountText.gameObject.SetActive(true);
            m_StatCountImage.SetActive(false);
            m_StatCountText.text = (statUpgradeTable.UpgradeCount * statUpgradeTable.UpgradeValue).ToString();
        }

        m_StatUpgradeCostText.text = statUpgradeTable.UpgradeGold.ToString();
        m_StatUpgradeStoneText.text = statUpgradeTable.UpgradeStone.ToString();
        m_StatUpgradeStoneImage.sprite = await GlobalManager.PlayerInventory.GetUpgradeStoneSprite(statUpgradeTable.UpgradeStoneIndex);
    }
}
