public class WarriorFirePassive : Weapon
{
    // 베기와 용사의 일격에 피격당한 적에게 화염 데미지 적용!
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Update(){
        if(gameObject.activeSelf){
            if(!player.GetComponentInChildren<MeleeWeapon>().warriorFire){
                player.GetComponentInChildren<MeleeWeapon>().warriorFire = true;
                player.GetComponentInChildren<MeleeWeapon>().warriorFireDamge = damage;
                player.GetComponentInChildren<MeleeWeapon>().warriorFireTime = curDelay;
            }

            if(!player.GetComponentInChildren<WarriorActiveSkill>().warriorFire){
                player.GetComponentInChildren<WarriorActiveSkill>().warriorFire = true;
                player.GetComponentInChildren<WarriorActiveSkill>().warriorFireDamge = damage;
                player.GetComponentInChildren<WarriorActiveSkill>().warriorFireTime = curDelay;
            }
        }
    }
    protected override void Fire()
    {
        
    }
    private void OnDisable() {
        if(player.GetComponentInChildren<MeleeWeapon>(true).warriorFire){
            player.GetComponentInChildren<MeleeWeapon>(true).warriorFire = false;
        }

        if(player.GetComponentInChildren<WarriorActiveSkill>(true).warriorFire){
            player.GetComponentInChildren<WarriorActiveSkill>(true).warriorFire = false;
        }
    }
}
