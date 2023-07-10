using UnityEngine;
using olimsko;

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

        OSManager.GetService<InputManager>().GetAction("ReadyActiveSkill").Enable();
        OSManager.GetService<InputManager>().GetAction("ConfirmActiveSkill").Enable();
        OSManager.GetService<InputManager>().GetAction("CancelActiveSkill").Enable();
    }
    private void OnDestroy()
    {
        OSManager.GetService<InputManager>().GetAction("ReadyActiveSkill").Disable();
        OSManager.GetService<InputManager>().GetAction("ConfirmActiveSkill").Disable();
        OSManager.GetService<InputManager>().GetAction("CancelActiveSkill").Disable();
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
        if(!isActive && _areaOn? OSManager.GetService<InputManager>().GetAction("ConfirmActiveSkill").IsPressed() : OSManager.GetService<InputManager>().GetAction("ReadyActiveSkill").IsPressed())
        {
            return true;
        } else {
            return false;
        }
    }
}
