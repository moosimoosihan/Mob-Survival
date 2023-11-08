using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using olimsko;
using DG.Tweening;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private DropItem m_DropItemPrefab;

    private ItemContext ItemContext => OSManager.GetService<ContextManager>().GetContext<ItemContext>();

    private List<DropItem> m_ListDropItem = new List<DropItem>();
    private Stack<DropItem> m_StackDropItem = new Stack<DropItem>();

    private void Awake()
    {
        ItemContext.OnCreateNewDropItem += CreateNewDropItem;
        ItemContext.OnConsumeItemUsed += OnConsumeItemUsed;
    }

    private void OnDestroy()
    {
        ItemContext.OnCreateNewDropItem -= CreateNewDropItem;
        ItemContext.OnConsumeItemUsed -= OnConsumeItemUsed;
    }

    public void CreateNewDropItem(DropItemData dropItemData)
    {
        DropItem dropItem = GetDropItem();
        dropItem.transform.position = dropItemData.Transform.position;
        dropItem.SetData(this, dropItemData);
    }

    public DropItem GetDropItem()
    {
        if (m_StackDropItem.Count > 0)
        {
            DropItem dropItem = m_StackDropItem.Pop();
            return dropItem;
        }
        else
        {
            DropItem dropItem = Instantiate(m_DropItemPrefab, transform);
            dropItem.gameObject.SetActive(false);
            m_ListDropItem.Add(dropItem);
            return dropItem;
        }
    }

    public void ReturnDropItem(DropItem dropItem)
    {
        dropItem.gameObject.SetActive(false);
        m_StackDropItem.Push(dropItem);
        m_ListDropItem.Remove(dropItem);
    }

    public void OnConsumeItemUsed(int itemID)
    {
        if (itemID == 43)
        {
            for (int i = 0; i < m_ListDropItem.Count; i++)
            {
                m_ListDropItem[i].DoMag(GameManager.instance.playerControl.mainCharacter.transform);
            }
        }
    }



}
