using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;

namespace olimsko
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup), typeof(CanvasRenderer))]
    [RequireComponent(typeof(CanvasScaler), typeof(GraphicRaycaster))]
    public abstract class UIView : UIBehaviour, IUIView
    {
        #region SerializedField
        [Tooltip("UI가 켜졌을 때, 인터렉션 여부")]
        [SerializeField, HideInInspector] private bool m_IsDependenceInScene = false;
        [Tooltip("UI가 켜졌을 때, 인터렉션 여부")]
        [SerializeField, HideInInspector] private bool m_IsInteractable = true;
        [Tooltip("Awake에서 UI 표시 여부")]
        [SerializeField, HideInInspector] private bool m_VisibleOnAwake = false;
        [Tooltip("애니메이션 시간")]
        [SerializeField, HideInInspector] private float m_AnimTime = 0.3f;
        [Tooltip("TimeScale 설정에 애니메이션이 영향을 받는지 여부")]
        [SerializeField, HideInInspector] private bool m_IgnoreTimeScale = true;
        [Tooltip("씬 전환시 자동 숨김 여부")]
        [SerializeField, HideInInspector] private bool m_HideWhenLoading = true;
        [Tooltip("취소키 누를시 자동 숨김 여부")]
        [SerializeField, HideInInspector] private bool m_UseCancelHotKey = true;
        [Tooltip("검정 배경 사용 여부")]
        [SerializeField, HideInInspector] private bool m_UseBlackOveray = false;
        [Tooltip("검정 배경 색상")]
        [SerializeField, HideInInspector] private Color m_BlackOverayColor = new Color(0, 0, 0, 0.5f);
        [Tooltip("뒤에 있는 UI 클릭 여부")]
        [SerializeField, HideInInspector] private bool m_IsModalUI = false;
        [Tooltip("Show 시작시 이벤트")]
        [SerializeField, HideInInspector] private UnityEvent m_OnShow = default;
        [Tooltip("Show 애니메이션 완료시 이벤트")]
        [SerializeField, HideInInspector] private UnityEvent m_OnShowComplete = default;
        [Tooltip("Hide 시작시 이벤트")]
        [SerializeField, HideInInspector] private UnityEvent m_OnHide = default;
        [Tooltip("Hide 애니메이션 완료시 이벤트")]
        [SerializeField, HideInInspector] private UnityEvent m_OnHideComplete = default;


        public bool VisibleOnAwake => m_VisibleOnAwake;
        public float AnimTime => m_AnimTime;
        public bool IgnoreTimeScale => m_IgnoreTimeScale;
        public bool HideWhenLoading => m_HideWhenLoading;
        public bool UseCancelHotKey => m_UseCancelHotKey;
        public bool UseBlackOveray => m_UseBlackOveray;
        public Color BlackOverayColor => m_BlackOverayColor;
        public bool IsModalUI => m_IsModalUI;
        public UnityEvent onShow { get => m_OnShow; set => m_OnShow = value; }
        public UnityEvent onShowComplete { get => m_OnShowComplete; set => m_OnShowComplete = value; }
        public UnityEvent onHide { get => m_OnHide; set => m_OnHide = value; }
        public UnityEvent onHideComplete { get => m_OnHideComplete; set => m_OnHideComplete = value; }

        #endregion

        private Image m_BlockImage;
        private bool m_IsVisible;
        private Camera m_RenderCamera;

        protected UIManager UIManager { get; set; }
        protected ContextManager Context { get; set; }
        protected RectTransform RectTransform => (RectTransform)transform;
        protected virtual CanvasGroup CanvasGroup { get; private set; }

        public bool IsDependenceInScene => m_IsDependenceInScene;
        public bool IsVisible { get => m_IsVisible; set => m_IsVisible = value; }
        public bool IsInteractable { get => m_IsInteractable; set => m_IsInteractable = value; }
        public Camera RenderCamera { get => m_RenderCamera; set { m_RenderCamera = value; GetComponent<Canvas>().worldCamera = value; } }
        public GameObject GameObject { get => this.gameObject; }

        private Dictionary<string, IUIBindable> m_DicBind = new Dictionary<string, IUIBindable>();
        private Dictionary<string, IUIViewEntity> m_DicEntity = new Dictionary<string, IUIViewEntity>();

        public event Action<bool> OnVisibilityChanged;

        private void BindingObject()
        {
            IUIBindable[] uIDatas = this.GetComponentsInChildren<IUIBindable>(true);

            foreach (var item in uIDatas)
            {
                if (item.IsBind)
                {
                    if (!m_DicBind.ContainsKey(item.BindPath))
                        m_DicBind.Add(item.BindPath, item);
                }
            }
        }

        public void AddEntity(IUIViewEntity entity)
        {
            if (!m_DicEntity.ContainsKey(entity.Name))
                m_DicEntity.Add(entity.Name, entity);
        }

        public void RemoveEntity(IUIViewEntity entity)
        {
            if (m_DicEntity.ContainsKey(entity.Name))
                m_DicEntity.Remove(entity.Name);
        }

        protected T GetEntity<T>(string name) where T : IUIViewEntity
        {
            return (T)m_DicEntity[name];
        }

        protected T Get<T>(string name) where T : IUIBindable
        {
            return (T)m_DicBind[name];
        }

        protected override void Awake()
        {
            base.Awake();

            BindingObject();
            UIManager = OSManager.GetService<UIManager>();

            if (IsDependenceInScene) UIManager.AddUI(new ManagedUIView(this.name, this.gameObject, this));
            Context = OSManager.GetService<ContextManager>();

            CanvasGroup = GetComponent<CanvasGroup>();

            m_BlockImage = new GameObject().AddComponent<Image>();

            m_BlockImage.transform.SetParent(this.transform);
            m_BlockImage.transform.SetAsFirstSibling();
            m_BlockImage.raycastTarget = m_IsModalUI;
            m_BlockImage.color = m_UseBlackOveray ? m_BlackOverayColor : new Color(0, 0, 0, 0);
            m_BlockImage.rectTransform.offsetMin = Vector2.zero;
            m_BlockImage.rectTransform.offsetMax = Vector2.zero;
            m_BlockImage.rectTransform.anchorMin = Vector2.zero;
            m_BlockImage.rectTransform.anchorMax = Vector2.one;

            CanvasGroup.interactable = IsInteractable;
            CanvasGroup.blocksRaycasts = IsInteractable;

            if (VisibleOnAwake)
            {
                IsVisible = false;
                ChangeVisibility(true, 0);
            }
            else
            {
                IsVisible = true;
                ChangeVisibility(false, 0);
            }
        }

        protected override void Start()
        {
            if (VisibleOnAwake)
            {
                IsVisible = false;
                ChangeVisibility(true, 0);
            }
            else
            {
                IsVisible = true;
                ChangeVisibility(false, 0);
            }
        }

        protected override void OnDestroy()
        {
            Hide();
            base.OnDestroy();
            if (IsDependenceInScene) UIManager.RemoveUI(this);
        }

        protected virtual void ChangeVisibility(bool visible, float? duration = null)
        {
            if (DOTween.IsTweening(CanvasGroup))
            {
                DOTween.Kill(CanvasGroup);
            }

            if (m_UseCancelHotKey && !IsVisible && visible) UIManager.EnQueueAutoHideUI(this);
            if (m_UseCancelHotKey && IsVisible && !visible) UIManager.DeQueueAutoHideUI(this);

            IsVisible = visible;

            OnVisibleEventTrigger(visible);

            if (!CanvasGroup) return;

            if (IsInteractable)
            {
                CanvasGroup.interactable = visible;
                CanvasGroup.blocksRaycasts = visible;
            }

            float animDuration = duration ?? AnimTime;
            float targetAlpha = visible ? 1 : 0;

            CanvasGroup.DOFade(targetAlpha, animDuration).SetUpdate(m_IgnoreTimeScale).OnComplete(() => OnVisibleCompleteEventTrigger(visible));

        }

        private void OnVisibleEventTrigger(bool visible)
        {
            if (visible) OnShow();
            else OnHide();
        }

        private void OnVisibleCompleteEventTrigger(bool visible)
        {
            if (visible) OnShowComplete();
            else OnHideComplete();
            OnVisibilityChanged?.Invoke(visible);
        }

        protected virtual void OnShow()
        {
            onShow?.Invoke();

        }

        protected virtual void OnHide()
        {
            onHide?.Invoke();

        }

        protected virtual void OnShowComplete()
        {
            onShowComplete?.Invoke();
        }

        protected virtual void OnHideComplete()
        {
            onHideComplete?.Invoke();
        }

        public void Show()
        {
            if (IsVisible) return;
            ChangeVisibility(true);
        }

        public void Hide()
        {
            if (!IsVisible) return;
            ChangeVisibility(false);
        }

        public void DestroyThis()
        {
            Destroy(this.gameObject);
        }
    }
}
