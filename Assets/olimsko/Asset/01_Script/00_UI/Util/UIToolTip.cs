using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using olimsko;

[RequireComponent(typeof(IToolTipData))]
public class UIToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float m_ShowDelay = 0.5f;
    private IToolTipData m_ToolTipData = null;
    private Coroutine m_ShowCoroutine = null;

    private void Awake()
    {
        m_ToolTipData = GetComponent<IToolTipData>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (m_ToolTipData.IsCanShowToolTip())
        {
            if (m_ShowCoroutine != null)
            {
                StopCoroutine(m_ShowCoroutine);
            }
            m_ShowCoroutine = StartCoroutine(ToolTipTimer());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (m_ShowCoroutine != null)
        {
            StopCoroutine(m_ShowCoroutine);
            m_ShowCoroutine = null;
        }

        OSManager.GetService<UIManager>().GetUI<UIToolTipView>().Hide();
    }

    private IEnumerator ToolTipTimer()
    {
        yield return new WaitForSecondsRealtime(m_ShowDelay);

        OSManager.GetService<UIManager>().GetUI<UIToolTipView>().Show(m_ToolTipData);
    }
}
