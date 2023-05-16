using UnityEngine;
using UnityEngine.UI;

public class HealthFollow : MonoBehaviour
{
    GameObject targetObj;
    RectTransform rect;
    float curSliderValue;
    Slider fillSlider;

    CharacterStatus characterStatus;

    public void Init(GameObject _followObj)
    {
        targetObj = _followObj;
        rect = GetComponent<RectTransform>();
        fillSlider = transform.GetChild(0).GetComponent<Slider>();
        characterStatus = targetObj.transform.GetComponent<CharacterStatus>();
    }

    void Update()
    {
        rect.position = Camera.main.WorldToScreenPoint(targetObj.transform.position);

        curSliderValue = characterStatus.curHP / characterStatus.maxHP;
        fillSlider.value = curSliderValue;

        if (curSliderValue <= 0)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);

    }
}