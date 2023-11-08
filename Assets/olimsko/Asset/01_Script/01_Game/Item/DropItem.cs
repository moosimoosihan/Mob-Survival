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

    private bool m_IsCanUse = false;

    private ItemTableSO ItemTableSO => OSManager.GetService<DataManager>().GetData<ItemTableSO>();
    private ItemContext ItemContext => OSManager.GetService<ContextManager>().GetContext<ItemContext>();

    public bool IsCanUse => m_IsCanUse;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public async void SetData(ItemSpawner spawner, DropItemData dropItemData)
    {
        m_IsCanUse = false;
        m_ItemSpawner = spawner;
        m_ItemTable = ItemTableSO.ItemTable[dropItemData.ItemID];
        m_SpriteRenderer.sprite = await m_ItemTable.GetItemSprite();
        this.transform.position = m_ItemTable.Type == ItemType.Drop ? dropItemData.Transform.position : GetRandomWithoutPlayer();
        this.gameObject.SetActive(true);
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

        m_IsCanUse = true;

    }

    public void DoMag(Transform transform)
    {
        m_IsCanUse = false;
        this.transform.DOMove(transform.position, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (this.gameObject.activeSelf == false)
        {
            return;
        }
        if (other.gameObject == GameManager.instance.playerControl.mainCharacter && other.CompareTag("Player"))
        {
            ItemContext.GetItem(m_ItemTable.Idx);

            m_ItemSpawner.ReturnDropItem(this);
        }
    }

    private Vector3 GetRandomWithoutPlayer()
    {
        Vector3 viewportCenter = new Vector3(0.5f, 0.5f, 0);
        Vector3 screenCenter = Camera.main.ViewportToWorldPoint(viewportCenter);
        float screenWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        float screenHeight = Camera.main.orthographicSize * 2;
        float minDistance = 10f;
        Vector3 randomPosition;

        do
        {
            float randomX = UnityEngine.Random.Range(screenCenter.x - screenWidth / 2, screenCenter.x + screenWidth / 2);
            float randomY = UnityEngine.Random.Range(screenCenter.y - screenHeight / 2, screenCenter.y + screenHeight / 2);
            randomPosition = new Vector3(randomX, randomY, 0);
        } while (Vector3.Distance(randomPosition, screenCenter) < minDistance);
        return randomPosition;
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
