using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IToolTipData
{
    string GetToolTipTitle();
    string GetToolTipDesc();
    bool IsCanShowToolTip();
    RectTransform GetRectTransform();
}
