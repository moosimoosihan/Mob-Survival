using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Localization;
using olimsko;

public class UIExitPopUpView : UIView
{
    [Serializable]
    private class OnMessageChangedEvent : UnityEvent<string> { }

    protected virtual Button ConfirmButton => confirmButton;
    protected virtual Button CancelButton => cancelButton;

    protected virtual bool? Confirmed { get; private set; }

    [SerializeField] private Button confirmButton = default;
    [SerializeField] private Button cancelButton = default;

    [SerializeField] private OnMessageChangedEvent onTitleChanged = default;
    [SerializeField] private OnMessageChangedEvent onMessageChanged = default;

    protected override void OnShow()
    {
        base.OnShow();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    public UniTask<bool> ConfirmAsync(string message)
    {
        return ConfirmAsync(message, null);
    }

    public virtual async UniTask<bool> ConfirmAsync(string message, string title = null)
    {
        if (this.IsVisible) return false;

        ConfirmButton.gameObject.SetActive(true);
        CancelButton.gameObject.SetActive(true);
        SetMessage(message);

        if (title != null) SetTitle(title);

        Show();

        while (!Confirmed.HasValue && IsVisible)
            await UniTask.NextFrame();

        if (!IsVisible) Confirmed = false;
        var result = Confirmed.Value;
        Confirmed = null;

        Hide();

        return result;
    }

    public UniTask NotifyAsync(string message)
    {
        return NotifyAsync(message, null);
    }

    public virtual async UniTask NotifyAsync(string message, string title = null)
    {
        if (IsVisible) return;

        ConfirmButton.gameObject.SetActive(true);
        CancelButton.gameObject.SetActive(false);

        SetMessage(message);

        if (title != null) SetTitle(title);

        Show();

        while (!Confirmed.HasValue && IsVisible)
            await UniTask.NextFrame();

        if (!IsVisible) Confirmed = false;
        Confirmed = null;

        Hide();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        ConfirmButton.onClick.AddListener(Confirm);
        CancelButton.onClick.AddListener(Cancel);

    }

    protected override void OnDisable()
    {
        base.OnDisable();

        ConfirmButton.onClick.RemoveListener(Confirm);
        CancelButton.onClick.RemoveListener(Cancel);

    }

    protected virtual void Confirm()
    {
        if (!IsVisible) return;
        Confirmed = true;
    }

    protected virtual void Cancel()
    {
        if (!IsVisible) return;
        Confirmed = false;
    }

    protected virtual void SetMessage(string value)
    {
        onMessageChanged?.Invoke(value);
    }

    protected virtual void SetTitle(string value)
    {
        onTitleChanged?.Invoke(value);
    }


}
