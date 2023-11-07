using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

namespace olimsko
{
    [InitializeAtRuntime]
    public class UIManager : IOSMEntity<UIConfiguration>
    {
        private readonly List<ManagedUIView> m_ListManagedUIView = new List<ManagedUIView>();
        private readonly Dictionary<Type, IUIView> m_DicGetUIResults = new Dictionary<Type, IUIView>();
        private List<IUIView> m_ListHideUI = new List<IUIView>();

        public UIConfiguration Configuration { get; }

        public bool UseHideHotKey { get; set; } = true;
        public KeyCode HideHotKeyCode { get; set; } = default;

        private static UniTaskCompletionSource m_InitializeUI = new UniTaskCompletionSource();
        public static bool IsInitialized => m_InitializeUI.Task.Status == UniTaskStatus.Succeeded;

        private UIConfiguration m_UIConfiguration;

        public UIManager(UIConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void DestroyThis()
        {

        }

        public async UniTask InitializeAsync()
        {
            m_InitializeUI = new UniTaskCompletionSource();

            m_UIConfiguration = ConfigurationProvider.LoadOrDefault<UIConfiguration>();

            UseHideHotKey = m_UIConfiguration.UseHideHotKey;
            HideHotKeyCode = m_UIConfiguration.HideHotKeyCode;

            Camera uICamera = null;
            if (m_UIConfiguration.CustomUICamera != null)
            {
                uICamera = OSManager.Instantiate(m_UIConfiguration.CustomUICamera, m_UIConfiguration.CustomUICamera.name, Configuration.OverrideObjectLayer ? (int?)Configuration.ObjectLayer : null).GetComponent<Camera>();
            }

            for (int i = 0; i < m_UIConfiguration.ListPrefabs.Count; i++)
            {
                var uIView = InstantiatePrefab(m_UIConfiguration.ListPrefabs[i].Prefab, m_UIConfiguration.ListPrefabs[i].Name);

                uIView.GameObject.GetComponent<Canvas>().renderMode = m_UIConfiguration.UIRenderMode;
                uIView.RenderCamera = uICamera;
            }

            if (m_UIConfiguration.AutoSpawnInputSystem)
            {
                if (BaseUtil.IsValid(m_UIConfiguration.CustomInputSystem))
                    OSManager.Instantiate(m_UIConfiguration.CustomInputSystem).transform.SetParent(((RuntimeBehaviour)OSManager.Behaviour).transform, false);
                else
                {
#if ENABLE_INPUT_SYSTEM && INPUT_SYSTEM_AVAILABLE
                    GameObject go = ((RuntimeBehaviour)OSManager.Behaviour).AddNewChildObject();
                    var inputModule = go.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                    inputModule.AssignDefaultActions();
#else
                    GameObject go = ((RuntimeBehaviour)OSManager.Behaviour).AddNewChildObject();
                    var inputModule = go.AddComponent<StandaloneInputModule>();
#endif
                    await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
                    inputModule.enabled = false; // Otherwise it stops processing UI events when using new input system.
                    inputModule.enabled = true;
                }
            }

            OSManager.Behaviour.OnBehaviourUpdate += Update;

            m_InitializeUI?.TrySetResult();
        }

        private void Update()
        {
            if (!IsInitialized) return;

            if (!UseHideHotKey) return;

            if (Input.GetKeyDown(HideHotKeyCode))
            {
                KeyEscapeDown();
            }
        }

        public void Reset()
        {

        }

        public void KeyEscapeDown()
        {
            if (m_ListHideUI.Count > 0)
            {
                UIView uI = (UIView)m_ListHideUI[m_ListHideUI.Count - 1];
                uI.Hide();
            }
            else
            {
                if (!string.IsNullOrEmpty(m_UIConfiguration.UINameWhenNoHotKeyList))
                {
                    ((UIView)GetUI(m_UIConfiguration.UINameWhenNoHotKeyList))?.Show();
                }
            }
        }

        public void EnQueueAutoHideUI(IUIView uI)
        {
            m_ListHideUI.Add(uI);
        }

        public void DeQueueAutoHideUI(IUIView uI)
        {
            if (m_ListHideUI.Count > 0)
            {
                int idx = m_ListHideUI.LastIndexOf(uI);

                if (idx != -1) m_ListHideUI.RemoveAt(idx);
            }
        }

        public virtual IReadOnlyCollection<IUIView> GetManagedUIs()
        {
            return m_ListManagedUIView.Select(u => u.UIComponent).ToArray();
        }

        public virtual T GetUI<T>() where T : class, IUIView => GetUI(typeof(T)) as T;

        public virtual IUIView GetUI(Type type)
        {
            if (m_DicGetUIResults.TryGetValue(type, out var cachedResult))
                return cachedResult;

            foreach (var managedUI in m_ListManagedUIView)
                if (type.IsAssignableFrom(managedUI.ComponentType))
                {
                    var result = managedUI.UIComponent;
                    m_DicGetUIResults[type] = result;
                    return managedUI.UIComponent;
                }

            return null;
        }

        public virtual IUIView GetUI(string name)
        {
            foreach (var managedUI in m_ListManagedUIView)
            {
                if (managedUI.Name == name)
                    return managedUI.UIComponent;
            }

            return null;
        }

        public virtual bool RemoveUI(IUIView managedUI)
        {
            if (!this.m_ListManagedUIView.Any(u => u.UIComponent == managedUI))
                return false;

            var ui = this.m_ListManagedUIView.FirstOrDefault(u => u.UIComponent == managedUI);
            this.m_ListManagedUIView.Remove(ui);
            foreach (var kv in m_DicGetUIResults.ToList())
            {
                if (kv.Value == managedUI)
                    m_DicGetUIResults.Remove(kv.Key);
            }

            BaseUtil.DestroyOrImmediate(ui.GameObject);

            return true;
        }

        public virtual bool AddUI(ManagedUIView managedUI)
        {
            if (this.m_ListManagedUIView.Any(u => u.UIComponent == managedUI.UIComponent))
                return false;

            this.m_ListManagedUIView.Add(managedUI);
            return true;
        }

        protected virtual IUIView InstantiatePrefab(GameObject prefab, string name = default)
        {
            var gameObject = OSManager.Instantiate(prefab, prefab.name, Configuration.OverrideObjectLayer ? (int?)Configuration.ObjectLayer : null);

            if (!gameObject.TryGetComponent<IUIView>(out var uiComponent))
                throw new Exception($"Failed to instantiate `{prefab.name}` UI prefab: the prefab doesn't contain a `{nameof(CustomUI)}` or `{nameof(UIView)}` component on the root object.");

            // if (!uiComponent.RenderCamera)
            //     uiComponent.RenderCamera = cameraManager.UICamera ? cameraManager.UICamera : cameraManager.Camera;
            if (!uiComponent.IsDependenceInScene)
            {
                var uIView = new ManagedUIView(name ?? prefab.name, gameObject, uiComponent);
                this.m_ListManagedUIView.Add(uIView);
            }
            else
            {
                uiComponent.DestroyThis();
            }

            return uiComponent;
        }

        public Sprite GetTransparentImage()
        {
            return m_UIConfiguration.TransparentImage;
        }
    }
}

