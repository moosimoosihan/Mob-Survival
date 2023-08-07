using UnityEngine;
using Cinemachine;
using olimsko;
using UnityEngine.InputSystem;
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

        OSManager.GetService<InputManager>().GetAction("ChangeCharacter1").Enable();
        OSManager.GetService<InputManager>().GetAction("ChangeCharacter1").performed += SelectPlayer1;
        OSManager.GetService<InputManager>().GetAction("ChangeCharacter2").Enable();
        OSManager.GetService<InputManager>().GetAction("ChangeCharacter2").performed += SelectPlayer2;
        OSManager.GetService<InputManager>().GetAction("ChangeCharacter3").Enable();
        OSManager.GetService<InputManager>().GetAction("ChangeCharacter3").performed += SelectPlayer3;
        OSManager.GetService<InputManager>().GetAction("ChangeCharacter4").Enable();
        OSManager.GetService<InputManager>().GetAction("ChangeCharacter4").performed += SelectPlayer4;
        OSManager.GetService<InputManager>().GetAction("ChangeCharacterNext").Enable();
        OSManager.GetService<InputManager>().GetAction("ChangeCharacterNext").performed += NextPlayer;
        OSManager.GetService<InputManager>().GetAction("ChangeCharacterPrev").Enable();
        OSManager.GetService<InputManager>().GetAction("ChangeCharacterPrev").performed += BackPlayer;
    }
    private void OnDestroy()
    {
        OSManager.GetService<InputManager>().GetAction("ChangeCharacter1").performed -= SelectPlayer1;
        OSManager.GetService<InputManager>().GetAction("ChangeCharacter1").Disable();
        OSManager.GetService<InputManager>().GetAction("ChangeCharacter2").performed -= SelectPlayer2;
        OSManager.GetService<InputManager>().GetAction("ChangeCharacter2").Disable();
        OSManager.GetService<InputManager>().GetAction("ChangeCharacter3").performed -= SelectPlayer3;
        OSManager.GetService<InputManager>().GetAction("ChangeCharacter3").Disable();
        OSManager.GetService<InputManager>().GetAction("ChangeCharacter4").performed -= SelectPlayer4;
        OSManager.GetService<InputManager>().GetAction("ChangeCharacter4").Disable();
        OSManager.GetService<InputManager>().GetAction("ChangeCharacterNext").performed -= NextPlayer;
        OSManager.GetService<InputManager>().GetAction("ChangeCharacterNext").Disable();
        OSManager.GetService<InputManager>().GetAction("ChangeCharacterPrev").performed -= BackPlayer;
        OSManager.GetService<InputManager>().GetAction("ChangeCharacterPrev").Disable();
    }
    
    void Update()
    {        
        // 다른곳으로 빼고싶은데 처음 초기화시 이상한 곳으로 가는 현상이 있음! ㅠㅠ
        selectP.position = playerRectTransforms[chatNum].position;
    }

    public void SelectPlayer1(InputAction.CallbackContext obj)
    {
        if(GameManager.instance.players.Length > 1 && GameManager.instance.isPlay){
            if(!GameManager.instance.players[0].playerDead){
                chatNum=0;
                mainCharacter = GameManager.instance.players[chatNum].gameObject;
                CamSwitch(mainCharacter);
                ControlSwitch(mainCharacter);
            }
        }
    }
    public void SelectPlayer2(InputAction.CallbackContext obj)
    {
        if(GameManager.instance.players.Length < 2)
            return;

        if(GameManager.instance.players.Length > 1 && GameManager.instance.isPlay){
            if(!GameManager.instance.players[1].playerDead){
                chatNum=1;
                mainCharacter = GameManager.instance.players[chatNum].gameObject;
                CamSwitch(mainCharacter);
                ControlSwitch(mainCharacter);
            }
        }
    }
    public void SelectPlayer3(InputAction.CallbackContext obj)
    {
        if(GameManager.instance.players.Length < 3)
            return;
        if(GameManager.instance.players.Length > 1 && GameManager.instance.isPlay){
            if(!GameManager.instance.players[2].playerDead){
                chatNum=2;
                mainCharacter = GameManager.instance.players[chatNum].gameObject;
                CamSwitch(mainCharacter);
                ControlSwitch(mainCharacter);
            }
        }
    }
    public void SelectPlayer4(InputAction.CallbackContext obj)
    {
        if(GameManager.instance.players.Length < 4)
            return;

        if(GameManager.instance.players.Length > 1 && GameManager.instance.isPlay){
            if(!GameManager.instance.players[3].playerDead){
                chatNum=3;
                mainCharacter = GameManager.instance.players[chatNum].gameObject;
                CamSwitch(mainCharacter);
                ControlSwitch(mainCharacter);
            }
        }
    }

    public void BackPlayer(InputAction.CallbackContext obj)
    {
        if(GameManager.instance.players.Length > 1 && GameManager.instance.isPlay){
            chatNum--;
            if (chatNum < 0)
            {
                chatNum = GameManager.instance.players.Length - 1;
            }
            if (GameManager.instance.players[chatNum].playerDead)
            {
                BackPlayer(obj);
                return;
            }
            mainCharacter = GameManager.instance.players[chatNum].gameObject;
            CamSwitch(mainCharacter);
            ControlSwitch(mainCharacter);
        }
    }

    public void NextPlayer(InputAction.CallbackContext obj)
    {
        if(GameManager.instance.players.Length > 1 && GameManager.instance.isPlay){
            chatNum++;
            if (chatNum > GameManager.instance.players.Length - 1)
            {
                chatNum = 0;
            }
            if (GameManager.instance.players[chatNum].playerDead)
            {
                NextPlayer(obj);
                return;
            }
            mainCharacter = GameManager.instance.players[chatNum].gameObject;
            CamSwitch(mainCharacter);
            ControlSwitch(mainCharacter);
        }
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
    public void PlayerDeadNextPlayer()
    {
        if(GameManager.instance.players.Length > 1 && GameManager.instance.isPlay){
            chatNum++;
            if (chatNum > GameManager.instance.players.Length - 1)
            {
                chatNum = 0;
            }
            if (GameManager.instance.players[chatNum].playerDead)
            {
                PlayerDeadNextPlayer();
                return;
            }
            mainCharacter = GameManager.instance.players[chatNum].gameObject;
            CamSwitch(mainCharacter);
            ControlSwitch(mainCharacter);
        }
    }
}
