using System;
using UnityEngine;

public class PlayerInventory
{
    public Action<int> OnGoldChanged;
    public Action<int> OnUpgradeStoneChanged;

    private int m_Gold = 0;
    private int m_UpgradeStone = 0;

    public PlayerInventory()
    {
        Gold = 0;
        UpgradeStone = 0;
    }

    public PlayerInventory(int gold, int upgradeStone)
    {
        Gold = gold;
        UpgradeStone = upgradeStone;
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
    public int UpgradeStone { get => m_UpgradeStone; set => m_UpgradeStone = value; }
}