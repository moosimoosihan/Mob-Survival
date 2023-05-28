using UnityEngine;
using UnityEngine.Events;

namespace olimsko
{
    public class UIToggleCheck : MonoBehaviour
    {
        [SerializeField] private UIToggle m_UIToggle;
        [SerializeField] private UnityEvent m_IsOn;
        [SerializeField] private UnityEvent m_IsOff;

        private void Awake()
        {
            if (m_UIToggle == null)
                m_UIToggle = GetComponent<UIToggle>();

            if (m_UIToggle != null)
                m_UIToggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(bool isOn)
        {
            if (isOn)
                m_IsOn.Invoke();
            else
                m_IsOff.Invoke();
        }
    }
}