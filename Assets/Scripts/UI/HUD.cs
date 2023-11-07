using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class HUD : MonoBehaviour
{
    public enum InfoYType { Exp, Level, Kill, Time, Health, Gold, Life, BossKill, ActiveSkill, Sheild, FPS }
    public InfoYType type;
    public int playerNum;

    TextMeshProUGUI myText;
    TextMeshProUGUI myChildText;
    Slider mySlider;
    Image myImage;
    float timer;

    // 이전 값 저장 변수
    float p_Exp;
    int p_Level;
    int p_Kill;
    int p_TimeMin;
    int p_TimeSec;
    float p_curHealth;
    float p_maxHealth;
    int p_Gold;
    int p_Life;
    int p_BossKill;
    float p_curActiveSkill;
    float p_maxActiveSkill;
    float p_curSheild;
    float p_maxSheild;
    float p_FPS;
    [SerializeField] private bool m_UseAnimation = true;
    [SerializeField] private float m_AnimationTime = 0.5f;

    void Awake()
    {
        myText = GetComponent<TextMeshProUGUI>();
        myChildText = GetComponentInChildren<TextMeshProUGUI>();
        mySlider = GetComponent<Slider>();
        myImage = GetComponent<Image>();
    }

    void Start()
    {

        switch (type)
        {
            case InfoYType.Exp:
                mySlider.value = p_Exp;
                break;
            case InfoYType.Level:
                myText.text = string.Format("Lv.{0:F0}", p_Level + 1);
                break;
            case InfoYType.Kill:
                myText.text = string.Format("{0:F0}", p_Kill);
                break;
            case InfoYType.BossKill:
                myText.text = string.Format("{0:F0}/4", p_BossKill);
                break;
            case InfoYType.Time:
                myText.text = string.Format("{0:D2}:{1:D2}", p_TimeMin, p_TimeSec);
                break;
            case InfoYType.Health:
                if (GameManager.instance.players.Length < playerNum)
                {
                    gameObject.SetActive(false);
                    return;
                }
                float curHealth = GameManager.instance.players[playerNum].CurHP;
                float maxHealth = GameManager.instance.players[playerNum].MaxHP;
                p_curHealth = curHealth;
                p_maxHealth = maxHealth;
                mySlider.value = curHealth / maxHealth;
                if (myChildText != null)
                {
                    myChildText.text = curHealth.ToString() + "/" + maxHealth.ToString();
                }
                break;
            case InfoYType.Gold:
                p_Gold = GameManager.instance.gold;
                myText.text = string.Format("{0:F0}", p_Gold);
                break;
            case InfoYType.Life:
                p_Life = GameManager.instance.life;
                myText.text = string.Format("x {0:F0}", p_Life);
                break;
            case InfoYType.ActiveSkill:
                if (GameManager.instance.players.Length < playerNum)
                {
                    gameObject.SetActive(false);
                    return;
                }
                float curTimer = GameManager.instance.players[playerNum].GetComponentInChildren<ActiveSkill>().timer;
                float maxTimer = GameManager.instance.players[playerNum].GetComponentInChildren<ActiveSkill>().delay;
                p_curActiveSkill = curTimer;
                p_maxActiveSkill = maxTimer;
                myImage.fillAmount = curTimer / maxTimer;
                break;
            case InfoYType.Sheild:
                if (GameManager.instance.players.Length < playerNum)
                {
                    gameObject.SetActive(false);
                    return;
                }
                if (GameManager.instance.players[playerNum].CurShield > 0 || GameManager.instance.players[playerNum].MaxShield > 0)
                {
                    float curShield = GameManager.instance.players[playerNum].CurShield;
                    float maxShield = GameManager.instance.players[playerNum].MaxShield;
                    p_curSheild = curShield;
                    p_maxSheild = maxShield;
                    mySlider.value = curShield / maxShield;
                    if (myChildText != null)
                    {
                        myChildText.text = curShield.ToString() + "/" + maxShield.ToString();
                    }
                }
                else
                {
                    GameManager.instance.players[playerNum].CurShield = 0;
                    GameManager.instance.players[playerNum].MaxShield = 0;
                    p_curSheild = 0;
                    p_maxSheild = 0;
                    mySlider.value = 0;
                    if (myChildText != null)
                    {
                        myChildText.text = "0/0";
                    }
                }
                break;
        }
    }

    void LateUpdate()
    {
        switch (type)
        {
            case InfoYType.Exp:
                float curExp = GameManager.instance.exp;
                float maxExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1)];
                if (curExp / maxExp != p_Exp)
                {
                    p_Exp = curExp / maxExp;
                    mySlider.value = p_Exp;
                }
                break;
            case InfoYType.Level:
                if (GameManager.instance.level != p_Level)
                {
                    p_Level = GameManager.instance.level;
                    myText.text = string.Format("Lv.{0:F0}", p_Level + 1);
                }
                break;
            case InfoYType.Kill:
                if (GameManager.instance.kill != p_Kill)
                {
                    p_Kill = GameManager.instance.kill;
                    myText.text = string.Format("{0:F0}", p_Kill);
                }
                break;
            case InfoYType.BossKill:
                if (GameManager.instance.bossKill != p_BossKill)
                {
                    p_BossKill = GameManager.instance.bossKill;
                    myText.text = string.Format("{0:F0}/4", p_BossKill);
                }
                break;
            case InfoYType.Time:
                float remainTime = GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                if (p_TimeMin != min || p_TimeSec != sec)
                {
                    p_TimeMin = min;
                    p_TimeSec = sec;
                    myText.text = string.Format("{0:D2}:{1:D2}", p_TimeMin, p_TimeSec);
                }
                break;
            case InfoYType.Health:

                float curHealth = GameManager.instance.players[playerNum].CurHP;
                float maxHealth = GameManager.instance.players[playerNum].MaxHP;
                curHealth = Mathf.FloorToInt(curHealth);
                maxHealth = Mathf.FloorToInt(maxHealth);
                if (p_curHealth != curHealth || p_maxHealth != maxHealth)
                {
                    p_curHealth = curHealth;
                    p_maxHealth = maxHealth;
                    mySlider.value = curHealth / maxHealth;
                    if (myChildText != null)
                    {
                        myChildText.text = curHealth.ToString() + "/" + maxHealth.ToString();
                    }
                }
                break;
            case InfoYType.Gold:
                if (p_Gold != GameManager.instance.gold)
                {
                    p_Gold = GameManager.instance.gold;
                    if (m_UseAnimation)
                    {
                        myText.DOCounter(int.Parse(myText.text.Replace(",", "")), p_Gold, m_AnimationTime);
                    }
                    else
                    {
                        myText.text = string.Format("{0:F0}", p_Gold);
                    }
                }
                break;
            case InfoYType.Life:
                if (p_Life != GameManager.instance.life)
                {
                    p_Life = GameManager.instance.life;
                    myText.text = string.Format("x {0:F0}", p_Life);
                }
                break;
            case InfoYType.ActiveSkill:
                float curTimer = GameManager.instance.players[playerNum].GetComponentInChildren<ActiveSkill>().timer;
                float maxTimer = GameManager.instance.players[playerNum].GetComponentInChildren<ActiveSkill>().delay;
                if (p_curActiveSkill != curTimer || p_maxActiveSkill != maxTimer)
                {
                    p_curActiveSkill = curTimer;
                    p_maxActiveSkill = maxTimer;
                    myImage.fillAmount = curTimer / maxTimer;
                }
                break;
            case InfoYType.Sheild:
                if (GameManager.instance.players[playerNum].CurShield > 0 && GameManager.instance.players[playerNum].MaxShield > 0)
                {
                    float curShield = GameManager.instance.players[playerNum].CurShield;
                    float maxShield = GameManager.instance.players[playerNum].MaxShield;
                    curShield = Mathf.FloorToInt(curShield);
                    maxShield = Mathf.FloorToInt(maxShield);
                    if (p_curSheild != curShield || p_maxSheild != maxShield)
                    {
                        p_curSheild = curShield;
                        p_maxSheild = maxShield;
                        mySlider.value = curShield / maxShield;
                        if (myChildText != null)
                        {
                            myChildText.text = curShield.ToString() + "/" + maxShield.ToString();
                        }
                    }
                }
                else
                {
                    GameManager.instance.players[playerNum].CurShield = 0;
                    GameManager.instance.players[playerNum].MaxShield = 0;
                    p_curSheild = 0;
                    p_maxSheild = 0;
                    mySlider.value = 0;
                    if (myChildText != null)
                    {
                        myChildText.text = "0/0";
                    }
                }
                break;
            case InfoYType.FPS:
                timer += Time.deltaTime;
                if (timer > 0.5f)
                {
                    if (p_FPS != 1 / Time.deltaTime)
                    {
                        p_FPS = 1 / Time.deltaTime;
                        myText.text = string.Format("FPS : {0:F0}", 1 / Time.deltaTime);
                    }
                    timer = 0;
                }
                break;
        }
    }
}
