using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;
using System;
using System.Linq;

public class PlayerContext : ContextModel
{
    public Action OnInventoryItemChanged;
    public Action OnSelectedCharacterChanged;

    private InventoryItemData[] m_ArrayEquipedItem = new InventoryItemData[4];
    private InventoryItemData[] m_ArrayInventoryItem = new InventoryItemData[28];

    private Dictionary<int, EquipedSkillData> m_DicPlayerEquipedSkill = new Dictionary<int, EquipedSkillData>();

    public InventoryItemData[] EquipedItem { get => m_ArrayEquipedItem; }
    public InventoryItemData[] InventoryItem { get => m_ArrayInventoryItem; }

    public Dictionary<int, EquipedSkillData> DicPlayerEquipedSkill { get => m_DicPlayerEquipedSkill; set => m_DicPlayerEquipedSkill = value; }

    private ContextManager ContextManager => OSManager.GetService<ContextManager>();
    ItemTableSO itemTableSO => OSManager.GetService<DataManager>().GetData<ItemTableSO>();

    private int m_SelectedCharacterIdx = 0;

    public int SelectedCharacterIdx
    {
        get
        {
            return m_SelectedCharacterIdx;
        }
        set
        {
            m_SelectedCharacterIdx = value;
            OnSelectedCharacterChanged?.Invoke();
        }
    }

    public void ResetContext()
    {
        for (int i = 0; i < m_ArrayEquipedItem.Length; i++)
        {
            m_ArrayEquipedItem[i] = null;
        }

        for (int i = 0; i < m_ArrayInventoryItem.Length; i++)
        {
            m_ArrayInventoryItem[i] = null;
        }

        m_DicPlayerEquipedSkill.Clear();

        for (int i = 0; i < ContextManager.GetContext<StageContext>().ListSelectedHero.Count; i++)
        {
            int playerID = ContextManager.GetContext<StageContext>().ListSelectedHero[i];
            m_DicPlayerEquipedSkill.Add(playerID, new EquipedSkillData(playerID));
        }
    }

    public void SetEquipedItem(int index, InventoryItemData itemData)
    {
        if (index < 0 || index >= m_ArrayEquipedItem.Length)
            return;

        m_ArrayEquipedItem[index] = itemData;
        OnInventoryItemChanged?.Invoke();
    }

    public void SetInventoryItem(int index, InventoryItemData itemData)
    {
        if (index < 0 || index >= m_ArrayInventoryItem.Length)
            return;

        m_ArrayInventoryItem[index] = itemData;
        OnInventoryItemChanged?.Invoke();
    }

    public void ReplaceInventoryItem(int prevSlotID, bool isPrevEquipedSlot, int nextSlotID, bool isNextEquipedSlot)
    {
        int? prevItemID = isPrevEquipedSlot ? m_ArrayEquipedItem[prevSlotID]?.Id : m_ArrayInventoryItem[prevSlotID]?.Id;
        int? nextItemID = isNextEquipedSlot ? m_ArrayEquipedItem[nextSlotID]?.Id : m_ArrayInventoryItem[nextSlotID]?.Id;

        if (isPrevEquipedSlot)
        {
            m_ArrayEquipedItem[prevSlotID] = nextItemID == null ? null : new InventoryItemData(nextItemID.Value);
        }
        else
        {
            m_ArrayInventoryItem[prevSlotID] = nextItemID == null ? null : new InventoryItemData(nextItemID.Value);
        }

        if (isNextEquipedSlot)
        {
            m_ArrayEquipedItem[nextSlotID] = prevItemID == null ? null : new InventoryItemData(prevItemID.Value);
        }
        else
        {
            m_ArrayInventoryItem[nextSlotID] = prevItemID == null ? null : new InventoryItemData(prevItemID.Value);
        }

        OnInventoryItemChanged?.Invoke();
    }

    public void AddInventoryItem(int id)
    {
        for (int i = 0; i < m_ArrayInventoryItem.Length; i++)
        {
            if (m_ArrayInventoryItem[i] == null)
            {
                m_ArrayInventoryItem[i] = new InventoryItemData(id);
                OnInventoryItemChanged?.Invoke();
                return;
            }
        }
    }

    public void RemoveInventoryItem(int index)
    {
        if (index < 0 || index >= m_ArrayInventoryItem.Length)
            return;

        m_ArrayInventoryItem[index] = null;
        OnInventoryItemChanged?.Invoke();
    }

    public void AddEquipedSkill(int playerID, int idx)
    {
        if (m_DicPlayerEquipedSkill.ContainsKey(playerID))
        {
            m_DicPlayerEquipedSkill[playerID].AddEquipedSkill(idx);
        }
    }

    public ItemTable IsHasItem(int itemID)
    {
        InventoryItemData inventoryItemData = EquipedItem.FirstOrDefault(item => item != null && item.Id == itemID);

        if (inventoryItemData != null && inventoryItemData.Id == itemID)
        {
            return itemTableSO.ItemTable[itemID];
        }
        return null;
    }

    public int[] GetRandomSkill()
    {
        List<SkillTable> listSkillTable = OSManager.GetService<DataManager>().GetData<SkillTableSO>().SkillTable;

        int[] randomSkill = new int[4] { -1, -1, -1, -1 };
        int[] randomChar = new int[2] { -1, -1 };

        List<int> validKeys = m_DicPlayerEquipedSkill.Keys.Where(key => m_DicPlayerEquipedSkill[key].IsNeedSkill()).ToList();

        for (int i = 0; i < randomChar.Length; i++)
        {
            if (validKeys.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, validKeys.Count);
                randomChar[i] = validKeys[randomIndex];
                validKeys.RemoveAt(randomIndex);
            }
        }
        int characterID0 = randomChar[0];
        int characterID1 = randomChar[1];

        if (characterID0 != -1)
        {
            List<SkillTable> matchingSkills = listSkillTable.Where(skill => skill.CharacterID == characterID0 && skill.Type == m_DicPlayerEquipedSkill[characterID0].GetNeedSkillType() && m_DicPlayerEquipedSkill[characterID0].IsCanGetSkillIdx(skill.Idx)).ToList();

            if (matchingSkills.Count >= 2)
            {
                int randomIndex = UnityEngine.Random.Range(0, matchingSkills.Count);
                randomSkill[1] = matchingSkills[randomIndex].Idx;
                matchingSkills.RemoveAt(randomIndex);
            }
            if (matchingSkills.Count >= 1)
            {
                int randomIndex = UnityEngine.Random.Range(0, matchingSkills.Count);
                randomSkill[0] = matchingSkills[randomIndex].Idx;
            }
        }

        if (characterID1 != -1)
        {
            List<SkillTable> matchingSkills = listSkillTable.Where(skill => skill.CharacterID == characterID1 && skill.Type == m_DicPlayerEquipedSkill[characterID1].GetNeedSkillType() && m_DicPlayerEquipedSkill[characterID1].IsCanGetSkillIdx(skill.Idx)).ToList();

            if (matchingSkills.Count >= 2)
            {
                int randomIndex = UnityEngine.Random.Range(0, matchingSkills.Count);
                randomSkill[3] = matchingSkills[randomIndex].Idx;
                matchingSkills.RemoveAt(randomIndex);
            }
            if (matchingSkills.Count >= 1)
            {
                int randomIndex = UnityEngine.Random.Range(0, matchingSkills.Count);
                randomSkill[2] = matchingSkills[randomIndex].Idx;
            }
        }
        return randomSkill;
    }

    public bool IsHasSkill(int playerID, int skillIdx)
    {
        if (m_DicPlayerEquipedSkill.ContainsKey(playerID))
        {
            if (GetSkillLevel(playerID, skillIdx) > 0)
            {
                return true;
            }
        }
        return false;
    }

    public int GetSkillLevel(int playerID, int skillIdx)
    {
        if (m_DicPlayerEquipedSkill.ContainsKey(playerID))
        {
            return m_DicPlayerEquipedSkill[playerID].GetSkillLevel(skillIdx);
        }
        return 0;
    }
}
