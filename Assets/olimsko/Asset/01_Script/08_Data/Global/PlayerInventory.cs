using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayerInventory
{
    public Action<int> OnGoldChanged;
    public Action<int, int> OnUpgradeStoneChanged;

    private int m_Gold = 0;
    private int[] m_ArrayUpgradeStone = new int[12];

    private Sprite[] m_ArrayUpgradeStoneSprite = new Sprite[12];

    public PlayerInventory()
    {
        Gold = 0;
        m_ArrayUpgradeStone = new int[12];
        m_ArrayUpgradeStoneSprite = new Sprite[12];
    }

    public int Gold
    {
        get => m_Gold; set
        {
            m_Gold = value;
            if (m_Gold < 0)
                m_Gold = 0;
            else if (m_Gold > 999999999)
                m_Gold = 999999999;
            OnGoldChanged?.Invoke(m_Gold);
        }
    }
    public int[] UpgradeStone { get => m_ArrayUpgradeStone; }
    public void SetUpgradeStone(int index, int value)
    {
        if (index < 0 || index >= m_ArrayUpgradeStone.Length)
            return;

        m_ArrayUpgradeStone[index] = value;
        OnUpgradeStoneChanged?.Invoke(index, value);
    }

    public void AddUpgradeStone(int index, int value)
    {
        if (index < 0 || index >= m_ArrayUpgradeStone.Length)
            return;

        m_ArrayUpgradeStone[index] += value;
        OnUpgradeStoneChanged?.Invoke(index, value);
    }

    public void RemoveUpgradeStone(int index, int value)
    {
        if (index < 0 || index >= m_ArrayUpgradeStone.Length)
            return;

        m_ArrayUpgradeStone[index] -= value;
        OnUpgradeStoneChanged?.Invoke(index, value);
    }

    public async UniTask<Sprite> GetUpgradeStoneSprite(int index)
    {
        if (index < 0 || index >= m_ArrayUpgradeStone.Length)
            return null;
        if (m_ArrayUpgradeStoneSprite[index] == null)
            m_ArrayUpgradeStoneSprite[index] = await Addressables.LoadAssetAsync<Sprite>($"UpgradeStone/{index}.png");

        return m_ArrayUpgradeStoneSprite[index];
    }
}