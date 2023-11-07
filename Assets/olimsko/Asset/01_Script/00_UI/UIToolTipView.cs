using UnityEngine;
using UnityEngine.UI;
using olimsko;

public class UIToolTipView : UIView
{
    private Vector2 m_AnchoredOffset = Vector2.zero;
    private IToolTipData m_ToolTipData = null;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnShow()
    {
        base.OnShow();

        if (m_ToolTipData == null) Hide();
    }

    protected override void OnHide()
    {
        base.OnHide();

        m_ToolTipData = null;
    }

    public void Show(IToolTipData toolTipData)
    {
        m_ToolTipData = toolTipData;
        SetToolTip();
        Show();
    }

    private void SetToolTip()
    {
        if (m_ToolTipData == null) return;

        Get<UITMPText>("Title").SetText(m_ToolTipData.GetToolTipTitle());
        Get<UITMPText>("Desc").SetText(m_ToolTipData.GetToolTipDesc());


        Vector3[] targetWorldCorners = new Vector3[4];
        m_ToolTipData.GetRectTransform().GetWorldCorners(targetWorldCorners);

        Vector2 pivot = GetToolTipPivot(targetWorldCorners);
        Get<UIImage>("ToolTip").rectTransform.pivot = pivot;
        Get<UIImage>("ToolTip").rectTransform.anchoredPosition = GetAnchorPosFromPivot(targetWorldCorners, pivot);
    }


    private Vector2 GetToolTipPivot(Vector3[] corner)
    {
        int pivotX = 0;
        int pivotY = 0;

        pivotX = corner[0].x < (Screen.width / 2) ? 0 : 1;
        pivotY = corner[0].y < (Screen.height / 2) ? 0 : 1;

        return new Vector2(pivotX, pivotY);
    }

    private Vector2 GetAnchorPosFromPivot(Vector3[] corner, Vector2 pivot)
    {
        float posX = 0;
        float posY = 0;

        posX = pivot.x == 0 ? corner[2].x : corner[0].x;
        posY = pivot.y == 0 ? corner[1].y : corner[3].y;

        return new Vector2(posX, posY);
    }

    private float GetAnchorAngle(Vector2 pivot)
    {
        if (pivot.x == 0 && pivot.y == 0) return 0;
        else if (pivot.x == 0 && pivot.y == 1) return -90;
        else if (pivot.x == 1 && pivot.y == 0) return -270;
        else if (pivot.x == 1 && pivot.y == 1) return -180;
        else return 0;
    }
}