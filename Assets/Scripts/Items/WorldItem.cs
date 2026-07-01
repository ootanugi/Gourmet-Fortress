using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Item ที่ตกอยู่บนพื้นใน dungeon รอให้ผู้เล่นเข้ามาเก็บ
/// เรียก Initialize() หลัง Instantiate เสมอ
/// </summary>
public class WorldItem : MonoBehaviour
{
    #region Variables — Visual

    [SerializeField] private SpriteRenderer spriteRenderer;

    #endregion

    #region Variables — สถานะ

    private ItemData data;
    private int amount;

    #endregion

    #region Events

    // Inventory จะ subscribe event นี้เมื่อระบบพร้อม
    public UnityEvent<WorldItem> OnPickedUp;

    #endregion

    #region Properties

    public ItemData Data   => data;
    public int Amount      => amount;

    #endregion

    #region Methods — Initialization

    public void Initialize(ItemData itemData, int itemAmount)
    {
        data   = itemData;
        amount = itemAmount;

        if (spriteRenderer != null && data.Icon != null)
            spriteRenderer.sprite = data.Icon;

        gameObject.name = $"[Item] {data.ItemName} x{amount}";
    }

    #endregion

    #region Methods — Pickup

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        OnPickedUp?.Invoke(this);
    }

    // เรียกจาก Inventory หลังเก็บสำเร็จแล้ว
    public void Collect()
    {
        Destroy(gameObject);
    }

    #endregion
}
