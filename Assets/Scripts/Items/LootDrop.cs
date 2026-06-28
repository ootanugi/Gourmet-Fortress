using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ใส่บน Enemy — เมื่อ EnemyBase.OnDeath ถูก invoke จะ roll drop table แล้ว spawn WorldItem
/// </summary>
public class LootDrop : MonoBehaviour
{
    #region Variables — Drop Table

    [SerializeField] private List<LootEntry> lootTable = new();

    #endregion

    #region Variables — Spawn

    [SerializeField] private GameObject worldItemPrefab;

    // กระจาย item ที่ drop ออกไม่ให้ซ้อนกันทับ
    [SerializeField] private float scatterRadius = 0.4f;

    #endregion

    #region Unity Callbacks

    private void Start()
    {
        // hook OnDeath โดยไม่ต้องลาก event ใน Inspector
        var enemy = GetComponent<EnemyBase>();
        if (enemy != null)
            enemy.OnDeath.AddListener(RollAndDrop);
    }

    #endregion

    #region Methods — Drop Logic

    private void RollAndDrop()
    {
        if (worldItemPrefab == null) return;

        foreach (var entry in lootTable)
        {
            if (!RollSuccess(entry.DropChance)) continue;

            int amount = Random.Range(entry.MinAmount, entry.MaxAmount + 1);
            SpawnWorldItem(entry.Item, amount);
        }
    }

    private bool RollSuccess(float chance)
    {
        return Random.value <= chance;
    }

    private void SpawnWorldItem(ItemData item, int amount)
    {
        Vector2 offset = Random.insideUnitCircle * scatterRadius;
        Vector3 spawnPos = transform.position + (Vector3)offset;

        var go = Instantiate(worldItemPrefab, spawnPos, Quaternion.identity);

        if (go.TryGetComponent<WorldItem>(out var worldItem))
            worldItem.Initialize(item, amount);
    }

    #endregion
}

/// <summary>
/// แถวเดียวใน drop table — กำหนดว่า item นี้มีโอกาส drop กี่ % และ drop กี่ชิ้น
/// </summary>
[System.Serializable]
public class LootEntry
{
    [SerializeField] private ItemData item;

    [Range(0f, 1f)]
    [SerializeField] private float dropChance = 0.5f;

    [SerializeField] private int minAmount = 1;
    [SerializeField] private int maxAmount = 1;

    public ItemData Item    => item;
    public float DropChance => dropChance;
    public int MinAmount    => minAmount;
    public int MaxAmount    => maxAmount;
}
