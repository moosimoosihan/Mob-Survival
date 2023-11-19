using UnityEngine;
using UnityEngine.UI;
using olimsko;
using DG.Tweening;

public class UIIntroView : UIView
{
    [SerializeField] private float m_FadeDuration = 2;
    [SerializeField] private float m_FadeDelay = 2;
    private Sequence m_Sequence;

    protected override void Awake()
    {
        base.Awake();
        AudioManager.Instance.Init();
    }

    protected override void OnShow()
    {
        base.OnShow();

        m_Sequence = DOTween.Sequence();
        m_Sequence.Append(Get<UIImage>("Logo").DOFade(1, m_FadeDuration));
        m_Sequence.AppendInterval(m_FadeDelay);
        m_Sequence.Append(Get<UIImage>("Logo").DOFade(0, m_FadeDuration));
        m_Sequence.OnComplete(() =>
        {
            if (OSManager.IsInitialized)
            {
                OSManager.GetService<ContextManager>().GetContext<LoadingContext>().LoadSceneAsyncTrigger("01_Main");
            }
            else
            {
                OSManager.OnInitializeComplete += () =>
                {
                    OSManager.GetService<ContextManager>().GetContext<LoadingContext>().LoadSceneAsyncTrigger("01_Main");
                };
            }
        });

    }

    protected override void OnHide()
    {
        base.OnHide();
    }

}