using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using olimsko;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public InputManager inputManager => OSManager.GetService<InputManager>();

    [Header("게임 컨트롤")]
    public float gameTime;
    public bool isPlay;
    public float curTimeScale = 1;

    [Header("플레이어 정보")]    
    public int level;
    public int kill;
    public int bossKill;
    public float exp;
    public int gold;
    public int[] nextExp;
    public int life;
    [SerializeField]
    TextAsset playerDatabase;
    List<playerData> playerDataList = new List<playerData>();
    [SerializeField]
    TextAsset expDatabase;

    [Header("게임 오브젝트")]
    public Player[] players;
    public GameObject playerDummies;
    public PoolManager pool;
    public PlayerControl playerControl;
    public ItemManager itemManager;
    public LevelUp uiLevelUp;
    public TextMeshProUGUI timeText;
    [Header("UI Sprite")]
    public GameObject gameOverObj;
    public Sprite[] playerSptrite;
    public GameObject pauseObj;

    // 임시
    [Header("이펙트 프리펩")]
    public GameObject burnEffect;

    void Awake()
    {
        Application.targetFrameRate = 60;
        instance = this;

        // 플레이어 묶음에서 현재 있는 플레이어 갯수를 가져오기
        players = playerDummies.GetComponentsInChildren<Player>();

        //플레이어 데이터 불러오기
        string seperator = "\r\n";
        string[] lines = playerDatabase.text.Substring(0).Split(seperator);
        for (int i = 0; i < lines.Length; i++)
        {
            string[] rows = lines[i].Split('\t');

            //아이템 생성
            playerData tempPlayerData = new playerData();
            tempPlayerData.damage = System.Convert.ToSingle(rows[0]);
            tempPlayerData.critRate = System.Convert.ToSingle(rows[1]);
            tempPlayerData.critDamage = System.Convert.ToSingle(rows[2]);
            tempPlayerData.attSpeed = System.Convert.ToSingle(rows[3]);
            tempPlayerData.attRange = System.Convert.ToSingle(rows[4]);
            tempPlayerData.heal = System.Convert.ToSingle(rows[5]);
            tempPlayerData.maxHP = System.Convert.ToInt32(rows[6]);
            tempPlayerData.def = System.Convert.ToInt32(rows[7]);
            tempPlayerData.hpRegen = System.Convert.ToInt32(rows[8]);
            tempPlayerData.evasion = System.Convert.ToSingle(rows[9]);
            tempPlayerData.vamp = System.Convert.ToSingle(rows[10]);
            tempPlayerData.moveSpeed = System.Convert.ToInt32(rows[11]);
            tempPlayerData.damageReduction = System.Convert.ToSingle(rows[12]);
            tempPlayerData.elementalDamage = System.Convert.ToSingle(rows[13]);
            tempPlayerData.activeDamage = System.Convert.ToSingle(rows[14]);
            tempPlayerData.activeCooldown = System.Convert.ToSingle(rows[15]);
            tempPlayerData.numberOfProjectile = System.Convert.ToInt32(rows[16]);
            tempPlayerData.projectilePenetration = System.Convert.ToInt32(rows[17]);

            playerDataList.Add(tempPlayerData);
        }

        // 임시로 플레이어 스탯 부여
        for(int i=0;i<players.Length;i++){
            if(players[i].chacter==Player.characterInfo.용사){
                players[i].Init(playerDataList[0]);
            } else if(players[i].chacter==Player.characterInfo.궁수){
                players[i].Init(playerDataList[1]);
            } else if(players[i].chacter==Player.characterInfo.현자){
                players[i].Init(playerDataList[2]);
            } else if(players[i].chacter==Player.characterInfo.사제){
                players[i].Init(playerDataList[3]);
            }
        }

        string[] expLines = expDatabase.text.Substring(0).Split(seperator);
        nextExp = new int[expLines.Length];
        for(int i=0;i<expLines.Length;i++){
            nextExp[i] = System.Convert.ToInt32(expLines[i]);
        }
    }
    
    void Start()
    {
        isPlay = true;
        inputManager.GetAction("UsePotion").Enable();
        inputManager.GetAction("UsePotion").performed += RevivalPotion;
    }
    private void OnDestroy()
    {
        inputManager.GetAction("UsePotion").performed -= RevivalPotion;
        inputManager.GetAction("UsePotion").Disable();
    }

    void Update()
    {
        gameTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
            Pause();
    }

    private void RevivalPotion(InputAction.CallbackContext obj)
    {
        if (isPlay)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].playerDead)
                {
                    if (life > 0)
                    {
                        life--;
                        // 죽은 플레이어 살리기
                        players[i].Revival();
                        if (Vector3.Distance(playerControl.mainCharacter.transform.position, players[i].transform.position) > players[i].gameObject.GetComponent<CharacterAI>().distWithinMainCharacter)
                        {
                            // 멀리서 부활했을 경우 플레이어 이동
                            players[i].transform.position = playerControl.mainCharacter.transform.position;
                        }
                        break;
                    }
                }
            }
        }
    }

    //경험치 획득
    public void GetExp(int amount)
    {
        exp+=amount;

        if(exp >= nextExp[level]){
            exp -= nextExp[level];
            level++;
            uiLevelUp.Show();
        }
    }
    public void GetGold(int amount)
    {
        gold+=amount;
    }
    public void Stop(){
        isPlay = false;
        foreach(Player player in players){
            player.inputVec = Vector2.zero;
        }
        Time.timeScale = 0;
    }
    public void Resume(){
        isPlay = true;
        Time.timeScale = curTimeScale;
    }
    public void Pause(){
        if(pauseObj.activeSelf)
        {
            pauseObj.SetActive(false);
            Resume();
        }
        else
        {
            pauseObj.SetActive(true);
            Stop();
        }
    }
    public void Replay(){
        Time.timeScale = 1;
        SceneManager.LoadScene("main");
    }
    public void GameExit(){
        Time.timeScale = 1;
        SceneManager.LoadScene("01_Main");
    }
    public void GameEnd(){
        Application.Quit();
    }
    public void GameSpeedUp(){
        if(!isPlay)
            return;

        if(Time.timeScale==1){
            curTimeScale=1.5f;
            timeText.text = "x1.5";
        } else if(Time.timeScale==1.5f){
            curTimeScale=2;
            timeText.text = "x2";
        } else if(Time.timeScale==2){
            curTimeScale=1;
            timeText.text = "x1";
        }
        Time.timeScale = curTimeScale;
    }
}
[System.Serializable]
public class playerData
{
    public enum characterInfo { 용사, 궁수, 현자, 사제 }
    public characterInfo chacter;
    public float maxHP;
    public float moveSpeed;
    public float damage;
    public float critRate;
    public float critDamage;
    public float attSpeed;
    public float attRange;
    public float heal;
    public int def;
    public int hpRegen;
    public float evasion;
    public float vamp;
    public float damageReduction;
    public float elementalDamage;
    public float activeDamage;
    public float activeCooldown;
    public int numberOfProjectile;
    public int projectilePenetration;
}