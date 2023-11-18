using System.Collections.Generic;
using UnityEngine;

public class EffectBullet : Bullet
{
    [SerializeField]
    private float attackRadius;
    [SerializeField]
    private float detectionAngle;

    Vector3 dir = Vector3.right;
    protected List<Enemy> enemyList = new List<Enemy>();

    [SerializeField]
    bool drawRay = false;

    public float AttackRadius
    {
        get
        {
            return attackRadius;
        }
        set
        {
            attackRadius = value;
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

    private void OnDrawGizmosSelected()
    {
        if (drawRay)
        {
            Debug.DrawRay(transform.position, EulerAngleToVector(DetectionAngle * 0.5f) * attackRadius);
            Debug.DrawRay(transform.position, EulerAngleToVector(-DetectionAngle * 0.5f) * attackRadius);
        }
    }

    Vector3 EulerAngleToVector(float _angle)
    {
        float tempAngle = _angle + transform.eulerAngles.y;
        float tempRadian = tempAngle * Mathf.Deg2Rad;
        Vector3 result = new Vector3(Mathf.Sin(tempRadian), Mathf.Cos(tempRadian), 0);
        return result;
    }

    public virtual List<Enemy> GetEnemies()
    {
        List<Enemy> result = new List<Enemy>();

        //�� ���� ������Ʈ���� �������� ���� physics ���
        Collider2D[] colliders2D = Physics2D.OverlapCircleAll(transform.position, AttackRadius, LayerMask.GetMask("Enemy"));

        if (colliders2D.Length > 0)
        {
            foreach (var targetCol in colliders2D)
            {
                Vector3 direction = targetCol.transform.position - transform.position;
                float angle = Vector3.Angle(transform.up, direction);

                // �ء�� ��ä�� ���� �ȿ� �ִ� �ֵ鸸 ��������   
                if (angle <= detectionAngle * 0.5f)
                {
                    //0 ~ 180�� -0 - 180
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
            if(!enemyList[i].gameObject.activeSelf)
                return;

            enemyList[i].GetDamage(damage, knockBackPower, isCritical, player);
        }

        if(_deActivate)
        {
            DeActivate(_duration);
        }
    }

    public override void OnTriggerEnter2DUpdate(Collider2D collision)
    {
        //�ݶ��̴� ��� ����
    }
    private void Update() {
        OnDrawGizmosSelected();
    }
}
