/// <summary>
/// หนึ่งช่องใน Inventory — เก็บ ItemData และจำนวนที่ถืออยู่
/// </summary>
[System.Serializable]
public class InventorySlot
{
    public ItemData Item;
    public int Amount;

    public InventorySlot(ItemData item, int amount)
    {
        Item   = item;
        Amount = amount;
    }

    public bool IsEmpty => Item == null || Amount <= 0;
}
