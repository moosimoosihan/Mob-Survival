using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;

public class UIInventorySlot : MonoBehaviour
{
    [SerializeField] private UIImage m_ItemImage = null;
    private UIInventory m_UIInventory = null;
    private bool m_IsEquipedSlot = false;
    private int m_SlotIndex = 0;
    private int m_ItemId = -1;

    public bool IsEquipedSlot => m_IsEquipedSlot;
    public int SlotIndex => m_SlotIndex;
    public int ItemId => m_ItemId;

    private Color m_EmptyColor = new Color(1, 1, 1, 0);

    private List<ItemTable> ListItemTable => OSManager.GetService<DataManager>().GetData<ItemTableSO>().ItemTable;

    private void Awake()
    {
        m_ItemImage.color = m_EmptyColor;
    }

    public void Init(UIInventory inventory, bool isEquipedSlot, int slotIndex)
    {
        m_UIInventory = inventory;
        m_IsEquipedSlot = isEquipedSlot;
        m_SlotIndex = slotIndex;
    }

    public async void SetItem(int id)
    {
        if (id >= 0 && id < ListItemTable.Count)
        {
            m_ItemImage.color = Color.white;
            m_ItemImage.sprite = await ListItemTable[id].GetItemSprite();
            if (m_ItemImage.sprite != null)
            {
                m_ItemImage.preserveAspect = true;
                m_ItemId = id;
            }
        }
        else
        {
            m_ItemImage.color = m_EmptyColor;
            m_ItemImage.sprite = null;
            m_ItemId = -1;
        }
    }


}
