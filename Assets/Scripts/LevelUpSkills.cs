using System.Collections;
using System.Collections.Generic;
using olimsko;
using UnityEngine;
using UnityEngine.Pool;

public class LevelUpSkills : MonoBehaviour
{
    static PlayerContext playerContext = OSManager.GetService<ContextManager>().GetContext<PlayerContext>();

    // 용사 0번 스킬 숙련된 베기 하프에서 서클로 변경
    public static void WorriorSkill0(Transform transform, IObjectPool<Bullet> poolBullet, Vector3 _dir, float _scalex, float _spawnDistance, float CurDetectionAngle, Player player, float Damage, int count, float knockBackPower, float duration){
        if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill.ContainsKey(0)){
            if(playerContext.DicPlayerEquipedSkill[0].DicEquipedSkill[0].Level > 0){
                Transform bulletBack = poolBullet.Get().transform;
                
                Vector3 backDir = new Vector3(_dir.x * -1,_dir.y * -1, _dir.z);
                bulletBack.parent = GameManager.instance.pool.transform;
                bulletBack.transform.localScale = new Vector3(_scalex, transform.transform.localScale.y, transform.transform.localScale.z);
                bulletBack.position = transform.position + backDir * _spawnDistance;
                bulletBack.rotation = Quaternion.FromToRotation(Vector3.down, _dir);
                bulletBack.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(player, Damage, out bool isCritical), count, Vector3.zero, knockBackPower, duration, isCritical);
                bulletBack.GetComponent<EffectBullet>().detectionAngle = CurDetectionAngle;
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
}
