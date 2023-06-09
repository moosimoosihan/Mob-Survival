public class WarriorFirePassive : Weapon
{
    // 베기와 용사의 일격에 피격당한 적에게 화염 데미지 적용!
    public override void InitWeapon(){
        
    }
    public override void UpdateWeapon(){
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
    private void OnDisable() {
        if(player.GetComponentInChildren<MeleeWeapon>().warriorFire){
            player.GetComponentInChildren<MeleeWeapon>().warriorFire = false;
        }

        if(player.GetComponentInChildren<WarriorActiveSkill>().warriorFire){
            player.GetComponentInChildren<WarriorActiveSkill>().warriorFire = false;
        }
    }
}
