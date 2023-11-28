using UnityEngine;
using UnityEngine.UI;
using olimsko;

public class UIStageView : UIView
{
    StageContext StageContext => OSManager.GetService<ContextManager>().GetContext<StageContext>();

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

    public void OnClickSelectChapter(int chapter)
    {
        StageContext.SelectedChapter = chapter;
        OSManager.GetService<UIManager>().GetUI<UISelectStageView>().Show();
    }

    public void OnClickUpgrade()
    {
        OSManager.GetService<UIManager>().GetUI<UIUpgradeView>().Show();
    }
}