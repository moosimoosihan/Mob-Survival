using UnityEngine;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour
{
    public LevelItemData data;
    public bool isLevelUp = false;
    Text textDesc;
    Text textName;
    Text textTypeName;
    Image itemNameUI;
    Image itemTypeUI;
    public int level;
    public int playerNum;
    void Awake()
    {
        // 0 은 자기 자신
        Text[] texts = GetComponentsInChildren<Text>();
        textName = texts[0];
        textDesc = texts[1];
        textTypeName = texts[2];
        Image[] images = GetComponentsInChildren<Image>();
        itemNameUI = images[1];
        itemTypeUI = images[2];
    }

    public void Init()
    {

        switch(data.itemType){
            case LevelItemData.ItemType.궁수12:
            case LevelItemData.ItemType.용사12:
            case LevelItemData.ItemType.용사13:
            case LevelItemData.ItemType.용사14:
            case LevelItemData.ItemType.용사15:
            case LevelItemData.ItemType.궁수14:
            case LevelItemData.ItemType.궁수15:
            case LevelItemData.ItemType.현자12:
            case LevelItemData.ItemType.현자13:
            case LevelItemData.ItemType.현자14:
            case LevelItemData.ItemType.현자15:
            case LevelItemData.ItemType.사제12:
            case LevelItemData.ItemType.사제13:
            case LevelItemData.ItemType.사제14:
            case LevelItemData.ItemType.사제15:
                if(level==10)
                {
                    itemInit("골드 50 획득", "골드", "골드");
                }
                else {
                    itemInit(string.Format(data.itemDesc, data.value*100*(level+1)), data.itemName, data.itemTypeName.ToString());
                }
                break;
            case LevelItemData.ItemType.궁수13:
            itemInit(string.Format(data.itemDesc, data.value*(level+1)), data.itemName, data.itemTypeName.ToString());
                break;
                //마지막 아이템
            case LevelItemData.ItemType.빈칸:
                GetComponent<Button>().interactable = false;
                break;
            default:
                itemInit(data.itemDesc, data.itemName, data.itemTypeName.ToString());
                break;
        }
    }

    private void itemInit(string desc, string name, string type)
    {
        textDesc.text = desc;
        textName.text = name;
        textTypeName.text = type;
        switch(type){
            case "기본스킬":
                itemNameUI.color = new Color(0.3f,0.6f,0.8f);
                itemTypeUI.color = new Color(0.3f,0.6f,0.8f);
                break;
            case "유니크스킬":
                itemNameUI.color = new Color(1,0,0.4f);
                itemTypeUI.color = new Color(1,0,0.4f);
                break;
            case "레벨스킬":
                itemNameUI.color = new Color(0.64f,0.64f,0.64f);
                itemTypeUI.color = new Color(0.64f,0.64f,0.64f);
                break;
            case "골드":
                itemNameUI.color = new Color(1,0.89f,0);
                itemTypeUI.color = new Color(1,0.89f,0);
                break;
            case "빈칸":
                itemNameUI.color = new Color(1,1,1);
                itemTypeUI.color = new Color(1,1,1);
                break;
        }
    }

    public void OnClick()
    {
        // 각 선택지별 구현
        switch(data.itemType){
            case LevelItemData.ItemType.용사0:
                // 베기의 RangeType QuarterCircle 에서 HalfCircle로 변경, AttackRange 50%증가
                isLevelUp = true;
                GameManager.instance.players[playerNum].gameObject.GetComponentsInChildren<MeleeWeapon>()[0].curDetectionAngle =  GameManager.instance.players[playerNum].gameObject.GetComponentsInChildren<MeleeWeapon>()[0].detectionAngle * 1.5f;
                break;
                
            case LevelItemData.ItemType.용사1:
                // HP 25%이하시 5초동안 1초마다 10% HP회복 (총 50% HP) 60초 쿨다운
            case LevelItemData.ItemType.용사2:
                // 파티원 전체 Damage 25% 증가 
            case LevelItemData.ItemType.용사3:
                // Damage Reduction 10% 증가하며 피격시 해당 몬스터에 패시브2 부여
            case LevelItemData.ItemType.용사4:
                // 화상공격이 최대 5번 중첩됨
            case LevelItemData.ItemType.용사5:
                // 용사가 적 10마리 처치시 화상공격의 데미지 증가
            case LevelItemData.ItemType.용사6:
                // 용사의 일격의 RangeType QuarterCircle 에서 HalfCircle로 변경, AttackRange 50%증가
            case LevelItemData.ItemType.용사7:
                // 용사의 일격의 Cooldown 20초 감소
            case LevelItemData.ItemType.용사8:
                // 화상공격의 지속시간이 사라지며 영구히 부착됨
            case LevelItemData.ItemType.용사9:
                // Damage Reduction 10% 증가하며 파티원이 받는 데미지 50%를 용사가 대신 받음
            case LevelItemData.ItemType.용사10:
                // 화상공격이 5번 중첩된 몬스터가 사망시 주변 몬스터에게 화상을 부여함
            case LevelItemData.ItemType.용사11:
                // 용사의 일격으로 피해를 입힌 적에게 5중첩 화상공격을 부여

            case LevelItemData.ItemType.궁수0:
                // 피격시 은신을 부여하여 3초간 무적과 이동속도 100%증가 90초 쿨다운
            case LevelItemData.ItemType.궁수1:
                // Att Speed 50% 증가
            case LevelItemData.ItemType.궁수2:
                // 파티원 전체 Att Speed 25% 증가
            case LevelItemData.ItemType.궁수3:
                // 5회 기본 공격시 다음 공격 Crit Rate 100%
            case LevelItemData.ItemType.궁수4:
                // 치명타 발생시 적을 관통하여 범위상에 존재하는 적에게 피해를 줌
            case LevelItemData.ItemType.궁수5:
                // 치명타 발생시 액티브 스킬 쿨다운 1초 감소
            case LevelItemData.ItemType.궁수6:
                // 액티브의 BuffTime 20초 증가
            case LevelItemData.ItemType.궁수7:
                // 액티브가 Critical Damage 100%도 증가시킴

            case LevelItemData.ItemType.궁수8:
                // Crit Rate 20%와 Crit Damage 70% 증가
            case LevelItemData.ItemType.궁수9:
                // 30초마다 10초간 파티원 전체 신속 부여 (Att Speed 50%, Move Speed 50% 증가)
            case LevelItemData.ItemType.궁수10:
                // 치명타 발생시 유도 부여 (관통 후 가장 가까운 적 에게 자동 타겟 설정)
            case LevelItemData.ItemType.궁수11:
                // 액티브가 패시브1의 공격에 화살을 2개 더 추가함

            case LevelItemData.ItemType.현자0:
                // Att Speed 50% 증가하며 패시브1의 ProjectileNumber 1개증가
            case LevelItemData.ItemType.현자1:
                // Damage 50% 증가하며 패시브1의 Attack Range 50%증가
            case LevelItemData.ItemType.현자2:
                // Add Exp 25%증가
            case LevelItemData.ItemType.현자3:
                // 캐릭터에게 피해를 입은 적에게 빙결을 부여 (10초간 이동속도 10%감소 최대 4중첩)
            case LevelItemData.ItemType.현자4:
                // 패시브2의 Attack Range 100% 와 지속시간 5초 증가
            case LevelItemData.ItemType.현자5:
                // 패시브2가 적의 이동속도 10% 감소시킴
            case LevelItemData.ItemType.현자6:
                // 액티브의 Attach Range 200%증가
            case LevelItemData.ItemType.현자7:
                // 액티브의 지속시간 10초증가하며 Cooldown 30초 감소

            case LevelItemData.ItemType.현자8:
                // 패시브1로 피해를 입힌 적에게 4중첩 빙결을 부여하며 Projectile Number 2개 증가
            case LevelItemData.ItemType.현자9:
                // 4중첩 빙결된 적에게 피해를 줄 시 해당 적의 이동속도를 2초간 100% 감소 
            case LevelItemData.ItemType.현자10:
                // 패시브2로 피해를 입는 적에게 2중첩 빙결을 부여하며 해당 적에게 50% 추가 피해
            case LevelItemData.ItemType.현자11:
                // 액티브 스킬의 범위가 맵 전체로 변경 지속시간 5초 감소와 CoolDown 30초 증가

            case LevelItemData.ItemType.사제0:
                // Att Speed 30% 증가하며 패시브1로 타격시 2초간 이동속도 100% 감소
            case LevelItemData.ItemType.사제1:
                // 아군에게 적용되어 있는 디버프 1개 제거 30초 쿨다운
            case LevelItemData.ItemType.사제2:
                // 파티원 전체 Damage 15%, Defense 15% 증가 
            case LevelItemData.ItemType.사제3:
                // 보호막이 적용된 아군이 5초마다 잃은 HP의 5%를 회복하
            case LevelItemData.ItemType.사제4:
                // 보호막이 파티 전체에 적용되지만 1중첩당 체력에 1%만큼의 보호막이 적용됨
            case LevelItemData.ItemType.사제5:
                // 보호막이 적용된 아군의 Damage 10%, Att Speed 10% 증가
            case LevelItemData.ItemType.사제6:
                // 액티브 스킬이 적용된 아군에게 5초간 무적효과 부여
            case LevelItemData.ItemType.사제7:
                // 액티브 스킬이 적용된 아군에게 10초감 Damage 50%, Att Speed 50% 증가

            case LevelItemData.ItemType.사제8:
                // 패시브1로 기절한 적 타격시 적 최대 HP의 10% 추가 피해
            case LevelItemData.ItemType.사제9:
                // 아군들이 15초마다 5초간 유지되는 버프를 획득함
            case LevelItemData.ItemType.사제10:
                // 패시브1로 적을 피격하지 않아도 5초마다 보호막이 적용되며 체력 회복량이 5% 추가됨
            case LevelItemData.ItemType.사제11:
                // 액티브 스킬이 모든 아군에게 적용됨
                isLevelUp = true;
                break;

            case LevelItemData.ItemType.용사12:
                // Damage,Att Range 5% 증가
            case LevelItemData.ItemType.용사13:
                // Damage Reduction 3% 증가
            case LevelItemData.ItemType.용사14:
                // 화상공격의 데미지 증가
            case LevelItemData.ItemType.용사15:
                // Active Damage 15% 증가
            
            case LevelItemData.ItemType.궁수12:
                // Damage,Att Speed 5% 증가
            case LevelItemData.ItemType.궁수13:
                // 은신시 무적시간 1초증가
            case LevelItemData.ItemType.궁수14:
                // 패시브2의 치명타 발생시 Att Speed 1% 추가 증가
            case LevelItemData.ItemType.궁수15:
                // 액티브의 Crit Rate 추가 1% 증가

            case LevelItemData.ItemType.현자12:
                // Damage 5%, 패시브1의 Attack Range 5% 증가
            case LevelItemData.ItemType.현자13:
                // Damage 5%, 패시브2의 Attack Range 5% 증가
            case LevelItemData.ItemType.현자14:
                // 이동속도 1% 추가 감소
            case LevelItemData.ItemType.현자15:
                // Active Damage 15% 증가

            case LevelItemData.ItemType.사제12:
                // Att Speed 10% 증가
            case LevelItemData.ItemType.사제13:
                // 보호막이 적용된 아군의 Damage, Att Speed 1% 추가
            case LevelItemData.ItemType.사제14:
                // 보호막이 최대 체력의 1%만큼 더 추가
            case LevelItemData.ItemType.사제15:
                // Active CoolDown 1% 감소
                if(level==10){
                    GameManager.instance.gold += 50;
                } else {
                    level++;
                    Init();
                }
                break;

            case LevelItemData.ItemType.빈칸:
                break;
        }
        if(isLevelUp){
            GetComponent<Button>().interactable = false;
        }
    }
}
