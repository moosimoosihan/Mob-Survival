using UnityEngine;

public abstract class ActiveSkill : MonoBehaviour
{
    // 사거리가 있는 스킬의 경우
    // 왼쪽클릭시 시전 사거리 표시
    // 마우스를 따라 사거리 이동
    // 왼쪽클릭시 시전

    // 즉시 시전의 경우
    // 왼쪽클릭시 시전
    public bool isActive;
    public bool areaOn;
    public GameObject skillArea;
    public float delay;
    public Player player;
    public float timer;
    [SerializeField]
    protected GameObject projectilePrefab;

    void Start()
    {
        timer = delay;
        ActiveSkillInit();
    }
    void Update()
    {
        if(isActive && timer < delay){
            timer += Time.deltaTime;
        }
        ActiveSkillUpdate();
    }
    public abstract void ActiveSkillUpdate();
    public abstract void ActiveSkillInit();
}
