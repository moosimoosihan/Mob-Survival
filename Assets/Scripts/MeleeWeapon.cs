using System;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField]
    public string targetLayerMaskName = "Enemy";
    [SerializeField]
    private float detectRadius = 0;
    private float curDetectRadius = 0;
    [SerializeField]
    public float spawnDistance = 0;

    [SerializeField]
    private float detectionAngle = 0;
    private float curDetectionAngle = 0;

    public bool warriorFire;
    
    public float warriorFireDamage;
    public float curWarriorFireDamage;
    public float warriorFireTime;
    public float curWarriorFireTime;

    public List<float> distanceList = new List<float>();

    public float DetectRadius
    {
        get
        {
            return detectRadius;
        }
        set
        {
            detectRadius = value;
        }
    }
    public float DetectionAngle
    {
        get
        {
            return detectionAngle;
        }
        set
        {
            detectionAngle = value;
        }
    }
    public float CurDetectionAngle
    {
        get
        {
            return curDetectionAngle;
        }
        set
        {
            curDetectionAngle = value;
        }
    }
    public float CurDetectRadius
    {
        get
        {
            return curDetectRadius;
        }
        set
        {
            curDetectRadius = value;
        }
    }
 
    protected override void Awake()
    {
        base.Awake();
        CurDetectionAngle = detectionAngle;
    }

    protected override void Update()
    {
        timer += Time.deltaTime;
        if (timer > CurDelay)
        {
            Collider2D[] col2D = Physics2D.OverlapCircleAll(transform.position, detectRadius, 1 << LayerMask.NameToLayer(targetLayerMaskName));

            if (distanceList.Count > 0)
                distanceList.Clear();

            foreach (var item in col2D)
            {
                distanceList.Add(Vector3.Distance(item.transform.position, transform.position));
            }

            if (distanceList.Count > 0)
            {
                Collider2D closestCol2D = null;

                float minDist = distanceList[0];
                for (int i = 0; i < distanceList.Count; i++)
                {
                    if (distanceList[i] < minDist)
                        minDist = distanceList[i];
                }

                int closestIndex = distanceList.FindIndex(dist => dist == minDist);
                closestCol2D = col2D[closestIndex];

                if (closestCol2D == null)
                    return;

                OnFire(closestCol2D.transform);
                timer = 0f;
            }
        }
    }
    protected override void Fire()
    {
        
    }
    protected virtual void OnFire(Transform _targetTransform)
    {
        Vector3 targetPos = _targetTransform.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = poolBullet.Get().transform;

        bullet.parent = GameManager.instance.pool.transform;
        bullet.position = transform.position + dir * spawnDistance;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(), Damage, out bool isCritical), count, Vector3.zero, knockBackPower, duration, isCritical);
        bullet.GetComponent<EffectBullet>().DetectionAngle = curDetectionAngle;
    }
}
