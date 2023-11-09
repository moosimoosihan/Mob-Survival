using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;

public class SkillContext : ContextModel
{
    private PlayerContext PlayerContext => OSManager.GetService<ContextManager>().GetContext<PlayerContext>();

    public float WarriorSkill(int skillIdx = 0, int playerID = 0)
    {
        if (PlayerContext.IsHasSkill(playerID, skillIdx))
        {
            int skillLevel = PlayerContext.GetSkillLevel(playerID, skillIdx);
        }

        return 0;
    }
}
