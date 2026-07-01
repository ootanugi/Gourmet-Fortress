using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// จัดการ UI ของ Inventory ทั้งหมด — spawn slot UI จาก prefab, refresh เมื่อ inventory เปลี่ยน
/// กด I (Action: Inventory) เพื่อเปิด/ปิด panel
/// </summary>
public class InventoryUI : MonoBehaviour
{
    #region Variables — References

    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject panel;

    // subscribe โดยตรงแทน Send Messages เพราะ InventoryUI อยู่บน Canvas คนละ GameObject กับ PlayerInput
    [SerializeField] private InputActionReference inventoryAction;

    #endregion

    #region Variables — Slot UI

    [SerializeField] private InventorySlotUI slotUIPrefab;

    // parent ที่มี GridLayoutGroup — slot UI จะถูก spawn เป็น children
    [SerializeField] private Transform slotGrid;

    #endregion

    #region Variables — สถานะ

    private InventorySlotUI[] slotUIs;
    private bool isOpen;

    #endregion

    #region Unity Callbacks

    private void Start()
    {
        SpawnSlotUIs();
        inventory.OnInventoryChanged.AddListener(OnInventoryChanged);
        panel.SetActive(false);
    }

    private void OnEnable()
    {
        if (inventoryAction == null) return;
        inventoryAction.action.performed += OnInventoryPerformed;
        inventoryAction.action.Enable();
    }

    private void OnDisable()
    {
        if (inventoryAction == null) return;
        inventoryAction.action.performed -= OnInventoryPerformed;
    }

    private void OnDestroy()
    {
        inventory.OnInventoryChanged.RemoveListener(OnInventoryChanged);
    }

    #endregion

    #region Methods — Input

    private void OnInventoryPerformed(InputAction.CallbackContext ctx)
    {
        Toggle();
    }

    #endregion

    #region Methods — Toggle และ Refresh

    private void Toggle()
    {
        isOpen = !isOpen;
        panel.SetActive(isOpen);

        if (isOpen) Refresh();
    }

    private void OnInventoryChanged()
    {
        // refresh เฉพาะตอนที่ panel เปิดอยู่เพื่อประหยัด performance
        if (isOpen) Refresh();
    }

    private void Refresh()
    {
        var slots = inventory.Slots;

        for (int i = 0; i < slotUIs.Length; i++)
        {
            if (i < slots.Count)
                slotUIs[i].SetSlot(slots[i]);
            else
                slotUIs[i].SetEmpty();
        }
    }

    #endregion

    #region Methods — Spawn

    private void SpawnSlotUIs()
    {
        // อ่าน maxSlots จาก inventory ผ่าน reflection ไม่ได้ เลยใช้จำนวน slot UI ที่ spawn
        // ใช้ Slots.Count สูงสุดจาก capacity ที่ตั้งใน Inventory (default 20)
        int slotCount = 20;

        slotUIs = new InventorySlotUI[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            var slotUI = Instantiate(slotUIPrefab, slotGrid, false);
            slotUI.SetEmpty();
            slotUIs[i] = slotUI;
        }
    }

    #endregion
}
