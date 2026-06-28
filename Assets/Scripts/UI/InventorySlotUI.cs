using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ควบคุม Visual ของช่อง Inventory แต่ละช่อง
/// เรียก SetSlot() หรือ SetEmpty() จาก InventoryUI เมื่อต้อง refresh
/// </summary>
public class InventorySlotUI : MonoBehaviour
{
    #region Variables — UI References

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;

    // ซ่อน frame ของ slot เมื่อว่างอยู่ เพื่อให้ดูสะอาดขึ้น
    [SerializeField] private GameObject emptyOverlay;

    #endregion

    #region Methods — อัปเดต Visual

    public void SetSlot(InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty)
        {
            SetEmpty();
            return;
        }

        iconImage.sprite  = slot.Item.Icon;
        iconImage.enabled = slot.Item.Icon != null;

        // แสดงจำนวนเฉพาะเมื่อมากกว่า 1 เพื่อไม่ให้ดูรก
        amountText.text    = slot.Amount > 1 ? slot.Amount.ToString() : string.Empty;
        amountText.enabled = slot.Amount > 1;

        if (emptyOverlay != null)
            emptyOverlay.SetActive(false);
    }

    public void SetEmpty()
    {
        iconImage.sprite  = null;
        iconImage.enabled = false;
        amountText.text   = string.Empty;
        amountText.enabled = false;

        if (emptyOverlay != null)
            emptyOverlay.SetActive(true);
    }

    #endregion
}
