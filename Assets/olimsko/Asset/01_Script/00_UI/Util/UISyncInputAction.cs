using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class UISyncInputAction : MonoBehaviour
{
    private InputManager InputManager => OSManager.GetService<InputManager>();

    [SerializeField] private string m_ActionName;
    [SerializeField] private int m_ActionIndex;

    [SerializeField] private Sprite m_LeftMouseSprite;
    [SerializeField] private Sprite m_RightMouseSprite;

    [SerializeField] private UIImage m_BindingImage;

    private TextMeshProUGUI m_InputText;
    private Coroutine m_Coroutine;

    private void Awake()
    {
        GetComponent<UIButton>().onClick.AddListener(ChangeKeyBinding);
        m_InputText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        SetInputPath();
    }

    private void OnEnable()
    {
        SetInputPath();
    }

    public void ChangeKeyBinding()
    {
        if (m_Coroutine != null)
        {
            StopCoroutine(m_Coroutine);
        }
        m_Coroutine = StartCoroutine(RebindKey());
    }

    private void OnDisable()
    {
        if (m_Coroutine != null)
        {
            StopCoroutine(m_Coroutine);
        }
    }

    IEnumerator RebindKey()
    {
        m_InputText.text = "";
        InputManager.GetAction(m_ActionName).Disable();
        var rebindOperation = InputManager.GetAction(m_ActionName).PerformInteractiveRebinding()
        .WithTargetBinding(m_ActionIndex)
        .WithControlsExcluding("Keyboard/escape")
        .OnMatchWaitForAnother(0.1f)
        .Start();

        while (!rebindOperation.completed)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                rebindOperation.Cancel();
                yield break;
            }

            yield return null;
        }

        SetInputPath();
    }

    private void SetInputPath()
    {
        string path = InputManager.GetAction(m_ActionName).bindings[m_ActionIndex].effectivePath.Substring(InputManager.GetAction(m_ActionName).bindings[m_ActionIndex].effectivePath.LastIndexOf("/") + 1).ToUpper();

        if (path == "LEFTBUTTON")
        {
            m_BindingImage.sprite = m_LeftMouseSprite;
            m_BindingImage.CrossFadeAlpha(1, 0.3f, true);
            path = "";
        }
        else if (path == "RIGHTBUTTON")
        {
            m_BindingImage.sprite = m_RightMouseSprite;
            m_BindingImage.CrossFadeAlpha(1, 0.3f, true);
            path = "";
        }
        else
        {
            m_BindingImage.CrossFadeAlpha(0, 0, true);
        }

        m_InputText.text = path;
    }
}
