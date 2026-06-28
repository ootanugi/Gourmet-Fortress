using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// จัดการ Inventory ของผู้เล่น — รองรับ auto-stack และ pickup อัตโนมัติเมื่อเดินทับ WorldItem
/// Player GameObject ต้องมี Trigger Collider2D ขนาดเล็ก (IsTrigger = true) สำหรับ pickup
/// </summary>
public class Inventory : MonoBehaviour
{
    #region Variables — ขนาด Inventory

    [SerializeField] private int maxSlots = 20;

    #endregion

    #region Variables — Slots

    [SerializeField] private List<InventorySlot> slots = new();

    #endregion

    #region Events

    // UI subscribe event นี้เพื่อ refresh เมื่อ inventory เปลี่ยน
    public UnityEvent OnInventoryChanged;

    #endregion

    #region Properties

    public IReadOnlyList<InventorySlot> Slots => slots;
    public bool IsFull => slots.Count >= maxSlots;

    #endregion

    #region Methods — Pickup

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<WorldItem>(out var worldItem)) return;

        int leftover = AddItem(worldItem.Data, worldItem.Amount);

        // เก็บสำเร็จอย่างน้อยบางส่วน → ลบออกจากโลก
        if (leftover < worldItem.Amount)
            worldItem.Collect();
    }

    #endregion

    #region Methods — เพิ่ม Item

    /// <summary>
    /// เพิ่ม item เข้า inventory โดย auto-stack
    /// คืนค่าจำนวนที่ใส่ไม่ได้ (0 = ใส่ครบ, >0 = inventory เต็ม)
    /// </summary>
    public int AddItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0) return amount;

        int remaining = amount;

        // เติมใน slot ที่มี item ชนิดเดียวกันและยังไม่เต็ม stack ก่อน
        foreach (var slot in slots)
        {
            if (remaining <= 0) break;
            if (slot.Item != item) continue;

            int canAdd = item.MaxStackSize - slot.Amount;
            int adding = Mathf.Min(canAdd, remaining);
            slot.Amount += adding;
            remaining -= adding;
        }

        // สร้าง slot ใหม่สำหรับของที่ยังเหลือ
        while (remaining > 0 && slots.Count < maxSlots)
        {
            int adding = Mathf.Min(item.MaxStackSize, remaining);
            slots.Add(new InventorySlot(item, adding));
            remaining -= adding;
        }

        if (remaining < amount)
            OnInventoryChanged?.Invoke();

        return remaining;
    }

    #endregion

    #region Methods — ลบ Item

    /// <summary>
    /// ลบ item ออกจาก inventory — คืน true ถ้าลบสำเร็จ
    /// </summary>
    public bool RemoveItem(ItemData item, int amount)
    {
        if (!HasItem(item, amount)) return false;

        int remaining = amount;

        // ลบจาก slot หลังก่อน เพื่อรักษาลำดับ slot แรกๆ ไว้
        for (int i = slots.Count - 1; i >= 0 && remaining > 0; i--)
        {
            if (slots[i].Item != item) continue;

            int removing = Mathf.Min(slots[i].Amount, remaining);
            slots[i].Amount -= removing;
            remaining -= removing;

            if (slots[i].Amount <= 0)
                slots.RemoveAt(i);
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    #endregion

    #region Methods — ตรวจสอบ

    public bool HasItem(ItemData item, int amount = 1)
    {
        int total = 0;
        foreach (var slot in slots)
        {
            if (slot.Item == item)
                total += slot.Amount;
        }
        return total >= amount;
    }

    public int GetTotalAmount(ItemData item)
    {
        int total = 0;
        foreach (var slot in slots)
        {
            if (slot.Item == item)
                total += slot.Amount;
        }
        return total;
    }

    #endregion
}
