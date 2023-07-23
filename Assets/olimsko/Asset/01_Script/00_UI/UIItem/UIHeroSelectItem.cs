using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using olimsko;

public class UIHeroSelectItem : MonoBehaviour
{
    [SerializeField] private UIToggle m_Toggle;

    [SerializeField] private UIImage m_CharacterImage;
    [SerializeField] private UIImage m_LockImage;

    private CharacterTable m_CharacterTable;

    public UIToggle Toggle { get => m_Toggle; }

    public async void SetData(CharacterTable characterTable = null, bool isLock = false, bool isOnlyShow = false)
    {
        m_CharacterTable = characterTable;

        Toggle.interactable = isOnlyShow ? false : !isLock;

        m_LockImage.gameObject.SetActive(isOnlyShow ? false : isLock);
        m_CharacterImage.gameObject.SetActive(characterTable == null ? false : true);

        if (characterTable != null)
        {
            m_CharacterImage.sprite = await m_CharacterTable.GetProfileSprite();
        }
    }
}
