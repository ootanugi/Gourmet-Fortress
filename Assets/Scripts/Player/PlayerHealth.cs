using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// จัดการ HP ของผู้เล่น รวมถึง Invincibility Frame ตอนโดนโจมตีและ Dodge
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
public class PlayerHealth : MonoBehaviour
{
    #region Variables — HP

    [SerializeField] private float maxHealth = 100f;

    #endregion

    #region Variables — Invincibility Frame

    // ระยะเวลา iFrame หลังโดนโจมตี ป้องกันโดนดาเมจซ้ำทันที
    [SerializeField] private float damageIFrameDuration = 0.6f;

    #endregion

    #region Events

    public UnityEvent<float, float> OnHealthChanged;  // (currentHP, maxHP)
    public UnityEvent OnDamaged;
    public UnityEvent OnDeath;

    #endregion

    #region Variables — สถานะภายใน

    private PlayerMovement movement;
    private float currentHealth;
    private float iFrameTimer;
    private bool isDead;

    #endregion

    #region Properties

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    // รวม iFrame จากทั้งโดนดาเมจและ dodge เข้าด้วยกัน
    public bool IsInvincible => iFrameTimer > 0f || movement.IsDodging;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        TickIFrame();
    }

    #endregion

    #region Methods — รับดาเมจและฮีล

    public void TakeDamage(float amount)
    {
        if (isDead || IsInvincible) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0f);
        iFrameTimer = damageIFrameDuration;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnDamaged?.Invoke();

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    #endregion

    #region Methods — การตาย

    private void Die()
    {
        isDead = true;
        OnDeath?.Invoke();
    }

    #endregion

    #region Methods — Invincibility Frame

    private void TickIFrame()
    {
        if (iFrameTimer > 0f)
            iFrameTimer -= Time.deltaTime;
    }

    #endregion
}
