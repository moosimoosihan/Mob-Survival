using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using olimsko;
using System.Linq;
using System;
using Cysharp.Threading.Tasks.Triggers;

public class ItemContext : ContextModel
{
    private ItemTableSO ItemTableSO => OSManager.GetService<DataManager>().GetData<ItemTableSO>();
    private MonsterTableSO MonsterTableSO => OSManager.GetService<DataManager>().GetData<MonsterTableSO>();

    public Action<int> OnConsumeItemUsed;
    public Action<DropItemData> OnCreateNewDropItem;

    public void RunEnemyDeadProcess(int enemyID, GameObject enemy)
    {
        List<ItemTable> listDropItem = ItemTableSO.ItemTable.Where(item => item.Type == ItemType.Drop).ToList();

        for (int i = 0; i < listDropItem.Count; i++)
        {
            float randomValue = UnityEngine.Random.Range(0, 100);

            if (randomValue < listDropItem[i].Probability)
            {
                DropItemData dropItemData = new DropItemData(listDropItem[i].Idx, enemy.transform);
                OnCreateNewDropItem?.Invoke(dropItemData);
            }
        }
    }

    public void CreateCunsumptionItem()
    {
        List<ItemTable> listDropItem = ItemTableSO.ItemTable.Where(item => item.Type == ItemType.Consumption).ToList();

        int sum = listDropItem.Sum(item => item.Probability);
        int randomValue = UnityEngine.Random.Range(0, sum);

        for (int i = 0; i < listDropItem.Count; i++)
        {
            if (randomValue < listDropItem[i].Probability)
            {
                DropItemData dropItemData = new DropItemData(listDropItem[i].Idx, GameManager.instance.playerControl.mainCharacter.transform);
                OnCreateNewDropItem?.Invoke(dropItemData);
                break;
            }
            else
            {
                randomValue -= listDropItem[i].Probability;
            }
        }
    }

    public void CreateItemBoxItem()
    {
        List<ItemTable> listDropItem = ItemTableSO.ItemTable.Where(item => item.Type == ItemType.ItemBox).ToList();

        int sum = listDropItem.Sum(item => item.Probability);
        int randomValue = UnityEngine.Random.Range(0, sum);

        for (int i = 0; i < listDropItem.Count; i++)
        {
            if (randomValue < listDropItem[i].Probability)
            {
                DropItemData dropItemData = new DropItemData(listDropItem[i].Idx, GameManager.instance.playerControl.mainCharacter.transform);
                OnCreateNewDropItem?.Invoke(dropItemData);
                break;
            }
            else
            {
                randomValue -= listDropItem[i].Probability;
            }
        }
    }

    public void GetItem(int itemId)
    {
        ItemTable item = ItemTableSO.ItemTable[itemId];
        switch (item.Type)
        {
            case ItemType.Drop:
                if (item.Idx == 53)
                {
                    GameManager.instance.GetExp((int)item.Value);
                    AudioManager.Instance.ItemSfxPlay(AudioManager.ItemSfx.Exp);
                }
                else if (item.Idx == 52)
                {
                    GameManager.instance.GetGold((int)item.Value);
                    AudioManager.Instance.ItemSfxPlay(AudioManager.ItemSfx.Coin);
                }
                break;
            case ItemType.ItemBox:
                GetBox(item);
                break;
            case ItemType.Consumption:
                switch (item.Idx)
                {
                    case 44:
                        GameManager.instance.Boom(); break;
                    case 45:
                        Player player = GameManager.instance.playerControl.mainCharacter.GetComponent<Player>();
                        player.GetDamage(-player.CurMaxHP * 0.2f, false, null, true); break;
                    case 46:
                        GameManager.instance.GetGold((int)item.Value);
                        AudioManager.Instance.ItemSfxPlay(AudioManager.ItemSfx.Coin); break;
                    case 47:
                        GameManager.instance.GetGold((int)item.Value);
                        AudioManager.Instance.ItemSfxPlay(AudioManager.ItemSfx.Coin); break;
                    case 48:
                        foreach(Player pl in GameManager.instance.players){
                            if(!pl.playerDead){
                                if(pl.strongPotion){
                                    pl.strongPotionTime = 20f;
                                } else {
                                    StartCoroutine(pl.StrongPotion());
                                }
                            }
                        } break;
                    case 49:
                        foreach(Player pl in GameManager.instance.players){
                            if(!pl.playerDead){
                                if(pl.speedPotion){
                                    pl.speedPotionTime = 20f;
                                } else {
                                    StartCoroutine(pl.SpeedPotion());
                                }
                            }
                        } break;
                    case 50:
                        foreach(Player pl in GameManager.instance.players){
                            if(!pl.playerDead){
                                if(pl.agilityPotion){
                                    pl.agilityPotionTime = 20f;
                                } else {
                                    StartCoroutine(pl.AgilityPotion());
                                }
                            }
                        } break;
                    case 51:
                        GameManager.instance.life++; break;                        
                    default: OnConsumeItemUsed?.Invoke(item.Idx); break;
                }
                break;
        }
    }

    private void GetBox(ItemTable boxItem)
    {
        List<ItemTable> listItems = ItemTableSO.ItemTable.Where(item => item.Type == ItemType.Equipment &&
        (boxItem.Grade == ItemGrade.Unique ? (item.Grade == ItemGrade.Unique || item.Grade == ItemGrade.Legendary) : boxItem.Grade == item.Grade))
        .ToList();

        int sum = listItems.Sum(item => item.Probability);

        int randomValue = UnityEngine.Random.Range(0, sum);

        for (int i = 0; i < listItems.Count; i++)
        {
            if (randomValue < listItems[i].Probability)
            {
                OSManager.GetService<ContextManager>().GetContext<PlayerContext>().AddInventoryItem(listItems[i].Idx);
                break;
            }
            else
            {
                randomValue -= listItems[i].Probability;
            }
        }
    }
}
