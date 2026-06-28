using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ควบคุมการเคลื่อนที่ของผู้เล่นในรูปแบบ Top-Down
/// รองรับการเดิน, วิ่ง, และการกลิ้งหลบ (Dodge Roll)
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    #region Variables — การเคลื่อนที่

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeedMultiplier = 1.6f;

    #endregion

    #region Variables — การกลิ้งหลบ

    // ระยะเวลาที่ dodge อยู่ในสถานะ invincible และแรงที่ใช้พุ่ง
    [SerializeField] private float dodgeForce = 12f;
    [SerializeField] private float dodgeDuration = 0.2f;
    [SerializeField] private float dodgeCooldown = 0.8f;
    [SerializeField] private LayerMask enemyLayer;

    #endregion

    #region Variables — สถานะภายใน

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.down;

    [SerializeField] private bool isRunning;
    [SerializeField] private bool isDodging;
    [SerializeField] private float dodgeCooldownTimer;

    #endregion

    #region Properties

    public Vector2 MoveInput => moveInput;
    public Vector2 LastMoveDirection => lastMoveDirection;
    public bool IsDodging => isDodging;
    public bool IsMoving => moveInput.sqrMagnitude > 0.01f;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        TickDodgeCooldown();
    }

    private void FixedUpdate()
    {
        if (isDodging) return;

        ApplyMovement();
    }

    #endregion

    #region Methods — Input (เรียกจาก PlayerInput component)

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        // จำทิศทางล่าสุดเพื่อใช้ใน dodge และ animation เมื่อหยุดนิ่ง
        if (moveInput.sqrMagnitude > 0.01f)
            lastMoveDirection = moveInput.normalized;
    }
     
    public void OnRun(InputValue value)
    {
        isRunning = value.isPressed;
    }

    public void OnDodge(InputValue value)
    {
        if (value.isPressed)
            TryDodge();
    }

    #endregion

    #region Methods — การเคลื่อนที่

    private void ApplyMovement()
    {
        float speed = isRunning ? moveSpeed * runSpeedMultiplier : moveSpeed;
        rb.velocity = moveInput.normalized * speed;
    }

    #endregion

    #region Methods — การกลิ้งหลบ

    private void TryDodge()
    {
        // ยังอยู่ใน cooldown หรือกำลัง dodge อยู่ ให้ข้ามไป
        if (isDodging || dodgeCooldownTimer > 0f) return;

        StartCoroutine(DodgeRoutine());
    }

    private System.Collections.IEnumerator DodgeRoutine()
    {
        isDodging = true;
        dodgeCooldownTimer = dodgeCooldown;
        rb.excludeLayers = enemyLayer;

        // พุ่งไปในทิศที่กำลังกด หรือทิศที่หันหน้าอยู่ถ้าไม่ได้กดทิศทาง
        Vector2 dodgeDirection = IsMoving ? moveInput.normalized : lastMoveDirection;
        rb.velocity = dodgeDirection * dodgeForce;

        yield return new WaitForSeconds(dodgeDuration);

        isDodging = false;
        rb.excludeLayers = default;
    }

    private void TickDodgeCooldown()
    {
        if (dodgeCooldownTimer > 0f)
            dodgeCooldownTimer -= Time.deltaTime;
    }

    #endregion
}
