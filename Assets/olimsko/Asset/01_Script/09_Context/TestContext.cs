using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;
using UnityEngine.InputSystem;

public class TestContext : ContextModel
{
    private Stat Damage;
    private Stat AttackSpeed;

    private List<CharacterTable> CharacterData => OSManager.GetService<DataManager>().GetData<CharacterTableSO>().CharacterTable;

    private void Init()
    {
        Damage = new Stat();
        AttackSpeed = new Stat();
    }

    private void Start()
    {

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            OSManager.GetService<GlobalManager>().PlayerInventory.Gold += 1000;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            OSManager.GetService<GlobalManager>().PlayerInventory.Gold -= 1000;
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log(CharacterData[0].Name + " " + CharacterData[0].Desc);
        }
    }
}

public enum StatType
{
    Damage,
    AttackSpeed
}

public enum StatAddType
{
    Add,
    Multiply
}

public enum StatCategory
{
    Character,
    Item
}

public class Stat
{

    public StatType StatType;
    public List<StatClass> StatList;

    public Stat()
    {

    }

    public float GetCurrentStat()
    {
        float value = 0;

        for (int i = 0; i < StatList.Count; i++)
        {
            switch (StatList[i].StatAddType)
            {
                case StatAddType.Add:
                    value += StatList[i].Value;
                    break;
                case StatAddType.Multiply:
                    value *= StatList[i].Value;
                    break;
            }
        }

        return value;
    }
}

public class StatClass
{
    public StatCategory StatCategory;
    public StatType StatType;
    public StatAddType StatAddType;
    public float Value;
}