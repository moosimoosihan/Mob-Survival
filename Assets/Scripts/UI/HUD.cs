using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoYType { Exp, Level, Kill, Time, Health, Gold, Life, BossKill, ActiveSkill }
    public InfoYType type;
    public int playerNum;

    Text myText;
    Slider mySlider;
    Image myImage;

    void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
        myImage = GetComponent<Image>();
    }

    void LateUpdate()
    {
        switch(type){
            case InfoYType.Exp:
                float curExp = GameManager.instance.exp;
                float maxExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level,GameManager.instance.nextExp.Length-1)];
                mySlider.value = curExp/maxExp;
                break;
            case InfoYType.Level:
                myText.text = string.Format("Lv.{0:F0}",GameManager.instance.level + 1);
                break;
            case InfoYType.Kill:
                myText.text = string.Format("{0:F0}",GameManager.instance.kill);
                break;
            case InfoYType.BossKill:
                myText.text = string.Format("{0:F0}/4",GameManager.instance.BossKill);
                break;
            case InfoYType.Time:
                float remainTime = GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}",min, sec);
                break;
            case InfoYType.Health:
                float curHealth = GameManager.instance.players[playerNum].curHP;
                float maxHealth = GameManager.instance.players[playerNum].maxHP;
                mySlider.value = curHealth/maxHealth;
                break;
            case InfoYType.Gold:
                myText.text = string.Format("{0:F0}",GameManager.instance.gold);
                break;
            case InfoYType.Life:
                myText.text = string.Format("{0:F0}",GameManager.instance.life);
                break;
            case InfoYType.ActiveSkill:
                myImage.fillAmount = GameManager.instance.players[playerNum].GetComponentInChildren<ActiveSkill>().timer/GameManager.instance.players[playerNum].GetComponentInChildren<ActiveSkill>().delay;
                break;
        }
    }
}
