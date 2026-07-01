using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// คลาสพื้นฐานของศัตรูทุกตัว — จัดการ AI, HP, และการโจมตีผู้เล่น
/// ศัตรูใหม่ให้ inherit คลาสนี้แล้ว override method ที่ต้องการ
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBase : MonoBehaviour, IDamageable
{
    #region Variables — Stats

    [SerializeField] private float maxHealth = 30f;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1.2f;

    #endregion

    #region Variables — Detection

    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float attackRange = 1f;

    #endregion

    #region Variables — Knockback

    // แรงกระแทกที่ศัตรูได้รับเมื่อโดนโจมตี
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.15f;

    #endregion

    #region Variables — State

    private enum EnemyState { Idle, Chase, Attack, Dead }
    private EnemyState currentState = EnemyState.Idle;

    private float attackCooldownTimer;
    private bool isKnockedBack;

    #endregion

    #region Variables — References

    private Rigidbody2D rb;
    private Transform player;
    private PlayerHealth playerHealth;
    private float currentHealth;

    #endregion

    #region Events

    // ต่อไปใช้กับระบบ Loot Drop และ Quest
    public UnityEvent OnDeath;

    #endregion

    #region Properties

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => currentState == EnemyState.Dead;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        // หา Player จาก Tag — ถ้า Player ยังไม่มีในฉากจะ return null
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return;

        player = playerObj.transform;
        playerHealth = playerObj.GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (IsDead || player == null) return;

        TickAttackCooldown();
        UpdateStateMachine();
    }

    private void FixedUpdate()
    {
        if (IsDead || isKnockedBack) return;

        if (currentState != EnemyState.Chase)
        {
            // ป้องกันถูก physics impulse ผลักออกไปขณะไม่ได้เคลื่อนที่
            rb.velocity = Vector2.zero;
            return;
        }

        ChasePlayer();
    }

    #endregion

    #region Methods — State Machine

    private void UpdateStateMachine()
    {
        float distance = GetDistanceToPlayer();

        switch (currentState)
        {
            case EnemyState.Idle:
                if (distance <= detectionRange)
                    SetState(EnemyState.Chase);
                break;

            case EnemyState.Chase:
                if (distance <= attackRange)
                    SetState(EnemyState.Attack);
                else if (distance > detectionRange)
                    SetState(EnemyState.Idle);
                break;

            case EnemyState.Attack:
                if (distance > attackRange)
                    SetState(EnemyState.Chase);
                else
                    TryAttack();
                break;
        }
    }

    private void SetState(EnemyState newState)
    {
        // หยุดเคลื่อนที่เมื่อออกจาก Chase
        if (currentState == EnemyState.Chase && newState != EnemyState.Chase)
            rb.velocity = Vector2.zero;

        currentState = newState;
    }

    #endregion

    #region Methods — AI

    private void ChasePlayer()
    {
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    private void TryAttack()
    {
        if (attackCooldownTimer > 0f) return;
        if (playerHealth == null || playerHealth.IsDead) return;

        playerHealth.TakeDamage(attackDamage);
        attackCooldownTimer = attackCooldown;
    }

    private void TickAttackCooldown()
    {
        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;
    }

    #endregion

    #region Methods — รับดาเมจและตาย (IDamageable)

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0f);

        if (currentHealth <= 0f)
            Die();
    }

    // เรียกจากภายนอกเพื่อให้ศัตรูถูกกระแทกกลับ เช่น ตอนโดน PlayerAttack
    public void ApplyKnockback(Vector2 direction)
    {
        if (IsDead) return;
        StartCoroutine(KnockbackRoutine(direction));
    }

    private System.Collections.IEnumerator KnockbackRoutine(Vector2 direction)
    {
        isKnockedBack = true;
        rb.velocity = direction * knockbackForce;

        yield return new WaitForSeconds(knockbackDuration);

        isKnockedBack = false;
    }

    private void Die()
    {
        SetState(EnemyState.Dead);
        rb.velocity = Vector2.zero;

        OnDeath?.Invoke();

        // Destroy หน่วง 0.3 วิ เผื่อ animation ตายทำงานได้ก่อน
        Destroy(gameObject, 0.3f);
    }

    #endregion

    #region Methods — Utility

    private float GetDistanceToPlayer()
    {
        return Vector2.Distance(transform.position, player.position);
    }

    #endregion

    #region Gizmos — แสดง Detection และ Attack range ใน Scene view

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    #endregion
}
