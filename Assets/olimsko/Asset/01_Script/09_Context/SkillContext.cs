using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using olimsko;
using System.Linq;

public class SkillContext : ContextModel
{
    private PlayerContext PlayerContext => OSManager.GetService<ContextManager>().GetContext<PlayerContext>();
    private SkillTableSO SkillTable => OSManager.GetService<DataManager>().GetData<SkillTableSO>();

    // 용사
    // 용사 0번 스킬 숙련된 베기 하프에서 서클로 변경
    public bool WarriorSkill0()
    {
        if (PlayerContext.IsHasSkill(0, 0))
        {
            return true;
        }
        return false;
    }

    // 용사 1번 스킬 범위증가(베기) 베기스킬 범위 2배 증가
    public float[] WarriorSkill1(float scalex, float detectionAngle)
    {
        float[] result = { scalex, detectionAngle };
        if (PlayerContext.IsHasSkill(0, 1))
        {
            float val = SkillTable.SkillTable[1].Value[0];
            result[0] *= val;
            result[1] = detectionAngle * val;
        }
        return result;
    }

    // 용사 2번 스킬 파티버프 전체 데미지 15% 증가
    public float WarriorSkill2(float _damage)
    {
        if (PlayerContext.IsHasSkill(0, 2))
        {
            if (_damage != 0)
            {
                float val = SkillTable.SkillTable[2].Value[0];
                return _damage * val;
            }
        }
        return 0;
    }

    // 용사 3스킬 튼튼한 갑옷 데미지 10% 감소
    public float WarriorSkill3(float _damage)
    {
        if (PlayerContext.IsHasSkill(0, 3))
        {
            if (_damage != 0)
            {
                float val = SkillTable.SkillTable[3].Value[0];
                return _damage * val;
            }
        }
        return 0;
    }

    // 용사 4스킬 화염 갑옷 용사 피격시 적군에게 화상스킬
    public void WarriorSkill4(Enemy enemy, float _damage, float _duration)
    {
        if (PlayerContext.IsHasSkill(0, 4) && enemy != null && enemy.gameObject.activeSelf)
        {
            enemy.StartCoroutine(enemy.WarriorFireOn(_damage, _duration, true));
        }
    }

    // 용사 5스킬 화상 피해 증가 10마리당 0.1% 증가
    public float WarriorSkill5(int killCount)
    {
        if (PlayerContext.IsHasSkill(0, 5))
        {
            float val = SkillTable.SkillTable[5].Value[0];
            return Mathf.Floor(killCount / 10) * val;
        }
        return 0;
    }

    // 용사 6스킬 용사의 일격 스킬 Projectile Type이 QuarterCircle 에서 HalfCircle로 변경 (액티브 스킬 좌우로 넓어짐)
    public float WarriorSkill6()
    {
        if (PlayerContext.IsHasSkill(0, 6))
        {
            return SkillTable.SkillTable[6].Value[0];
        }
        return 0;
    }

    // 용사 7스킬 용사의 일격 스킬의 Project Range 600으로변경 (액티브 스킬로 바꾸고 속도 2배 증가)
    public float WarriorSkill7()
    {
        if (PlayerContext.IsHasSkill(0, 7))
        {
            return SkillTable.SkillTable[7].Value[0];
        }
        return 0;
    }

    // 용사 8스킬 화상 스킬의 Buff Time이 200으로 번경
    public float WarriorSkill8()
    {
        if (PlayerContext.IsHasSkill(0, 8))
        {
            float val = SkillTable.SkillTable[8].Value[0];
            return val;
        }
        return 0;
    }

    // 용사 9스킬 Damage Reduction 10% 증가하며 파티원이 받는 데미지 50%를 용사가 대신 받음
    public float WarriorSkill9(string character, float _damage, bool isCritical)
    {
        if (PlayerContext.IsHasSkill(0, 9))
        {
            // 워리어라면 10% 감소된 데미지를 받음
            if (character.Equals("용사"))
            {
                float val = SkillTable.SkillTable[9].Value[0];
                return _damage * val;
            }
            else
            {
                // 나머지는 워리어가 살아있다면 50%의 데미지를 워리어에게 전달
                foreach (Player player in GameManager.instance.players)
                {
                    if (player.character.Equals("용사") && !player.playerDead)
                    {
                        float val = SkillTable.SkillTable[9].Value[1];
                        player.GetDamage(_damage * val, isCritical, null, true);
                        return _damage * val;
                    }
                }
            }
        }
        return 0;
    }

    // 용사 10스킬 화상 디버프가 있는 몬스터가 사망시 Range 100 이내 주변 몬스터에게 화상 디버프를 부여
    public bool WarriorSkill10()
    {
        if (PlayerContext.IsHasSkill(0, 10))
        {
            return true;
        }
        return false;
    }

    // 용사 11스킬 용사의 일격 스킬 Projectile Type을 Half Circle에서 Circle로 변경시키고 Cooldown 20초 감소(액티브 스킬 앞 뒤로 나가도록!)
    public bool WarriorSkill11()
    {
        if (PlayerContext.IsHasSkill(0, 11))
        {
            return true;
        }
        return false;
    }

    // 용사 12스킬 Damage,Att Range 5% 증가 (공격 범위는 적용 해야함)
    public float WarriorSkill12(float values)
    {
        if (PlayerContext.IsHasSkill(0, 12))
        {
            float val = SkillTable.SkillTable[12].Value[0];
            int level = PlayerContext.GetSkillLevel(0, 12);
            return values * (level * val);
        }
        return 0;
    }

    // 용사 13스킬 Damage Reduction 3% 증가
    public float WarriorSkill13(float _damage)
    {
        if (PlayerContext.IsHasSkill(0, 13))
        {
            int level = PlayerContext.GetSkillLevel(0, 13);
            if (_damage > 0)
            {
                float val = SkillTable.SkillTable[13].Value[0];
                return _damage * (val * level);
            }
        }
        return 0;
    }

    // 용사 14스킬 화상 스킬의 Debuff Value 0.1% 증가
    public float WarriorSkill14()
    {
        if (PlayerContext.IsHasSkill(0, 14))
        {
            float val = SkillTable.SkillTable[14].Value[0];
            int level = PlayerContext.GetSkillLevel(0, 14);
            return val * level;
        }
        return 0;
    }

    // 용사 15스킬 Active Damage 15% 증가
    public float WarriorSkill15(float _damage)
    {
        if (PlayerContext.IsHasSkill(0, 15))
        {
            int level = PlayerContext.GetSkillLevel(0, 15);
            if (_damage > 0)
            {
                float val = SkillTable.SkillTable[15].Value[0];
                return _damage * (val * level);
            }
        }
        return 0;
    }

    // 궁수
    // 궁수 0스킬 피격시 무적시간 1초(현재 0.5f), 이동속도 100%증가로 변경 (피격시 5초동안 이속증가 버프)
    public float[] ArcherSkill0()
    {
        float[] vals = { 0, 0 };
        if (PlayerContext.IsHasSkill(1, 16))
        {
            vals[0] = SkillTable.SkillTable[16].Value[0];
            vals[1] = SkillTable.SkillTable[16].Value[1];
            return vals;
        }
        return vals;
    }

    // 궁수 1스킬 Att Speed 50% 증가
    public float ArcherSkill1()
    {
        if (PlayerContext.IsHasSkill(1, 17))
        {
            return SkillTable.SkillTable[17].Value[0];
        }
        return 0;
    }

    // 궁수 2스킬 파티원 전체 Att Speed/Active Cooldown 15% 증가
    public float ArcherSkill2()
    {
        if (PlayerContext.IsHasSkill(1, 18))
        {
            return SkillTable.SkillTable[18].Value[0]; ;
        }
        return 0;
    }

    // 궁수 3스킬 사격에 특수효과를 추가하여 5회 기본 공격시 Crit Rate를 100%로 변경
    public bool ActherSkill3()
    {
        if (PlayerContext.IsHasSkill(1, 19))
        {
            return true;
        }
        return false;
    }

    // 궁수 4스킬 궁수가 적 50마리 처치시 사격스킬의 투사체 관통 1증가
    public int ArcherSkill4(int count)
    {
        if (PlayerContext.IsHasSkill(1, 20))
        {
            return count / (int)SkillTable.SkillTable[20].Value[0]; // 50마리마다 1씩 증가
        }
        return 0;
    }

    // 궁수 5스킬 치명타 발생시 액티브 스킬 쿨다운 1초 감소
    public float ArcherSkill5()
    {
        if (PlayerContext.IsHasSkill(1, 21))
        {
            return SkillTable.SkillTable[21].Value[0]; ;
        }
        return 0;
    }

    // 궁수 6스킬 액티브의 BuffTime 20초 증가
    public float ArcherSkill6()
    {
        if (PlayerContext.IsHasSkill(1, 22))
        {
            return SkillTable.SkillTable[22].Value[0]; ;
        }
        return 0;
    }

    // 궁수 7스킬 액티브가 Critical Damage 100%도 증가시킴
    public float ArcherSkill7()
    {
        if (PlayerContext.IsHasSkill(1, 23))
        {
            return SkillTable.SkillTable[23].Value[0]; ;
        }
        return 0;
    }

    // 궁수 8스킬 Crit Rate 20%와 Crit Damage 70% 증가
    public float[] ArcherSkill8()
    {
        float[] result = { 0, 0 };
        if (PlayerContext.IsHasSkill(1, 24))
        {
            result[0] += SkillTable.SkillTable[24].Value[0];
            result[1] += SkillTable.SkillTable[24].Value[1];
        }
        return result;
    }

    // 궁수 9스킬 30초마다 10초간 파티원 전체 신속 부여 (Att Speed 50%, Move Speed 50% 증가)
    public float ArcherSkill9()
    {
        if (PlayerContext.IsHasSkill(1, 25))
        {
            return SkillTable.SkillTable[25].Value[0];
        }
        return 0;
    }

    // 궁수 10스킬 투사체 관통 1증가 시키고 유도 부여 (관통시 가장 가까운 적 에게 자동 타겟 설정)
    public bool ArcherSkill10()
    {
        if (PlayerContext.IsHasSkill(1, 26))
        {
            return true;
        }
        return false;
    }

    // 궁수 11스킬 액티브가 지속중인 동안 사격 스킬의 Number of Projectile 2개 더 추가함 / 다발사격 느낌으로 좌우로 퍼져서 2개 추가되어 나가도록!
    public float ArcherSkill11()
    {
        if (PlayerContext.IsHasSkill(1, 27))
        {
            return SkillTable.SkillTable[27].Value[0];
        }
        return 0;
    }

    // 궁수 12스킬 Damage,Att Speed 5% 증가
    public float ArcherSkill12(float _damage)
    {
        if (PlayerContext.IsHasSkill(1, 28))
        {
            float val = SkillTable.SkillTable[28].Value[0];
            int level = PlayerContext.GetSkillLevel(1, 28);
            return _damage * (val * level);
        }
        return 0;
    }

    // 궁수 13스킬 Evasion 1% 증가
    public float ArcherSkill13()
    {
        if (PlayerContext.IsHasSkill(1, 29))
        {
            int level = PlayerContext.GetSkillLevel(1, 29);
            return SkillTable.SkillTable[29].Value[0] * level;
        }
        return 0;
    }

    // 궁수 14스킬 패시브2의 치명타 발생시 Att Speed 1% 추가 증가
    public float ArcherSkill14()
    {
        if (PlayerContext.IsHasSkill(1, 30))
        {
            int level = PlayerContext.GetSkillLevel(1, 30);
            return SkillTable.SkillTable[30].Value[0] * level;
        }
        return 0;
    }
    // 궁수 15스킬 액티브의 Crit Rate 추가 1% 증가
    public float ArcherSkill15()
    {
        if (PlayerContext.IsHasSkill(1, 31))
        {
            int level = PlayerContext.GetSkillLevel(1, 31);
            return SkillTable.SkillTable[31].Value[0] * level;
        }
        return 0;
    }

    // 현자
    // 현자 0스킬 Damage 50% 증가하며 ProjectSize 15로 변경 (아이스볼트가 커짐)
    public float[] WizardSkill0(float _damage = 0)
    {
        float[] vals = { 0, 0 };
        if (PlayerContext.IsHasSkill(2, 32))
        {
            vals[0] = _damage * SkillTable.SkillTable[32].Value[0];
            vals[1] = SkillTable.SkillTable[32].Value[1];
            return vals;
        }
        return vals;
    }

    // 현자 1스킬 Att Speed 50% 증가하며 ProjectileSpeed 2로 변경 / 총알 투사체 속도가 증가 구현해야함
    public float WizardSkill1()
    {
        if (PlayerContext.IsHasSkill(2, 33))
        {
            return SkillTable.SkillTable[33].Value[0];
        }
        return 0;
    }

    // 현자 2스킬 Add Exp 25%증가
    public float WizardSkill2(float exp)
    {
        if (PlayerContext.IsHasSkill(2, 34))
        {
            return exp * SkillTable.SkillTable[34].Value[0];
        }
        return 0;
    }

    // 현자 3스킬 모든 스킬에 특수효과로 디버프 빙결을 부여 (피격시 10초간 이동속도 10%감소 4중첩 가능)
    public bool WizardSkill3()
    {
        if (PlayerContext.IsHasSkill(2, 35))
        {
            return true;
        }
        return false;
    }

    // 현자 4스킬 아이스 필드 스킬의 Area Size 200으로 변경
    public float WizardSkill4()
    {
        if (PlayerContext.IsHasSkill(2, 36))
        {
            return SkillTable.SkillTable[36].Value[0];
        }
        return 0;
    }
    // 현자 5스킬 아이스 필드 스킬의 Area Duration 10으로 변경
    public float WizardSkill5()
    {
        if (PlayerContext.IsHasSkill(2, 37))
        {
            return SkillTable.SkillTable[37].Value[0];
        }
        return 0;
    }
    // 현자 6스킬 블리자드의 Area Size 500으로 변경
    public float WizardSkill6()
    {
        if (PlayerContext.IsHasSkill(2, 38))
        {
            return SkillTable.SkillTable[38].Value[0];
        }
        return 0;
    }
    // 현자 7스킬 블리자드의 Area Duration 20초로 변경하고 Cooldown 60초로 변경
    public float[] WizardSkill7()
    {
        float[] vals = { 0, 0 };
        if (PlayerContext.IsHasSkill(2, 39))
        {
            vals[0] = SkillTable.SkillTable[39].Value[0];
            vals[1] = SkillTable.SkillTable[39].Value[1];
            return vals;
        }
        return vals;
    }

    // 현자 8스킬 아이스볼 Damage 30, Cooldown 5초로 변경시키며 피격시 빙결 2중첩씩 부여
    public float[] WizardSkill8()
    {
        float[] vals = { 0, 0 };
        if (PlayerContext.IsHasSkill(2, 40))
        {
            vals[0] = SkillTable.SkillTable[40].Value[0];
            vals[1] = SkillTable.SkillTable[40].Value[1];
            return vals;
        }
        return vals;
    }

    // 현자 9스킬 빙결 4중첩된 적을 피격시 2초간 이동속도 100%감소시킴
    public bool WizardSkill9()
    {
        if (PlayerContext.IsHasSkill(2, 41))
        {
            return true;
        }
        return false;
    }
    // 현자 10스킬 아이스 필드의 Area Size를 300, Area Duration을15초로 변경
    public float[] WizardSkill10()
    {
        float[] vals = { 0, 0 };
        if (PlayerContext.IsHasSkill(2, 42))
        {
            vals[0] = SkillTable.SkillTable[42].Value[0];
            vals[1] = SkillTable.SkillTable[42].Value[1];
            return vals;
        }
        return vals;
    }
    // 현자 11스킬 블리자드의 범위가 맵 전체로 변경 지속시간 5초 감소와 CoolDown 30초 증가
    public float[] WizardSkill11()
    {
        float[] vals = { 0, 0 };
        if (PlayerContext.IsHasSkill(2, 43))
        {
            vals[0] = SkillTable.SkillTable[43].Value[0];
            vals[1] = SkillTable.SkillTable[43].Value[1];
            return vals;
        }
        return vals;
    }

    // 현자 12스킬 Damage,Att Range 5% 증가
    public float WizardSkill12(float _damage)
    {
        if (PlayerContext.IsHasSkill(2, 44))
        {
            float val = SkillTable.SkillTable[44].Value[0];
            int level = PlayerContext.GetSkillLevel(2, 44);
            return _damage * (val * level);
        }
        return 0;
    }

    // 현자 13스킬 아이스 필드의 Area Size 10% 증가
    public float WizardSkill13()
    {
        if (PlayerContext.IsHasSkill(2, 45))
        {
            float val = SkillTable.SkillTable[45].Value[0];
            int level = PlayerContext.GetSkillLevel(2, 45);
            return val * level;
        }
        return 0;
    }
    // 현자 14스킬 디버프 빙결의 이동속도 감소를 1% 추가 감소
    public float WizardSkill14()
    {
        if (PlayerContext.IsHasSkill(2, 46))
        {
            float val = SkillTable.SkillTable[46].Value[0];
            int level = PlayerContext.GetSkillLevel(2, 46);
            return val * level;
        }
        return 0;
    }
    // 현자 15스킬 Active Damage 15% 증가
    public float WizardSkill15(float _damage)
    {
        if (PlayerContext.IsHasSkill(2, 47))
        {
            float val = SkillTable.SkillTable[47].Value[0];
            int level = PlayerContext.GetSkillLevel(2, 47);
            return _damage * (val * level);
        }
        return 0;
    }

    // 사제
    // 사제 0스킬 Att Speed 50% 증가하며 이단심판으로 타격시 2초간 이동속도 100% 감소
    public float[] PriestSkill0()
    {
        float[] vals = { 0, 0 };
        if (PlayerContext.IsHasSkill(3, 48))
        {
            vals[0] = SkillTable.SkillTable[48].Value[0];
            vals[1] = SkillTable.SkillTable[48].Value[1];
            return vals;
        }
        return vals;
    }

    // 사제 1스킬 이단심판으로 타격시 아군에게 적용되어 있는 디버프 1개 제거
    public bool PriestSkill1()
    {
        if (PlayerContext.IsHasSkill(3, 49))
        {
            return true;
        }
        return false;
    }

    // 사제 2스킬 파티원 전체 Damage 15%, Defense 15% 증가
    public float PriestSkill2(float value)
    {
        if (PlayerContext.IsHasSkill(3, 50))
        {
            float val = SkillTable.SkillTable[50].Value[0];
            return value * val;
        }
        return 0;
    }

    // 사제 3스킬 홀리쉴드가 적용된 아군이 5초마다 잃은 HP의 5%를 회복
    public float PriestSkill3()
    {
        if (PlayerContext.IsHasSkill(3, 51))
        {
            return SkillTable.SkillTable[51].Value[0];
        }
        return 0;
    }

    // 사제 4스킬 홀리쉴드가 파티 전체에 적용되지만 체력에 5%만큼의 보호막이 적용됨(현재 10 적용되니 5로 줄이는건가?)
    public float PriestSkill4()
    {
        if (PlayerContext.IsHasSkill(3, 52))
        {
            return SkillTable.SkillTable[52].Value[0];
        }
        return 0;
    }

    // 사제 5스킬 보호막이 적용된 아군의 Damage 10%, Att Speed 10% 증가
    public float PriestSkill5(float value)
    {
        if (PlayerContext.IsHasSkill(3, 53))
        {
            float val = SkillTable.SkillTable[53].Value[0];
            return value * val;
        }
        return 0;
    }
    // 사제 6스킬 기적이 적용된 아군에게 5초간 무적효과 부여
    public float PriestSkill6()
    {
        if (PlayerContext.IsHasSkill(3, 54))
        {
            return SkillTable.SkillTable[54].Value[0];
        }
        return 0;
    }
    // 사제 7스킬 기적이 적용된 아군에게 10초간 Damage 50%, Att Speed 50% 증가
    public float PriestSkill7(float value)
    {
        if (PlayerContext.IsHasSkill(3, 55))
        {
            float val = SkillTable.SkillTable[55].Value[0];
            return value * val;
        }
        return 0;
    }

    // 사제 8스킬 이단심판으로 타격시 적 현재 HP의 5% 피해 추가
    public float PriestSkill8()
    {
        if (PlayerContext.IsHasSkill(3, 56))
        {
            return SkillTable.SkillTable[56].Value[0];
        }
        return 0;
    }

    // 사제 9스킬 아군들이 Damage, Att Speed, Active Damage, Active Cooldown 25% 증가
    public float PriestSkill9(float value)
    {
        if (PlayerContext.IsHasSkill(3, 57))
        {
            float val = SkillTable.SkillTable[57].Value[0];
            return value * val;
        }
        return 0;
    }

    // 사제 10스킬 이단심판으로 적을 피격하지 않아도 5초마다 홀리쉴드가 적용됨
    public float PriestSkill10()
    {
        if (PlayerContext.IsHasSkill(3, 58))
        {
            return SkillTable.SkillTable[58].Value[0];
        }
        return 0;
    }

    // 사제 11스킬 기적 스킬이 모든 아군에게 적용됨
    public bool PriestSkill11()
    {
        if (PlayerContext.IsHasSkill(3, 59))
        {
            return true;
        }
        return false;
    }

    // 사제 12스킬 Att Speed 10% 증가
    public float PriestSkill12()
    {
        if (PlayerContext.IsHasSkill(3, 60))
        {
            float level = PlayerContext.GetSkillLevel(3, 60);
            return level * SkillTable.SkillTable[60].Value[0];
        }
        return 0;
    }

    // 사제 13스킬 보호막이 적용된 아군의 Damage, Att Speed 1% 추가
    public float PriestSkill13(float value)
    {
        if (PlayerContext.IsHasSkill(3, 61))
        {
            float val = SkillTable.SkillTable[61].Value[0];
            float level = PlayerContext.GetSkillLevel(3, 61);
            return value * (val * level);
        }
        return 0;
    }
    // 사제 14스킬 보호막이 최대 체력의 1%만큼 더 추가 (보호막이 현재 값으로 들어가있어서 수정해야 할듯!)
    public float PriestSkill14(float value)
    {
        if (PlayerContext.IsHasSkill(3, 62))
        {
            float val = SkillTable.SkillTable[62].Value[0];
            float level = PlayerContext.GetSkillLevel(3, 62);
            return value * (val * level);
        }
        return 0;
    }

    // 사제 15스킬 Active CoolDown 1% 감소
    public float PriestSkill15()
    {
        if (PlayerContext.IsHasSkill(3, 63))
        {
            float val = SkillTable.SkillTable[63].Value[0];
            float level = PlayerContext.GetSkillLevel(3, 63);
            return val * level;
        }
        return 0;
    }

    public int[] GetItemValues(int itemID)
    {
        int[] values = new int[3] { 0, 0, 0 };

        ItemTable itemTable = PlayerContext.IsHasItem(itemID);
        if (itemTable != null)
        {
            StatType statType = itemTable.StatType1;
            StatAddType statAddType = itemTable.StatAddType1;
            values[0] = itemTable.StatValue1;
            values[1] = itemTable.StatValue2;
            values[2] = itemTable.StatValue3;
            return values;
        }
        return values;
    }
    // TODO : AttRange, ElementalDamage 구현 해야 함
}
