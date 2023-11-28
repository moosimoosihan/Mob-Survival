using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;
using System.Linq;

public class UIResultPlayerInfo : MonoBehaviour
{
    [SerializeField] private UIImage m_PlayerIcon;
    [SerializeField] private UITMPText m_UniqueSkillCount;
    [SerializeField] private UITMPText m_NormalSkillCount;
    [SerializeField] private UITMPText m_LevelSkillCount;

    SkillTableSO SkillTableSO => OSManager.GetService<DataManager>().GetData<SkillTableSO>();

    public async void SetPlayerInfo(int charId)
    {
        m_PlayerIcon.sprite = await OSManager.GetService<DataManager>().GetData<CharacterTableSO>().CharacterTable[charId].GetSDSprite();

        var playerSkill = OSManager.GetService<ContextManager>().GetContext<PlayerContext>().DicPlayerEquipedSkill[charId];

        List<SkillTable> listSkill = SkillTableSO.SkillTable.Where(x => x.CharacterID == charId).ToList();

        int uniqueSkillCount = listSkill.Where(x => x.Type == SkillType.UniqueSkill && playerSkill.DicEquipedSkill.ContainsKey(x.SkillID)).Count();
        int normalSkillCount = listSkill.Where(x => x.Type == SkillType.BasicSkill && playerSkill.DicEquipedSkill.ContainsKey(x.SkillID)).Count();
        int levelSkillCount = listSkill.Where(x => x.Type == SkillType.LevelSkill && playerSkill.DicEquipedSkill.ContainsKey(x.SkillID)).Count();

        m_UniqueSkillCount.text = uniqueSkillCount.ToString();
        m_NormalSkillCount.text = normalSkillCount.ToString();
        m_LevelSkillCount.text = levelSkillCount.ToString();
    }
}
