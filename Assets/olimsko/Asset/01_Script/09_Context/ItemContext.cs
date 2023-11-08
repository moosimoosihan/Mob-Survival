using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;
using System.Linq;
using System;

public class ItemContext : ContextModel
{
    private ItemTableSO ItemTableSO => OSManager.GetService<DataManager>().GetData<ItemTableSO>();
    private MonsterTableSO MonsterTableSO => OSManager.GetService<DataManager>().GetData<MonsterTableSO>();

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
}
