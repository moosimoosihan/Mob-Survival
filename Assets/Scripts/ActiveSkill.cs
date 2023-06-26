using UnityEngine;

public abstract class ActiveSkill : MonoBehaviour
{
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
        if(!player.inputEnabled || !GameManager.instance.isPlay){
            AreaOff();
            return;
        }
        if(KeyDown(areaOn)){
            ActiveSkillUpdate();
        }
        AreaUpdate();
    }
    public abstract void ActiveSkillUpdate();
    public abstract void ActiveSkillInit();
    public abstract void AreaUpdate();
    public abstract void AreaOff();
    public bool KeyDown(bool _areaOn){
        if(!isActive && _areaOn? Input.GetKeyDown(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.R)){
            return true;
        } else {
            return false;
        }
    }
}
