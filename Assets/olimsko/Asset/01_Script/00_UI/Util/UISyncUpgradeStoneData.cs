using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;
using TMPro;
using DG.Tweening;

public class UISyncUpgradeStoneData : MonoBehaviour
{
    [SerializeField] private int m_UpgradeStoneIndex = -1;
    [SerializeField] private bool m_UseAnimation = true;
    [SerializeField] private float m_AnimationTime = 0.5f;

    [SerializeField] private TextMeshProUGUI m_UpgradeStoneCountText;
    [SerializeField] private UIImage m_UpgradeStoneImage;

    private GlobalManager GlobalManager => OSManager.GetService<GlobalManager>();

    private void Awake()
    {
        if (m_UpgradeStoneImage == null) m_UpgradeStoneImage = GetComponentInChildren<UIImage>();
        if (m_UpgradeStoneCountText == null) m_UpgradeStoneCountText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private async void Start()
    {
        GlobalManager.PlayerInventory.OnUpgradeStoneChanged += OnUpgradeStoneChanged;

        if (m_UpgradeStoneImage != null)
        {
            m_UpgradeStoneImage.sprite = await GlobalManager.PlayerInventory.GetUpgradeStoneSprite(m_UpgradeStoneIndex);
        }
        OnUpgradeStoneChanged(m_UpgradeStoneIndex, GlobalManager.PlayerInventory.UpgradeStone[m_UpgradeStoneIndex]);
    }

    private void OnDestroy()
    {
        GlobalManager.PlayerInventory.OnUpgradeStoneChanged -= OnUpgradeStoneChanged;
    }

    private void OnUpgradeStoneChanged(int index, int count)
    {
        if (index != m_UpgradeStoneIndex)
            return;
        if (m_UseAnimation)
        {
            m_UpgradeStoneCountText.DOCounter(int.Parse(m_UpgradeStoneCountText.text.Replace(",", "")), count, m_AnimationTime);
            return;
        }
        else
        {
            m_UpgradeStoneCountText.text = count.ToString();
        }
    }
}
