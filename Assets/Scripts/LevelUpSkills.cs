using System.Collections;
using System.Collections.Generic;
using olimsko;
using UnityEngine;
using UnityEngine.Pool;

public class LevelUpSkills : MonoBehaviour
{
    static PlayerContext playerContext = OSManager.GetService<ContextManager>().GetContext<PlayerContext>();

    // 용사 0번 스킬 숙련된 베기 하프에서 서클로 변경
    public static void WorriorSkill0(Transform transform, IObjectPool<Bullet> poolBullet, Vector3 _dir, float _scalex, float _spawnDistance, float curDetectionAngle, Player player, float Damage, int count, float knockBackPower, float duration, float scaley, float detectRadius){
        if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill.ContainsKey(0)){
            if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill[0].Level > 0){
                Transform bulletBack = poolBullet.Get().transform;
                
                Vector3 backDir = new Vector3(_dir.x * -1,_dir.y * -1, _dir.z);
                bulletBack.parent = GameManager.instance.pool.transform;
                bulletBack.transform.localScale = new Vector3(_scalex, scaley, transform.transform.localScale.z);
                bulletBack.position = transform.position + backDir * _spawnDistance;
                bulletBack.rotation = Quaternion.FromToRotation(Vector3.down, _dir);
                bulletBack.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(player, Damage, out bool isCritical), count, Vector3.zero, knockBackPower, duration, isCritical);
                bulletBack.GetComponent<EffectBullet>().DetectionAngle = curDetectionAngle;
                bulletBack.GetComponent<EffectBullet>().AttackRadius = detectRadius;
            }
        }
    }
    public static float[] WorriorSkill1(float scalex, float detectionAngle){
        float[] result = {scalex , detectionAngle};
        // 용사 1번 스킬 범위증가(베기) 베기스킬 범위 2배 증가
        if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill.ContainsKey(1)){
            if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill[1].Level > 0){
                result[0] *= 2f;
                result[1] = detectionAngle * 2f;
            }
        }
        return result;
    }

    // 용사 2번 스킬 파티버프 전체 데미지 15% 증가
    public static float WorriorSkill2(float _damage){
        float damage = _damage;
        if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill.ContainsKey(2)){
            if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill[2].Level > 0){
                if(_damage!=0)
                    damage += _damage * 0.15f;
            }
        }
        return damage;
    }
    
    // 용사 3스킬 튼튼한 갑옷 데미지 10% 감소
    public static float WorriorSkill3(float _damage){
        float dam = _damage;
        if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill.ContainsKey(3)){
            if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill[3].Level > 0){
                if(_damage!=0)
                    dam -= _damage * 0.1f;
            }
        }
        return dam;
    }

    // 용사 4스킬 화염 갑옷 화상스킬 
    // 만들어야 함
    public static void WorriorSkill4(){

    }

    // 용사 5스킬 화상 피해 증가 10마리당 0.1% 증가
    public static float WorriorSkill5(float _damage, int killCount){
        float dam = _damage;
        if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill.ContainsKey(5)){
            if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill[5].Level > 0){
                dam += killCount * 0.0001f;
            }
        }
        return dam;
    }

    // 용사 6스킬 용사의 일격 스킬 Projectile Type이 QuarterCircle 에서 HalfCircle로 변경
    // 만들어야 함
    public static void WorriorSkill6(){

    }

    // 용사 7스킬 용사의 일격 스킬의 Project Range 600으로변경
    public static float[] WorriorSkill7(float _scaley, float _detectRadius){
        float[] result = {_scaley, _detectRadius};
        if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill.ContainsKey(7)){
            if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill[7].Level > 0){
                result[0] *= 2f;
                result[1] *= 2f;
            }
        }
        return result;
    }

    // 용사 8스킬 화상 스킬의 Buff Time이 200으로 번경
    public static float WorriorSkill8(float _buffTime){
        float buffTime = _buffTime;
        if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill.ContainsKey(7)){
            if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill[7].Level > 0){
                buffTime = 200f;
            }
        }
        return buffTime;
    }

    // 용사 9스킬 Damage Reduction 10% 증가하며 파티원이 받는 데미지 50%를 용사가 대신 받음
    public static float WorriorSkill9(string character, float _damage, bool isCritical){
        float damage = _damage;
        if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill.ContainsKey(9)){
            if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill[9].Level > 0){
                // 워리어라면 10% 감소된 데미지를 받음
                if(character.Equals("용사")){
                    damage -= _damage * 0.1f;
                } else {
                    // 나머지는 워리어가 살아있다면 50%의 데미지를 워리어에게 전달
                    foreach(Player player in GameManager.instance.players){
                        if(player.character.Equals("용사") && !player.playerDead){
                            player.GetDamage(damage * 0.5f, isCritical);
                            return damage * 0.5f;
                        }
                    }
                }
            }
        }
        return damage;
    }

    // 용사 10스킬 화상 디버프가 있는 몬스터가 사망시 Range 100 이내 주변 몬스터에게 화상 디버프를 부여
    public static bool WorriorSkill10(){
        if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill.ContainsKey(10)){
            if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill[10].Level > 0){
                return true;
            }
        }
        return false;
    }

    // 용사 11스킬 용사의 일격 스킬 Projectile Type을 Half Circle에서 Circle로 변경시키고 Cooldown 20초 감소
    // (만들어야 함)
    public static void WorriorSkill11(){

    }

    // 용사 12스킬 Damage,Att Range 5% 증가 (적용 해야함)
    public static float[] WorriorSkill12(float _damage, float _detectRadius){
        float[] result = {_damage, _detectRadius};
        if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill.ContainsKey(12)){
            if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill[12].Level > 0){
                int level = playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill[12].Level;
                result[0] *= 1f + (level * 0.05f);
                result[1] *= 1f + (level * 0.05f);
            }
        }
        return result;
    }

    // 용사 13스킬 Damage Reduction 3% 증가
    public static float WorriorSkill13(float _damage){
        float dam = _damage;
        
        if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill.ContainsKey(13)){
            if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill[13].Level > 0){
                int level = playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill[13].Level;
                if(_damage!=0)
                    dam -= _damage * (0.03f*level);
            }
        }
        return dam;
    }
    
    // 용사 14스킬 화상 스킬의 Debuff Value 0.1% 증가
    public static float WorriorSkill14(float _fireDamage){
        float fireDamage = _fireDamage;
        if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill.ContainsKey(14)){
            if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill[14].Level > 0){
                int level = playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill[14].Level;
                fireDamage += 0.001f * level;
            }
        }
        return fireDamage;
    }

    // 용사 15스킬 Active Damage 15% 증가


    // 피격시 무적시간 1초, 이동속도 100%증가로 변경
    // Att Speed 50% 증가
    // 파티원 전체 Att Speed/Active Cooldown 15% 증가
    // 사격에 특수효과를 추가하여 5회 기본 공격시 Crit Rate를 100%로 변경
    // 궁수가 적 50마리 처치시 사격스킬의 투사체 관통 1증가
    // 치명타 발생시 액티브 스킬 쿨다운 1초 감소
    // 액티브의 BuffTime 20초 증가
    // 액티브가 Critical Damage 100%도 증가시킴
    // Crit Rate 20%와 Crit Damage 70% 증가
    // 30초마다 10초간 파티원 전체 신속 부여 (Att Speed 50%, Move Speed 50% 증가)
    // 투사체 관통 1증가 시키고 유도 부여 (관통시 가장 가까운 적 에게 자동 타겟 설정)
    // 액티브가 지속중인 동안 사격 스킬의 Number of Projectile 2개 더 추가함
    // Damage,Att Speed 5% 증가
    // Evasion 1% 증가
    // 패시브2의 치명타 발생시 Att Speed 1% 추가 증가
    // 액티브의 Crit Rate 추가 1% 증가


    // Damage 50% 증가하며 ProjectSize 15로 변경
    // Att Speed 50% 증가하며 ProjectileSpeed 2로 변경
    // Add Exp 25%증가
    // 모든 스킬에 특수효과로 디버프 빙결을 부여 (피격시 10초간 이동속도 10%감소 4중첩 가능)
    // 아이스 필드 스킬의 Area Size 200으로 변경
    // 아이스 필드 스킬의 Area Duration 10으로 변경
    // 블리자드의 Area Size 500으로 변경
    // 블리자드의 Area Duration 20초로 변경하고 Cooldown 60초로 변경
    // 아이스볼 Damage 30, Cooldown 5초로 변경시키며 피격시 빙결 2중첩씩 부여
    // 빙결 4중첩된 적을 피격시 2초간 이동속도 100%감소시킴
    // 아이스 필드의 Area Size를 300, Area Duration을15초로 변경
    // 블리자드의 범위가 맵 전체로 변경 지속시간 5초 감소와 CoolDown 30초 증가
    // Damage,Att Range 5% 증가
    // 아이스 필드의 Area Size 10% 증가
    // 디버프 빙결의 이동속도 감소를 1% 추가 감소
    // Active Damage 15% 증가


    // Att Speed 50% 증가하며 이단심판으로 타격시 2초간 이동속도 100% 감소
    // 이단심판으로 타격시 아군에게 적용되어 있는 디버프 1개 제거
    // 파티원 전체 Damage 15%, Defense 15% 증가
    // 홀리쉴드가 적용된 아군이 5초마다 잃은 HP의 5%를 회복
    // 홀리쉴드가 파티 전체에 적용되지만 체력에 5%만큼의 보호막이 적용됨
    // 보호막이 적용된 아군의 Damage 10%, Att Speed 10% 증가
    // 기적이 적용된 아군에게 5초간 무적효과 부여
    // 기적이 적용된 아군에게 10초간 Damage 50%, Att Speed 50% 증가
    // 이단심판으로 타격시 적 현재 HP의 5% 피해 추가
    // 아군들이 Damage, Att Speed, Active Damage, Active Cooldown 25% 증가
    // 이단심판으로 적을 피격하지 않아도 5초마다 홀리쉴드가 적용됨
    // 기적 스킬이 모든 아군에게 적용됨
    // Att Speed 10% 증가
    // 보호막이 적용된 아군의 Damage, Att Speed 1% 추가
    // 보호막이 최대 체력의 1%만큼 더 추가
    //Active CoolDown 1% 감소
}
