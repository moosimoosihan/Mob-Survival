using UnityEngine;
using UnityEngine.UI;
using olimsko;
using Cysharp.Threading.Tasks;

public class UISettingView : UIView
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
        OSManager.GetService<StateManager>().SaveSettingsAsync().Forget();
    }

}