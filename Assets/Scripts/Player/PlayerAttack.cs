using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem; 

/// <summary>
/// จัดการการโจมตีของผู้เล่นแบบ Melee ระบบ 2-Hit Combo
/// Hitbox วางตำแหน่งหน้าผู้เล่นตามทิศที่หันอยู่
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerAttack : MonoBehaviour
{
    #region Variables — ดาเมจและ Hitbox

    [SerializeField] private float attackDamage = 15f;
    [SerializeField] private float attackRange = 0.8f;

    // ระยะห่างจากตัวผู้เล่นถึงจุดกลาง hitbox
    [SerializeField] private float attackOffset = 0.6f;

    [SerializeField] private LayerMask enemyLayer;

    #endregion

    #region Variables — Combo

    // Combo 2 hit: hit แรกดาเมจปกติ, hit สองดาเมจ x1.5 (finisher)
    [SerializeField] private float comboWindow = 0.5f;
    [SerializeField] private float attackCooldown = 0.3f;

    private static readonly float[] ComboDamageMultipliers = { 1f, 1.5f };

    #endregion

    #region Events

    // ส่ง combo step (0, 1) ไปให้ Animator เลือก clip ที่ถูกต้อง
    public UnityEvent<int> OnAttack;
    public UnityEvent OnHit;

    #endregion

    #region Variables — สถานะภายใน

    private PlayerMovement movement;
    private PlayerHealth health;

    private int comboStep;
    private float cooldownTimer;
    private float comboWindowTimer;

    private readonly Collider2D[] hitResults = new Collider2D[8];

    #endregion

    #region Properties

    public bool IsOnCooldown => cooldownTimer > 0f;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        health = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        TickTimers();
    }

    #endregion

    #region Methods — Input (เรียกจาก PlayerInput component)

    public void OnAttackInput(InputValue value)
    {
        if (!value.isPressed) return;
        Debug.Log($"Attack");
        TryAttack();
    }

    #endregion

    #region Methods — การโจมตี

    private void TryAttack()
    {
        // ห้ามโจมตีขณะ dodge, ตาย, หรืออยู่ใน cooldown
        if (IsOnCooldown || movement.IsDodging || health.IsDead) return;

        ExecuteAttack();
    }

    private void ExecuteAttack()
    {
        float damage = attackDamage * ComboDamageMultipliers[comboStep];
        bool hitSomething = DetectAndDamageEnemies(damage);

        OnAttack?.Invoke(comboStep);
        if (hitSomething) OnHit?.Invoke();

        // เลื่อน combo step แล้วรีเซ็ตถ้าครบรอบ
        comboStep = (comboStep + 1) % ComboDamageMultipliers.Length;
        cooldownTimer = attackCooldown;
        comboWindowTimer = comboWindow;
    }

    private bool DetectAndDamageEnemies(float damage)
    {
        Vector2 hitboxCenter = (Vector2)transform.position
                               + movement.LastMoveDirection * attackOffset;

        int count = Physics2D.OverlapCircleNonAlloc(
            hitboxCenter, attackRange, hitResults, enemyLayer
        );

        bool hitAny = false;
        for (int i = 0; i < count; i++)
        {
            Debug.Log($"count = {count}");
            if (hitResults[i].TryGetComponent<IDamageable>(out var target) && !target.IsDead)
            {
                Debug.Log($"<color=green><b>{hitResults[i].name}</color></b> TakeDamage = {damage}");
                target.TakeDamage(damage);
                hitAny = true;
            }
        }

        return hitAny;
    }

    #endregion

    #region Methods — Timer

    private void TickTimers()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (comboWindowTimer > 0f)
        {
            comboWindowTimer -= Time.deltaTime;

            // หมด window โดยไม่กดต่อ → รีเซ็ต combo
            if (comboWindowTimer <= 0f)
                comboStep = 0;
        }
    }

    #endregion

    #region Gizmos — แสดง Hitbox ใน Scene view

    private void OnDrawGizmosSelected()
    {
        if (movement == null) return;

        Gizmos.color = Color.red;
        Vector2 center = (Vector2)transform.position
                         + movement.LastMoveDirection * attackOffset;
        Gizmos.DrawWireSphere(center, attackRange);
    }

    #endregion
}
