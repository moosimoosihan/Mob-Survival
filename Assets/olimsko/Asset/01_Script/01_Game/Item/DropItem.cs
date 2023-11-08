using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;
using DG.Tweening;

public class DropItem : MonoBehaviour
{
    [SerializeField] private float m_AnimTime = 1f;
    [SerializeField] private float m_AnimRange = 5;
    private Sequence m_Sequence;
    private SpriteRenderer m_SpriteRenderer;
    private ItemTable m_ItemTable;
    private ItemSpawner m_ItemSpawner;

    private bool m_IsUsed = false;

    private ItemTableSO ItemTableSO => OSManager.GetService<DataManager>().GetData<ItemTableSO>();

    public bool IsUsed => m_IsUsed;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public async void SetData(ItemSpawner spawner, DropItemData dropItemData)
    {
        m_IsUsed = false;
        m_ItemSpawner = spawner;
        m_ItemTable = ItemTableSO.ItemTable[dropItemData.ItemID];
        m_SpriteRenderer.sprite = await m_ItemTable.GetItemSprite();
        this.transform.position = dropItemData.Transform.position;

        DoAnimation();
    }

    public void DoAnimation()
    {

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(Random.Range(-m_AnimRange, m_AnimRange), Random.Range(-m_AnimRange * 0.5f, m_AnimRange * 0.5f), 0);
        Vector3 peakPos = (startPos + endPos) / 2 + m_AnimRange * 0.5f * Vector3.up;

        m_Sequence = DOTween.Sequence();

        m_Sequence.Append(transform.DOMoveX(peakPos.x, m_AnimTime / 2).SetEase(Ease.InQuad));
        m_Sequence.Join(transform.DOMoveY(peakPos.y, m_AnimTime / 2).SetEase(Ease.OutQuad));
        m_Sequence.Append(transform.DOMoveX(endPos.x, m_AnimTime / 2).SetEase(Ease.OutQuad));
        m_Sequence.Join(transform.DOMoveY(endPos.y, m_AnimTime / 2).SetEase(Ease.InQuad));
    }

    public void DoMag(Transform transform)
    {
        m_IsUsed = true;
        this.transform.DOMove(transform.position, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == GameManager.instance.playerControl.mainCharacter && other.CompareTag("Player"))
        {

            switch (m_ItemTable.Idx)
            {
                case 53:
                    GameManager.instance.GetExp((int)m_ItemTable.Value);
                    AudioManager.Instance.ItemSfxPlay(AudioManager.ItemSfx.Exp);
                    break;
                case 52:
                    GameManager.instance.GetGold((int)m_ItemTable.Value);
                    AudioManager.Instance.ItemSfxPlay(AudioManager.ItemSfx.Coin);
                    break;
            }

            m_ItemSpawner.ReturnDropItem(this);
        }
    }
}

public class DropItemData
{
    private int m_ItemID;
    private Transform m_Transform;

    public DropItemData(int itemID, Transform transform)
    {
        m_ItemID = itemID;
        m_Transform = transform;
    }

    public int ItemID => m_ItemID;
    public Transform Transform => m_Transform;
}
