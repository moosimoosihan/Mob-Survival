using UnityEngine;
using UnityEngine.UI;
using olimsko;

public class UISelectHeroView : UIView
{
    // Add your UI logic here
    protected override void Awake()
    {
        base.Awake();

        Get<UIButton>("StartButton").onClick.AddListener(() =>
        {
            OSManager.GetService<ContextManager>().GetContext<LoadingContext>().LoadSceneAsyncTrigger("main");
        });
    }

    protected override void OnShow()
    {
        base.OnShow();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }



}