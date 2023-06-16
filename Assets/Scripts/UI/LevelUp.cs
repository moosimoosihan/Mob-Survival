using UnityEngine;
using UnityEngine.UI;
public class LevelUp : MonoBehaviour
{
    public Font font;
    RectTransform rect;
    LevelItem[] items;
    public RectTransform itemList;
    public LevelItemData[] data;
    public Image[] playerSptrite;
    public Text[] playerText;
    public int maxLen = 16;
    public Image[] selPlayer;
    void Start()
    {
        rect = GetComponent<RectTransform>();
        // 캐릭터별 선언
        items = new LevelItem[(GameManager.instance.players.Length*maxLen) + 1];
        
        for(int i=0;i<GameManager.instance.players.Length;i++){
            playerSptrite[i].transform.parent.gameObject.SetActive(true);
            switch(GameManager.instance.players[i].chacter){
                case Player.characterInfo.용사:
                    playerSptrite[i].sprite = GameManager.instance.playerSptrite[0];
                    playerText[i].text = "용사";
                    for(int y=0;y<maxLen;y++)
                    {
                        // 16개의 아이템 추가
                        NewItem(i, y, 0, false);
                    }
                    break;
                case Player.characterInfo.궁수:
                    playerSptrite[i].sprite = GameManager.instance.playerSptrite[1];
                    playerText[i].text = "궁수";
                    for(int y=0;y<maxLen;y++){
                        // 16개의 아이템 추가
                        NewItem(i, y, 1, false);
                    }
                    break;
                case Player.characterInfo.현자:
                    playerSptrite[i].sprite = GameManager.instance.playerSptrite[2];
                    playerText[i].text = "현자";
                    for(int y=0;y<maxLen;y++){
                        // 16개의 아이템 추가
                        NewItem(i, y, 2, false);
                    }
                    break;
                case Player.characterInfo.사제:
                    playerSptrite[i].sprite = GameManager.instance.playerSptrite[3];
                    playerText[i].text = "사제";
                    for(int y=0;y<maxLen;y++){
                        // 16개의 아이템 추가
                        NewItem(i, y, 3, false);
                    }
                    break;
            }
        }
        NewItem(GameManager.instance.players.Length - 1, maxLen, 0, true);
    }

    private void NewItem(int i, int y, int playerNum, bool isblank)
    {
        // 상위 오브젝트
        GameObject item = new GameObject(isblank? $"blank{i}" : $"{i}" + "-" + $"{y}");
        RectTransform itemRectTransform = item.AddComponent<RectTransform>();
        itemRectTransform.SetParent(itemList);
        itemRectTransform.localScale = Vector3.one;
        itemRectTransform.sizeDelta = new Vector2(217.5f,600);
        VerticalLayoutGroup vertical = item.AddComponent<VerticalLayoutGroup>();
        vertical.childControlHeight = false;
        Button itemButton = item.AddComponent<Button>();
        Image itemImage = item.AddComponent<Image>();
        
        // item 상위에 스킬이름 오브젝트 넣기
        GameObject skillNameUI = new GameObject("SkillNameUI");
        RectTransform nameUIRectTransform = skillNameUI.AddComponent<RectTransform>();
        nameUIRectTransform.SetParent(item.transform);
        nameUIRectTransform.localScale = Vector3.one;
        nameUIRectTransform.sizeDelta = new Vector2(nameUIRectTransform.sizeDelta.x , 150);
        Image itemSkillNameUI = skillNameUI.AddComponent<Image>();
        itemSkillNameUI.raycastTarget = false;

        GameObject skillNameTextItem = new GameObject("SkillNameText");
        RectTransform nameTextRectTransform = skillNameTextItem.AddComponent<RectTransform>();        
        nameTextRectTransform.SetParent(skillNameUI.transform);
        nameTextRectTransform.localScale = Vector3.one;
        Text itemSkillText = skillNameTextItem.AddComponent<Text>();
        itemSkillText.alignment = TextAnchor.MiddleCenter;
        itemSkillText.font = font;
        itemSkillText.color = Color.black;
        itemSkillText.fontSize = 20;
        itemSkillText.fontStyle = FontStyle.Bold;
        itemSkillText.horizontalOverflow = HorizontalWrapMode.Overflow;
        itemSkillText.verticalOverflow = VerticalWrapMode.Overflow;
        itemSkillText.raycastTarget = false;

        // 스킬 설명
        GameObject descTextItem = new GameObject("SkillDescText");
        RectTransform textRectTransform = descTextItem.AddComponent<RectTransform>();        
        textRectTransform.SetParent(item.transform);
        textRectTransform.localScale = Vector3.one;
        textRectTransform.sizeDelta = new Vector2(textRectTransform.sizeDelta.x , 300);
        Text itemDescText = descTextItem.AddComponent<Text>();
        itemDescText.alignment = TextAnchor.MiddleCenter;
        itemDescText.font = font;
        itemDescText.color = Color.black;
        itemDescText.fontSize = 15;
        itemDescText.fontStyle = FontStyle.Bold;
        itemDescText.horizontalOverflow = HorizontalWrapMode.Wrap;
        itemDescText.verticalOverflow = VerticalWrapMode.Overflow;
        itemDescText.raycastTarget = false;

        // item 하위에 스킬타입 오브젝트 넣기
        GameObject skillTypeUI = new GameObject("SkillTypeUI");
        RectTransform typeUIRectTransform = skillTypeUI.AddComponent<RectTransform>();
        typeUIRectTransform.SetParent(item.transform);
        typeUIRectTransform.localScale = Vector3.one;
        typeUIRectTransform.sizeDelta = new Vector2(typeUIRectTransform.sizeDelta.x , 150);
        Image itemSkillTypeUI = skillTypeUI.AddComponent<Image>();

        GameObject skillTypeTextItem = new GameObject("SkillTypeText");
        RectTransform typeTextRectTransform = skillTypeTextItem.AddComponent<RectTransform>();        
        typeTextRectTransform.SetParent(skillTypeUI.transform);
        typeTextRectTransform.localScale = Vector3.one;
        Text itemTypeText = skillTypeTextItem.AddComponent<Text>();
        itemTypeText.alignment = TextAnchor.MiddleCenter;
        itemTypeText.font = font;
        itemTypeText.color = Color.black;
        itemTypeText.fontSize = 20;
        itemTypeText.fontStyle = FontStyle.Bold;
        itemTypeText.horizontalOverflow = HorizontalWrapMode.Overflow;
        itemTypeText.verticalOverflow = VerticalWrapMode.Overflow;

        items[y + (i * (maxLen))] = item.AddComponent<LevelItem>();
        items[y + (i * (maxLen))].playerNum = i;
        items[y + (i * (maxLen))].data = isblank? data[data.Length-1] : data[y + (playerNum * (maxLen))];
        items[y + (i * (maxLen))].Init();
        Navigation navigation = itemButton.navigation;
        navigation.mode = Navigation.Mode.None;
        itemButton.navigation = navigation;
        itemButton.onClick.AddListener(items[y + (i * (maxLen))].GetComponent<LevelItem>().OnClick);
        itemButton.onClick.AddListener(Hide);
    }

    public void Show(){
        Next();
        rect.localScale = Vector3.one;
        GameManager.instance.Stop();
    }
    public void Hide(){
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
    }
    void Next(){
        // 현재는 각 8개씩만 선택 가능, 8개 선택 이후 따로 최종 선택지 4개 중 1개 선택 할 수 있도록 해야함, 이후 4개 선택지 중복으로 나와야 함!

        // 1. 캐릭터 최대 2개 선정
        int ranPlayer1 = 0;
        int ranPlayer2 = 0;
        
        if(GameManager.instance.players.Length > 0){
            while(true){
                ranPlayer1 = Random.Range(0,GameManager.instance.players.Length);
                ranPlayer2 = Random.Range(0,GameManager.instance.players.Length);

                if(ranPlayer1 != ranPlayer2)
                    break;
            }
        }
        
        // 2. 모든 아이템 비활성화
        foreach(LevelItem item in items){
            item.gameObject.SetActive(false);
        }
        
        if(ranPlayer1 != ranPlayer2){

            if(ranPlayer1>ranPlayer2){
                selPlayer[1].sprite = playerSptrite[ranPlayer1].sprite;
                selPlayer[0].sprite = playerSptrite[ranPlayer2].sprite;
            } else {
                selPlayer[0].sprite = playerSptrite[ranPlayer1].sprite;
                selPlayer[1].sprite = playerSptrite[ranPlayer2].sprite;
            }

            int[] ran1 = new int[2];
            int[] ran2 = new int[2];
            
            int minIndex = ranPlayer1 * 16;
            int maxIndex = minIndex + 8;
            int skillNum = 0;
            int notUpNum = 0;
            int loopNum = 0;

            // 해당 캐릭터가 8개의 스킬을 다 배웠는가?
            for(int i=minIndex;i<maxIndex;i++){
                if(items[i].isLevelUp){
                    skillNum++;
                } else {
                    notUpNum = i;
                }
            }
            if(skillNum==8){
                minIndex += 8;
                maxIndex = minIndex + 4;

                // 해당 캐릭터가 8개 스킬 이후 4개의 스킬 중 1개라도 배웠는가?
                skillNum=0;
                for(int i=minIndex;i<maxIndex;i++){
                    LevelItem ranItem = items[i];
                    if(ranItem.isLevelUp){
                        skillNum++;
                    }
                }
                // 그렇다면 해당 캐릭터는 마지막 4개의 스킬 중 랜덤하게 나오고 레벨업 할 수 있도록 한다.
                if(skillNum>=1){
                    minIndex += 4;
                    maxIndex = minIndex + 4;
                }
            }
            // 그 중에서 각 캐릭터의 랜덤 2개 아이템 활성화
            while(true){
                ran1[0] = Random.Range(minIndex, maxIndex);
                ran1[1] = Random.Range(minIndex, maxIndex);
                if(ran1[0] != ran1[1]){
                    if(!items[ran1[0]].isLevelUp && !items[ran1[1]].isLevelUp)
                        break;
                    
                    if (skillNum==7) { // 첫번째에서 홀수의 경우
                        ran1[0] = notUpNum;
                        ran1[1] = items.Length - 1;
                        items[items.Length - 1].transform.SetSiblingIndex(items[ran1[0]].transform.GetSiblingIndex() + 1);
                        break;
                    }
                }
                if(loopNum++ > 10000)
                    throw new System.Exception("Infinite Loop");
            }
            minIndex = ranPlayer2 * 16;
            maxIndex = minIndex + 8;
            // 해당 캐릭터가 8개의 스킬을 다 배웠는가?
            skillNum = 0;
            for(int i=minIndex;i<maxIndex;i++){
                if(items[i].isLevelUp){
                    skillNum++;
                } else {
                    notUpNum = i;
                }
            }
            if(skillNum==8){
                minIndex += 8;
                maxIndex = minIndex + 4;

                // 해당 캐릭터가 8개 스킬 이후 4개의 스킬 중 1개라도 배웠는가?
                skillNum=0;
                for(int i=minIndex;i<maxIndex;i++){
                    if(items[i].isLevelUp){
                        skillNum++;
                    }
                }
                // 그렇다면 해당 캐릭터는 마지막 4개의 스킬 중 랜덤하게 나오고 레벨업 할 수 있도록 한다.
                if(skillNum>=1){
                    minIndex += 4;
                    maxIndex = minIndex + 4;
                }
            }
            loopNum = 0;
            while(true){
                ran2[0] = Random.Range(minIndex, maxIndex);
                ran2[1] = Random.Range(minIndex, maxIndex);
                if(ran2[0] != ran2[1]){
                    if(!items[ran2[0]].isLevelUp && !items[ran2[1]].isLevelUp)
                        break;

                    if (skillNum==7) { // 첫번째에서 홀수의 경우
                        ran2[0] = notUpNum;
                        ran2[1] = items.Length - 1;
                        items[items.Length - 1].transform.SetSiblingIndex(items[ran2[0]].transform.GetSiblingIndex() + 1);
                        break;
                    }
                }
                if(loopNum++ > 10000)
                    throw new System.Exception("Infinite Loop");
            }
            
            for(int index=0; index<ran1.Length;index++){
                items[ran1[index]].gameObject.SetActive(true);
            }
             for(int index=0; index<ran2.Length;index++){
                items[ran2[index]].gameObject.SetActive(true);
            }
        } else {
            // 5. 캐릭터가 하나의 경우
            selPlayer[0].sprite = playerSptrite[ranPlayer1].sprite;
            selPlayer[1].gameObject.SetActive(false);
            int minIndex = ranPlayer1 * 16;
            int maxIndex = minIndex + 7;
            int skillNum = 0;
            int notUpNum = 0;
            int loopNum = 0;

            for(int i=minIndex;i<maxIndex+1;i++){
                if(items[i].isLevelUp){
                    skillNum++;
                } else {
                    notUpNum = i;
                }
            }
            if(skillNum==8){
                minIndex += 8;
                maxIndex = minIndex + 4;

                skillNum=0;
                // 해당 캐릭터가 8개 스킬 이후 4개의 스킬 중 1개라도 배웠는가?
                for(int i=minIndex;i<maxIndex+1;i++){
                    if(items[i].isLevelUp){
                        skillNum++;
                    }
                }
                // 그렇다면 해당 캐릭터는 마지막 4개의 스킬 중 랜덤하게 나오고 레벨업 할 수 있도록 한다.
                if(skillNum>=1){
                    minIndex += 4;
                    maxIndex = minIndex + 4;
                }
            }
            int[] ran = new int[2];
            while(true){
                ran[0] = Random.Range(minIndex, maxIndex);
                ran[1] = Random.Range(minIndex, maxIndex);
                if(ran[0] != ran[1]){
                    if(!items[ran[0]].isLevelUp && !items[ran[1]].isLevelUp)
                        break;

                    if (skillNum==7) { // 첫번째에서 홀수의 경우
                        ran[0] = notUpNum;
                        ran[1] = items.Length - 1;
                        items[items.Length - 1].transform.SetSiblingIndex(items[ran[0]].transform.GetSiblingIndex() + 1);
                        break;
                    }
                }
                if(loopNum++ > 10000)
                    throw new System.Exception("Infinite Loop");
            }
            for(int index=0; index<ran.Length;index++){
                items[ran[index]].gameObject.SetActive(true);
            }
        }
    }
}
