using UnityEngine;

public class InventoryItemData
{

    private int id;

    public InventoryItemData()
    {
    }

    public InventoryItemData(int id)
    {
        this.id = id;
    }

    public int Id { get => id; set => id = value; }
}