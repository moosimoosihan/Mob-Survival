using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;
using System;

public class PlayerContext : ContextModel
{
    public Action OnEquipedItemChanged;
    public Action OnInventoryItemChanged;

    private InventoryItemData[] m_ArrayEquipedItem = new InventoryItemData[4];
    private InventoryItemData[] m_ArrayInventoryItem = new InventoryItemData[28];

    public InventoryItemData[] EquipedItem { get => m_ArrayEquipedItem; }
    public InventoryItemData[] InventoryItem { get => m_ArrayInventoryItem; }

    public void Reset()
    {
        for (int i = 0; i < m_ArrayEquipedItem.Length; i++)
        {
            m_ArrayEquipedItem[i] = null;
        }
    }

    public void SetEquipedItem(int index, InventoryItemData itemData)
    {
        if (index < 0 || index >= m_ArrayEquipedItem.Length)
            return;

        m_ArrayEquipedItem[index] = itemData;
        OnEquipedItemChanged?.Invoke();
    }

    public void SetInventoryItem(int index, InventoryItemData itemData)
    {
        if (index < 0 || index >= m_ArrayInventoryItem.Length)
            return;

        m_ArrayInventoryItem[index] = itemData;
        OnInventoryItemChanged?.Invoke();
    }

    public void AddInventoryItem(int id)
    {
        for (int i = 0; i < m_ArrayInventoryItem.Length; i++)
        {
            if (m_ArrayInventoryItem[i] == null)
            {
                m_ArrayInventoryItem[i] = new InventoryItemData(id);
                OnInventoryItemChanged?.Invoke();
                return;
            }
        }
    }

    public void RemoveInventoryItem(int index)
    {
        if (index < 0 || index >= m_ArrayInventoryItem.Length)
            return;

        m_ArrayInventoryItem[index] = null;
        OnInventoryItemChanged?.Invoke();
    }



}
