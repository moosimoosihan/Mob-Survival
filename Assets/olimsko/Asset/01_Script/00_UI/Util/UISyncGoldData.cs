using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;
using TMPro;
using DG.Tweening;

public class UISyncGoldData : MonoBehaviour
{
    [SerializeField] private bool m_UseAnimation = true;
    [SerializeField] private float m_AnimationTime = 0.5f;

    private TextMeshProUGUI m_GoldText;
    private GlobalManager GlobalManager => OSManager.GetService<GlobalManager>();

    private void Awake()
    {
        m_GoldText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        GlobalManager.PlayerInventory.OnGoldChanged += OnGoldChanged;
        OnGoldChanged(GlobalManager.PlayerInventory.Gold);
    }

    private void OnDestroy()
    {
        GlobalManager.PlayerInventory.OnGoldChanged -= OnGoldChanged;
    }

    private void OnGoldChanged(int gold)
    {
        if (m_UseAnimation)
        {
            m_GoldText.DOCounter(int.Parse(m_GoldText.text.Replace(",", "")), gold, m_AnimationTime);
            return;
        }
        else
        {
            m_GoldText.text = gold.ToString();
        }
    }

}
