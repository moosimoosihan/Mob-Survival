using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IToolTipData
{
    private UIInventory m_UIInventory = null;
    private RectTransform m_ItemRect;

    [SerializeField] private CanvasGroup m_ItemCG;
    [SerializeField] private UIImage m_ItemImage = null;
    [SerializeField] private bool m_IsEquipedSlot = false;
    [SerializeField] private int m_SlotIndex = 0;
    [SerializeField] private bool m_UseAutoSync = false;
    private int m_ItemId = -1;

    public bool IsEquipedSlot => m_IsEquipedSlot;
    public int SlotIndex => m_SlotIndex;
    public int ItemId => m_ItemId;

    private List<ItemTable> ListItemTable => OSManager.GetService<DataManager>().GetData<ItemTableSO>().ItemTable;
    private PlayerContext PlayerContext => OSManager.GetService<ContextManager>().GetContext<PlayerContext>();

    private void Awake()
    {
        m_UIInventory = GetComponentInParent<UIInventory>();
        m_ItemRect = m_ItemCG.GetComponent<RectTransform>();
        m_ItemImage.sprite = OSManager.GetService<UIManager>().GetTransparentImage();

        if (m_UseAutoSync)
        {
            PlayerContext.OnInventoryItemChanged += SyncEquipedItem;
        }
    }

    public async void SetItem(int id)
    {
        if (id >= 0 && id < ListItemTable.Count)
        {
            m_ItemImage.sprite = await ListItemTable[id].GetItemSprite();
            if (m_ItemImage.sprite != null)
            {
                m_ItemImage.preserveAspect = true;
                m_ItemId = id;
            }
        }
        else
        {
            m_ItemImage.sprite = OSManager.GetService<UIManager>().GetTransparentImage();
            m_ItemId = -1;
        }
    }

    public void SyncEquipedItem()
    {
        int itemID = -1;
        if (m_IsEquipedSlot)
        {
            itemID = PlayerContext.EquipedItem[m_SlotIndex] == null ? -1 : PlayerContext.EquipedItem[m_SlotIndex].Id;
        }
        else
        {
            itemID = PlayerContext.InventoryItem[m_SlotIndex] == null ? -1 : PlayerContext.InventoryItem[m_SlotIndex].Id;
        }

        SetItem(itemID);
    }

    private void OnDestroy()
    {
        PlayerContext.OnInventoryItemChanged -= SyncEquipedItem;

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_UIInventory == null || m_ItemId == -1) return;

        m_ItemRect.SetParent(m_UIInventory.transform);
        m_ItemCG.blocksRaycasts = false;
        m_ItemCG.alpha = 0.5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_UIInventory == null || m_ItemId == -1) return;

        m_ItemRect.position = eventData.position;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_UIInventory == null) return;

        m_ItemRect.SetParent(this.transform);
        m_ItemRect.localPosition = Vector3.zero;
        m_ItemCG.blocksRaycasts = true;
        m_ItemCG.alpha = 1;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (m_UIInventory == null || eventData.pointerDrag == null) return;

        UIInventorySlot prevSlot = eventData.pointerDrag.GetComponent<UIInventorySlot>();
        if (prevSlot == null) return;

        PlayerContext.ReplaceInventoryItem(prevSlot.SlotIndex, prevSlot.IsEquipedSlot, m_SlotIndex, m_IsEquipedSlot);
        prevSlot.OnEndDrag(eventData);
    }

    public string GetToolTipTitle()
    {
        if (m_ItemId != -1)
            return LocalizationSettings.StringDatabase.GetTable("LanguageTable").GetEntry(ListItemTable[m_ItemId].Name).GetLocalizedString();
        else return "";
    }

    public string GetToolTipDesc()
    {
        if (m_ItemId != -1)
            return LocalizationSettings.StringDatabase.GetTable("LanguageTable").GetEntry(ListItemTable[m_ItemId].Desc).GetLocalizedString();
        else return "";
    }

    public bool IsCanShowToolTip()
    {
        return m_ItemId != -1;
    }

    public RectTransform GetRectTransform()
    {
        return m_ItemRect;
    }
}
