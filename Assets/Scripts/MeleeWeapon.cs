using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField]
    string targetLayerMaskName = "Enemy";
    [SerializeField]
    float detectRadius = 0;
    [SerializeField]
    float spawnDistance = 0;

    [SerializeField]
    public float detectionAngle = 0;
    public float curDetectionAngle = 0;

    List<float> distanceList = new List<float>();

    private void Awake() {
        curDetectionAngle = detectionAngle;
    }
    public override void InitWeapon()
    {

    }

    public override void UpdateWeapon()
    {
        timer += Time.deltaTime;
        if (timer > delay)
        {
            //근처에 적을 탐지 했을때
            Collider2D[] col2D = Physics2D.OverlapCircleAll(transform.position, detectRadius, 1 << LayerMask.NameToLayer(targetLayerMaskName));

            if (distanceList.Count > 0)
                distanceList.Clear();

            //제일 가까운 녀석 찾기
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

                //제일 가까운 녀석 넣어주기
                int closestIndex = distanceList.FindIndex(dist => dist == minDist);
                closestCol2D = col2D[closestIndex];

                if (closestCol2D == null)
                    return;

                Fire(closestCol2D.transform);
                timer = 0f;
            }
        }
    }

    void Fire(Transform _targetTransform)
    {
        Vector3 targetPos = _targetTransform.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = GameManager.instance.pool.Get(projectilePrefab).transform;

        bullet.position = transform.position + dir * spawnDistance;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Fire( DamageManager.Instance.Critical(GetComponentInParent<Player>(), damage, out bool isCritical), count, Vector3.zero, knockBackPower, duration, isCritical);
        bullet.GetComponent<EffectBullet>().detectionAngle = curDetectionAngle;
    }
}
