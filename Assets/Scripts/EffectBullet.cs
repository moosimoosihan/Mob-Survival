using System.Collections.Generic;
using UnityEngine;

public class EffectBullet : Bullet
{
    [SerializeField]
    float attackRadius = 0;
    [SerializeField]
    public float detectionAngle = 0;

    Vector3 dir = Vector3.right;
    protected List<Enemy> enemyList = new List<Enemy>();

    [SerializeField]
    bool drawRay = false;

    private void OnDrawGizmosSelected()
    {
        if (drawRay)
        {
            Debug.DrawRay(transform.position, EulerAngleToVector(detectionAngle * 0.5f) * attackRadius);
            Debug.DrawRay(transform.position, EulerAngleToVector(-detectionAngle * 0.5f) * attackRadius);
        }
    }

    Vector3 EulerAngleToVector(float _angle)
    {
        float tempAngle = _angle + transform.eulerAngles.y;
        float tempRadian = tempAngle * Mathf.Deg2Rad;
        Vector3 result = new Vector3(Mathf.Sin(tempRadian), Mathf.Cos(tempRadian), 0);
        return result;
    }

    protected List<Enemy> GetEnemies()
    {
        List<Enemy> result = new List<Enemy>();

        //원 안의 오브젝트들을 가져오기 위해 physics 사용
        Collider2D[] colliders2D = Physics2D.OverlapCircleAll(transform.position, attackRadius, LayerMask.GetMask("Enemy"));

        if (colliders2D.Length > 0)
        {
            foreach (var targetCol in colliders2D)
            {
                Vector3 direction = targetCol.transform.position - transform.position;
                float angle = Vector3.Angle(transform.up, direction);

                // ↖↑↗ 부채꼴 각도 안에 있는 애들만 가져오기   
                if (angle <= detectionAngle * 0.5f)
                {
                    //0 ~ 180도 -0 - 180
                    Enemy enemyScript = targetCol.GetComponent<Enemy>();
                    if (enemyScript != null)
                        result.Add(enemyScript);
                }
            }
        }

        return result;
    }

    public override void Fire(float _damage, int _per, Vector3 _dir,  float _knockBackPower, float _duration, bool _isCritical, bool _deActivate = true, bool _hitOnlyOnce = true)
    {
        if (enemyList.Count > 0)
            enemyList.Clear();

        enemyList = GetEnemies();
        hitOnlyOnce = _hitOnlyOnce;
        damage = _damage;
        knockBackPower = _knockBackPower;
        isCritical = _isCritical;

        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].GetDamage(damage, knockBackPower, isCritical);

            if(warriorFire){
                if(!enemyList[i].isFire){
                    StartCoroutine(enemyList[i].WarriorFireOn(warriorFireDamge, warriorFireTime));
                } else {
                    enemyList[i].FireInit(warriorFireDamge, warriorFireTime);
                }
            }
        }

        if(_deActivate)
        {
            DeActivate(_duration);
        }
    }

    public override void OnTriggerEnter2DUpdate(Collider2D collision)
    {
        //콜라이더 사용 안함
    }
}
