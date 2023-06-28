using UnityEngine;
using Cinemachine;
public class PlayerControl : MonoBehaviour
{
    public CinemachineVirtualCamera cinevirtual;
    public GameObject mainCharacter;
    public RectTransform selectP;
    public RectTransform[] playerRectTransforms;

    public int chatNum = 0;

    // 특정 버튼으로 플레이어 교체
    // Q, E 로 하나씩 바꿀껀지?
    // 1,2,3,4 로 원하는 캐릭터로 교체
    void Start()
    {
        mainCharacter = GameManager.instance.players[chatNum].gameObject;
        CamSwitch(mainCharacter);
        ControlSwitch(mainCharacter);
    }
    
    void Update()
    {
        if(GameManager.instance.players.Length > 1 && GameManager.instance.isPlay){ // 1명 이상일 경우 (2명 이상이라도 나머지가 죽어서 1명이 되는경우도 포함해야 함!)
            if(Input.GetKeyDown(KeyCode.E))
            { // q를 눌렀을 경우
                NextPlyaer();
            }
            else if(Input.GetKeyDown(KeyCode.Q))
            { // e 를 눌렀을 경우
                BackPlayer();
            }
            else if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectPlayer(0);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectPlayer(1);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                SelectPlayer(2);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                SelectPlayer(3);
            }
        }
        // 다른곳으로 빼고싶은데 처음 초기화시 이상한 곳으로 가는 현상이 있음! ㅠㅠ
        selectP.position = playerRectTransforms[chatNum].position;
    }

    public void SelectPlayer(int _playerNum)
    {
        if(!GameManager.instance.players[_playerNum].playerDead){
            chatNum=_playerNum;
            mainCharacter = GameManager.instance.players[chatNum].gameObject;
            CamSwitch(mainCharacter);
            ControlSwitch(mainCharacter);
        }
    }

    public void BackPlayer()
    {
        chatNum--;
        if (chatNum < 0)
        {
            chatNum = GameManager.instance.players.Length - 1;
        }
        if (GameManager.instance.players[chatNum].playerDead)
        {
            BackPlayer();
            return;
        }
        mainCharacter = GameManager.instance.players[chatNum].gameObject;
        CamSwitch(mainCharacter);
        ControlSwitch(mainCharacter);
    }

    public void NextPlyaer()
    {
        chatNum++;
        if (chatNum > GameManager.instance.players.Length - 1)
        {
            chatNum = 0;
        }
        if (GameManager.instance.players[chatNum].playerDead)
        {
            NextPlyaer();
            return;
        }
        mainCharacter = GameManager.instance.players[chatNum].gameObject;
        CamSwitch(mainCharacter);
        ControlSwitch(mainCharacter);
    }

    void CamSwitch(GameObject folplayer)
    {
        cinevirtual.Follow = folplayer.transform;
    }
    void ControlSwitch(GameObject conPlayer)
    {
        for(int i = 0;i<GameManager.instance.players.Length;i++){
            if(GameManager.instance.players[i].gameObject == conPlayer){
                GameManager.instance.players[i].inputEnabled = true;
                for(int j = 0;j<GameManager.instance.players.Length;j++){
                    GameManager.instance.players[j].GetComponent<CharacterAI>().mainCharacter = mainCharacter;
                }
            } else {
                GameManager.instance.players[i].inputEnabled = false;
            }
        }
    }
}
