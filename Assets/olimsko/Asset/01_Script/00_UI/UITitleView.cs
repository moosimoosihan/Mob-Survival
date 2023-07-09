using UnityEngine;
using UnityEngine.UI;
using olimsko;

public class UITitleView : UIView
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

    public void OnShowSettingView()
    {
        Hide();
        OSManager.GetService<UIManager>().GetUI<UISettingView>().Show();
    }

    public void OnShowStageView()
    {
        OSManager.GetService<UIManager>().GetUI<UIStageView>().Show();
    }

    public void OnShowCreditView()
    {
        OSManager.GetService<UIManager>().GetUI<UICreditView>().Show();
    }

    public async void OnShowExitGamePopUp()
    {
        Hide();
        if (!await OSManager.GetService<UIManager>().GetUI<UIExitPopUpView>().ConfirmAsync(""))
        {
            Show();
            return;
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); 
#endif
    }
}