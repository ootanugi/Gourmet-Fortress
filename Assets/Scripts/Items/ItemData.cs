using UnityEngine;

/// <summary>
/// ScriptableObject เก็บข้อมูลของ item แต่ละชนิด
/// สร้างผ่าน Assets → Create → Gourmet Fortress → Item Data
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "Gourmet Fortress/Item Data")]
public class ItemData : ScriptableObject
{
    #region Variables — ข้อมูลพื้นฐาน

    [SerializeField] private string itemName = "Unnamed Item";
    [SerializeField] private Sprite icon;
    [SerializeField] private ItemType itemType;
    [SerializeField] private string description;

    #endregion

    #region Variables — ระบบร้านค้า

    [SerializeField] private int sellPrice = 10;

    #endregion

    #region Variables — Inventory

    // จำนวนสูงสุดที่วางซ้อนกันได้ในช่องเดียว
    [SerializeField] private int maxStackSize = 99;

    #endregion

    #region Properties

    public string ItemName     => itemName;
    public Sprite Icon         => icon;
    public ItemType ItemType   => itemType;
    public string Description  => description;
    public int SellPrice       => sellPrice;
    public int MaxStackSize    => maxStackSize;

    #endregion
}

public enum ItemType
{
    Ingredient,   // วัตถุดิบดิบจาก dungeon
    Material,     // วัสดุที่ใช้อัพเกรด
    CookedFood    // อาหารที่ปรุงแล้ว พร้อมขาย
}
