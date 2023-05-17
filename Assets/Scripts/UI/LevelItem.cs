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
            case LevelItemData.ItemType.용사1:
            case LevelItemData.ItemType.용사2:
            case LevelItemData.ItemType.용사3:
            case LevelItemData.ItemType.용사4:
            case LevelItemData.ItemType.용사5:
            case LevelItemData.ItemType.용사6:
            case LevelItemData.ItemType.용사7:
            
            case LevelItemData.ItemType.용사8:
            case LevelItemData.ItemType.용사9:
            case LevelItemData.ItemType.용사10:
            case LevelItemData.ItemType.용사11:

            case LevelItemData.ItemType.궁수0:
            case LevelItemData.ItemType.궁수1:
            case LevelItemData.ItemType.궁수2:
            case LevelItemData.ItemType.궁수3:
            case LevelItemData.ItemType.궁수4:
            case LevelItemData.ItemType.궁수5:
            case LevelItemData.ItemType.궁수6:
            case LevelItemData.ItemType.궁수7:

            case LevelItemData.ItemType.궁수8:
            case LevelItemData.ItemType.궁수9:
            case LevelItemData.ItemType.궁수10:
            case LevelItemData.ItemType.궁수11:

            case LevelItemData.ItemType.현자0:
            case LevelItemData.ItemType.현자1:
            case LevelItemData.ItemType.현자2:
            case LevelItemData.ItemType.현자3:
            case LevelItemData.ItemType.현자4:
            case LevelItemData.ItemType.현자5:
            case LevelItemData.ItemType.현자6:
            case LevelItemData.ItemType.현자7:

            case LevelItemData.ItemType.현자8:
            case LevelItemData.ItemType.현자9:
            case LevelItemData.ItemType.현자10:
            case LevelItemData.ItemType.현자11:

            case LevelItemData.ItemType.사제0:
            case LevelItemData.ItemType.사제1:
            case LevelItemData.ItemType.사제2:
            case LevelItemData.ItemType.사제3:
            case LevelItemData.ItemType.사제4:
            case LevelItemData.ItemType.사제5:
            case LevelItemData.ItemType.사제6:
            case LevelItemData.ItemType.사제7:

            case LevelItemData.ItemType.사제8:
            case LevelItemData.ItemType.사제9:
            case LevelItemData.ItemType.사제10:
            case LevelItemData.ItemType.사제11:
                isLevelUp = true;
                break;

            case LevelItemData.ItemType.용사12:
            case LevelItemData.ItemType.용사13:
            case LevelItemData.ItemType.용사14:
            case LevelItemData.ItemType.용사15:
            
            case LevelItemData.ItemType.궁수12:
            case LevelItemData.ItemType.궁수13:
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
