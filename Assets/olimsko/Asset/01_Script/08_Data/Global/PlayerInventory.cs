using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayerInventory
{
    [JsonIgnore] public Action<int> OnGoldChanged;
    [JsonIgnore] public Action<int, int> OnUpgradeStoneChanged;

    private int m_Gold = 0;
    private int[] m_ArrayUpgradeStone = new int[12];

    [JsonIgnore] private Sprite[] m_ArrayUpgradeStoneSprite = new Sprite[12];

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
    public int[] UpgradeStone { get => m_ArrayUpgradeStone; set => m_ArrayUpgradeStone = value; }
    public void SetUpgradeStone(int index, int value)
    {
        if (index < 0 || index >= m_ArrayUpgradeStone.Length)
            return;

        m_ArrayUpgradeStone[index] = value;
        OnUpgradeStoneChanged?.Invoke(index, m_ArrayUpgradeStone[index]);
    }

    public void AddUpgradeStone(int index, int value)
    {
        if (index < 0 || index >= m_ArrayUpgradeStone.Length)
            return;

        m_ArrayUpgradeStone[index] += value;
        OnUpgradeStoneChanged?.Invoke(index, m_ArrayUpgradeStone[index]);
    }

    public bool RemoveUpgradeStone(int index, int value)
    {
        if (index < 0 || index >= m_ArrayUpgradeStone.Length)
            return false;

        if (m_ArrayUpgradeStone[index] < value)
        {
            return false;
        }
        m_ArrayUpgradeStone[index] -= value;
        OnUpgradeStoneChanged?.Invoke(index, m_ArrayUpgradeStone[index]);
        return true;
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