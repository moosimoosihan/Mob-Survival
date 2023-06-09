using UnityEngine;
using UnityEngine.UI;
using olimsko;

public class UISelectStageView : UIView
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

    protected override void OnHide()
    {
        base.OnHide();
    }

    public void OnShowSelectHero()
    {
        OSManager.GetService<UIManager>().GetUI<UISelectHeroView>().Show();
    }
}