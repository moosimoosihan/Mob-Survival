using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using olimsko;
using TMPro;

public class UITMPHoverGradation : UITransitionComp
{
    private UITMPText m_TextMeshProUGUI;

    [SerializeField] private VertexGradient m_VertexGradientDefault = new VertexGradient(Color.white, Color.white, Color.white, Color.white);
    [SerializeField] private VertexGradient m_VertexGradientHover = new VertexGradient(Color.white, Color.white, Color.white, Color.white);

    protected override void Awake()
    {
        m_TextMeshProUGUI = GetComponent<UITMPText>();
        base.Awake();

        m_TextMeshProUGUI.colorGradient = m_VertexGradientDefault;
    }

    public override void OnPointerClick()
    {
        base.OnPointerClick();

    }

    public override void OnPointerEnter()
    {
        base.OnPointerEnter();
        m_TextMeshProUGUI.enableVertexGradient = true;
        m_TextMeshProUGUI.colorGradient = m_VertexGradientHover;

    }

    public override void OnPointerExit()
    {
        base.OnPointerExit();
        m_TextMeshProUGUI.enableVertexGradient = false;
        m_TextMeshProUGUI.colorGradient = m_VertexGradientDefault;
    }

    public override void OnPointerDown()
    {
        base.OnPointerDown();
    }

    public override void OnInteractableStateChanged(bool isInteractable)
    {
        base.OnInteractableStateChanged(isInteractable);
    }
}