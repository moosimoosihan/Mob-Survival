using UnityEngine;

public class Scaner : MonoBehaviour
{
    [Header("적군 스캔 정보")]
    public float scanRange;
    public LayerMask targetLayer;
    public RaycastHit2D[] targets;
    public Transform nearestTarget;

    [Header("아이템 스캔 정보")]
    public float itemScanRange;
    public LayerMask itemLayer;
    public RaycastHit2D[] itemTargets;

    void FixedUpdate()
    {
        itemTargets = Physics2D.CircleCastAll(transform.position, itemScanRange, Vector2.zero, 0, itemLayer);
        ItemGet();
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearest();
    }
    void ItemGet()
    {
        if (gameObject == GameManager.instance.playerControl.mainCharacter)
        {
            foreach (RaycastHit2D itemTarget in itemTargets)
            {
                DropItem dropItem = itemTarget.transform.gameObject.GetComponent<DropItem>();
                if (dropItem.IsCanUse)
                {
                    dropItem.DoMag(gameObject.transform);
                }
            }
        }
    }
    Transform GetNearest()
    {
        Transform result = null;
        float diff = 100;

        foreach (RaycastHit2D target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector3.Distance(myPos, targetPos);

            if (curDiff < diff)
            {
                diff = curDiff;
                result = target.transform;
            }
        }
        return result;
    }
}
