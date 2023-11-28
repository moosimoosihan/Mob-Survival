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
        int stageIdx = Get<UIToggle>("Stage1").isOn ? 1 : Get<UIToggle>("Stage2").isOn ? 2 : 3;
        OSManager.GetService<ContextManager>().GetContext<StageContext>().SelectedStage = stageIdx;

        int difficulty = Get<UIToggle>($"Stage{stageIdx}_Normal").isOn ? 1 : Get<UIToggle>($"Stage{stageIdx}_Hard").isOn ? 2 : 3;
        OSManager.GetService<ContextManager>().GetContext<StageContext>().SelectedDifficulty = difficulty;

        OSManager.GetService<UIManager>().GetUI<UISelectHeroView>().Show();
    }

}