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

        GameManager.instance.inputManager.GetAction("ReadyActiveSkill").Enable();
        GameManager.instance.inputManager.GetAction("ConfirmActiveSkill").Enable();
        GameManager.instance.inputManager.GetAction("CancelActiveSkill").Enable();
    }
    private void OnDestroy()
    {
        GameManager.instance.inputManager.GetAction("ReadyActiveSkill").Disable();
        GameManager.instance.inputManager.GetAction("ConfirmActiveSkill").Disable();
        GameManager.instance.inputManager.GetAction("CancelActiveSkill").Disable();
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
        if(!isActive && _areaOn? GameManager.instance.inputManager.GetAction("ConfirmActiveSkill").IsPressed() : GameManager.instance.inputManager.GetAction("ReadyActiveSkill").IsPressed())
        {
            return true;
        } else {
            return false;
        }
    }
}
