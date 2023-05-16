using UnityEngine;

public class MineBullet : EffectBullet
{
    [SerializeField]
    GameObject mineObj;
    [SerializeField]
    GameObject explosionEffect;

    [SerializeField]
    string targetLayerMaskName = "Enemy";
    [SerializeField]
    float detectRadius = 0;

    bool isExplode = false;

    private void OnEnable()
    {
        mineObj.SetActive(true);
        explosionEffect.SetActive(false);
    }

    void Update()
    {
        if (isExplode == false)
        {
            //근처에 적을 탐지 했을때
            Collider2D[] col2D = Physics2D.OverlapCircleAll(transform.position, detectRadius, 1 << LayerMask.NameToLayer(targetLayerMaskName));
            if (col2D.Length > 0)
            {
                isExplode = true;
                explosionEffect.SetActive(true);
                mineObj.SetActive(false);
                Fire(damage, -1, Vector3.zero, knockBackPower, false);
                DeActivate(duration);
            }
        }        
    }


    public override void DeActivate(float _inTime)
    {
        StartCoroutine(CoDelayStarter(() =>
        {
            isExplode = false;
            gameObject.SetActive(false);
            detectedEnemyList.Clear();  //비활성화시 이전에 데미지 줬던 녀석들 리스트에서 해제
        },
        _inTime
        ));
    }

}
