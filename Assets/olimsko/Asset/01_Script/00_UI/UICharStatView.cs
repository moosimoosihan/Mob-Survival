using UnityEngine;
using UnityEngine.UI;
using olimsko;

public class UICharStatView : UIView
{
    // Add your UI logic here
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnShow()
    {
        base.OnShow();
    }

    public void Show(RectTransform rectTransform)
    {
        base.Show();
        transform.position = rectTransform.position;
    }


    protected override void OnHide()
    {
        base.OnHide();
    }

}