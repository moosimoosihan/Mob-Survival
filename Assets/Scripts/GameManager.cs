using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using olimsko;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public InputManager inputManager => OSManager.GetService<InputManager>();
    private List<CharacterTable> CharacterData => OSManager.GetService<DataManager>().GetData<CharacterTableSO>().CharacterTable;

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
    List<playerData> playerDataList = new List<playerData>();
    [SerializeField]
    TextAsset expDatabase;
    public GameObject[] playerPrefs;

    [Header("게임 오브젝트")]
    public Player[] players;
    public GameObject playerDummies;
    public GameObject pool;
    public PlayerControl playerControl;
    public ItemManager itemManager;
    public LevelUp uiLevelUp;
    public TextMeshProUGUI timeText;
    [Header("UI Sprite")]
    public GameObject gameOverObj;
    public Sprite[] playerSptrite;
    public Sprite[] playerSkillSprite;
    public GameObject pauseObj;
    public GameObject[] playerUI;

    // 임시
    [Header("이펙트 프리펩")]
    public GameObject burnEffect;

    private StageContext StageContext => OSManager.GetService<ContextManager>().GetContext<StageContext>();

    void Awake()
    {
        Application.targetFrameRate = 60;
        instance = this;

        // 플레이어 묶음에서 현재 있는 플레이어 갯수를 가져오기
        players = playerDummies.GetComponentsInChildren<Player>();

        string seperator = "\r\n";

        //경험치 데이터 불러오기
        string[] expLines = expDatabase.text.Substring(0).Split(seperator);
        nextExp = new int[expLines.Length];
        for(int i=0;i<expLines.Length;i++){
            nextExp[i] = System.Convert.ToInt32(expLines[i]);
        }
        CharacterInit();
        uiLevelUp.Init();
    }
    // asycn 필요함
    void CharacterInit()
    {
        // 플레이어 선택 화면에서 선택한 캐릭터 정보 가져오기
        players = new Player[StageContext.ListSelectedHero.Count];
        for(int i=0;i<StageContext.ListSelectedHero.Count;i++){
            Player player = Instantiate(playerPrefs[StageContext.ListSelectedHero[i]]).GetComponent<Player>();
            player.transform.SetParent(playerDummies.transform);
            players[i] = player;
            players[i].Init(CharacterData[StageContext.ListSelectedHero[i]]);
            playerUI[i].SetActive(true);
            playerUI[i].GetComponentsInChildren<Image>()[2].sprite = playerSptrite[StageContext.ListSelectedHero[i]];
            playerUI[i].GetComponentsInChildren<Image>()[6].sprite = playerSkillSprite[StageContext.ListSelectedHero[i]];
            playerUI[i].GetComponentsInChildren<Image>()[7].sprite = playerSkillSprite[StageContext.ListSelectedHero[i]];
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
    // 리플레이 혹은 메인화면으로 돌아갈 시 singleton 초기화 해야함
    public void Replay(){
        Pause();
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
            curTimeScale=3;
            timeText.text = "x3";
        } else if(Time.timeScale==3){
            curTimeScale=5;
            timeText.text = "x5";
        } else if(Time.timeScale==5){
            curTimeScale=1;
            timeText.text = "x1";
        }
        Time.timeScale = curTimeScale;
    }
}
[System.Serializable]
public class playerData
{
    public string character;
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