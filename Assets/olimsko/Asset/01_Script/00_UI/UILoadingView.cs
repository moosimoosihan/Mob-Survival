using UnityEngine;
using UnityEngine.UI;
using olimsko;
using UnityEngine.Events;

public class UILoadingView : UIView
{
    private LoadingContext LoadingContext => OSManager.GetService<ContextManager>().GetContext<LoadingContext>();

    [SerializeField] private UnityEvent<float> OnProgressBarChanged;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnShow()
    {
        base.OnShow();
        LoadingContext.OnSceneLoadProgressChanged += OnProgressChanged;
    }

    protected override void OnHide()
    {
        base.OnHide();
        LoadingContext.OnSceneLoadProgressChanged -= OnProgressChanged;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        LoadingContext.OnSceneLoadStart += Show;
        LoadingContext.OnSceneLoadComplete += Hide;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        LoadingContext.OnSceneLoadStart -= Show;
        LoadingContext.OnSceneLoadComplete -= Hide;
        LoadingContext.OnSceneLoadProgressChanged -= OnProgressChanged;
    }

    public void OnProgressChanged(float progress)
    {
        OnProgressBarChanged?.Invoke(progress);
    }


}