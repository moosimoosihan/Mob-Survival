using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;

public class UIInventory : UIViewEntity
{
    [SerializeField] private UIInventorySlot m_SelectedSlot = null;
    [SerializeField] private List<UIInventorySlot> m_ListEquipedSlot = new List<UIInventorySlot>();
    [SerializeField] private List<UIInventorySlot> m_ListInventorySlot = new List<UIInventorySlot>();

    PlayerContext PlayerContext => OSManager.GetService<ContextManager>().GetContext<PlayerContext>();

    public UIInventorySlot SelectedSlot { get => m_SelectedSlot; set => m_SelectedSlot = value; }

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("??b");
        for (int i = 0; i < m_ListEquipedSlot.Count; i++)
        {
            m_ListEquipedSlot[i].Init(this, true, i);
        }

        for (int i = 0; i < m_ListInventorySlot.Count; i++)
        {
            m_ListInventorySlot[i].Init(this, false, i);
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        SyncInventory();
        Debug.Log("??");
        PlayerContext.OnEquipedItemChanged -= SyncEquipedItem;
        PlayerContext.OnInventoryItemChanged -= SyncInventoryItem;
        PlayerContext.OnEquipedItemChanged += SyncEquipedItem;
        PlayerContext.OnInventoryItemChanged += SyncInventoryItem;
    }

    protected override void OnHide()
    {
        base.OnHide();

        PlayerContext.OnEquipedItemChanged -= SyncEquipedItem;
        PlayerContext.OnInventoryItemChanged -= SyncInventoryItem;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        PlayerContext.OnEquipedItemChanged -= SyncEquipedItem;
        PlayerContext.OnInventoryItemChanged -= SyncInventoryItem;
    }

    public void SetSelectedSlot(UIInventorySlot slot)
    {
        if (slot == null)
        {
            m_SelectedSlot.Init(this, false, -1);
            m_SelectedSlot.SetItem(-1);
            m_SelectedSlot.gameObject.SetActive(false);
        }
        else
        {
            SelectedSlot.Init(this, slot.IsEquipedSlot, slot.SlotIndex);
            SelectedSlot.SetItem(slot.ItemId);
            SelectedSlot.gameObject.SetActive(true);
        }
    }

    private void SyncInventory()
    {
        SyncEquipedItem();
        SyncInventoryItem();
    }

    private void SyncEquipedItem()
    {
        for (int i = 0; i < PlayerContext.EquipedItem.Length; i++)
        {
            if (PlayerContext.EquipedItem[i] != null)
            {
                m_ListEquipedSlot[i].SetItem(PlayerContext.EquipedItem[i].Id);
            }
        }
    }

    private void SyncInventoryItem()
    {
        for (int i = 0; i < PlayerContext.InventoryItem.Length; i++)
        {
            if (PlayerContext.InventoryItem[i] != null)
            {
                m_ListInventorySlot[i].SetItem(PlayerContext.InventoryItem[i].Id);
            }
        }
    }
}
