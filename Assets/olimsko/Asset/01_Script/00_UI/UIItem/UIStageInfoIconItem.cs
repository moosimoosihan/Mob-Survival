using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;

public class UIStageInfoIconItem : MonoBehaviour
{
    [SerializeField] private UIImage m_IconImage;
    [SerializeField] private UIImage m_LabelBoss;

    public void SetData(Sprite sprite, bool isBoss = false)
    {
        m_IconImage.sprite = sprite;
        m_LabelBoss.gameObject.SetActive(isBoss);
    }
}
