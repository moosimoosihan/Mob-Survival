using UnityEngine;
using DG.Tweening;

namespace olimsko
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIViewEntity : UIBase, IUIViewEntity
    {
        private UIView m_View;
        private CanvasGroup m_CanvasGroup;
        [SerializeField] protected float m_AnimTime = 0.3f;
        [SerializeField] private bool m_IsVisibleOnAwake = true;
        [SerializeField] private bool m_IgnoreTimeScale = true;
        [SerializeField] private bool m_UseActiveWhenVisible = false;

        public virtual string Name => name;
        public T GetView<T>() where T : UIView
        {
            if (m_View == null)
                m_View = GetComponentInParent<UIView>();

            return (T)m_View;
        }

        public UIView View
        {
            get
            {
                if (m_View == null)
                    m_View = GetComponentInParent<UIView>();

                return m_View;
            }
        }

        public CanvasGroup CanvasGroup
        {
            get
            {
                if (m_CanvasGroup == null)
                {
                    m_CanvasGroup = GetComponent<CanvasGroup>();
                }
                return m_CanvasGroup;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            View.AddEntity(this);

            View.onHide.AddListener(Hide);

            if (m_IsVisibleOnAwake)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        protected virtual void OnDestroy()
        {
            View.onHide.RemoveListener(Hide);
            View.RemoveEntity(this);
        }

        public virtual void Show()
        {
            CanvasGroup.DOFade(1, m_AnimTime).SetUpdate(m_IgnoreTimeScale);
            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.interactable = true;
            OnShow();
        }

        protected virtual void OnShow()
        {
            if (m_UseActiveWhenVisible)
                gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            CanvasGroup.DOFade(0, m_AnimTime).SetUpdate(m_IgnoreTimeScale);
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.interactable = false;

            OnHide();
        }

        protected virtual void OnHide()
        {
            if (m_UseActiveWhenVisible)
                gameObject.SetActive(false);
        }

        public void Visible(bool visible)
        {
            if (visible)
                Show();
            else
                Hide();
        }




    }
}