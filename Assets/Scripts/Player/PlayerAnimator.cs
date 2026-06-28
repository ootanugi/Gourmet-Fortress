using UnityEngine;

/// <summary>
/// จัดการ Animation ของผู้เล่นโดยอ่านสถานะจาก PlayerMovement
/// ใช้ Blend Tree 2D แบบ Freeform Directional สำหรับ 8 ทิศทาง
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerAnimator : MonoBehaviour
{
    #region Variables — การปรับความลื่นไหล

    // SmoothDamp ป้องกัน animation กระตุกเมื่อเปลี่ยนทิศกะทันหัน
    [SerializeField] private float directionSmoothTime = 0.08f;

    #endregion

    #region Variables — Animator Parameter Hashes

    // แคช hash ไว้ล่วงหน้าแทนการส่ง string ทุก frame เพื่อประสิทธิภาพ
    private static readonly int HashMoveX     = Animator.StringToHash("MoveX");
    private static readonly int HashMoveY     = Animator.StringToHash("MoveY");
    private static readonly int HashSpeed     = Animator.StringToHash("Speed");
    private static readonly int HashIsDodging = Animator.StringToHash("IsDodging");

    #endregion

    #region Variables — สถานะภายใน

    private Animator animator;
    private PlayerMovement movement;

    private Vector2 smoothedDirection;
    private Vector2 directionVelocity;

    #endregion

    #region Properties

    public Vector2 FacingDirection => smoothedDirection;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        UpdateDirectionParameters();
        UpdateSpeedParameter();
        UpdateDodgeParameter();
    }

    #endregion

    #region Methods — อัปเดต Animator Parameters

    private void UpdateDirectionParameters()
    {
        // ทิศทางเป้าหมาย: ถ้ากำลังเคลื่อนที่ใช้ input, ถ้าหยุดใช้ทิศล่าสุด
        Vector2 targetDirection = movement.IsMoving
            ? movement.MoveInput.normalized
            : movement.LastMoveDirection;

        smoothedDirection = Vector2.SmoothDamp(
            smoothedDirection,
            targetDirection,
            ref directionVelocity,
            directionSmoothTime
        );

        animator.SetFloat(HashMoveX, smoothedDirection.x);
        animator.SetFloat(HashMoveY, smoothedDirection.y);
    }

    private void UpdateSpeedParameter()
    {
        // Speed = 0 → Idle, 0–1 → Walk, >1 → Run (Blend Tree จะจัดการ transition)
        animator.SetFloat(HashSpeed, movement.MoveInput.magnitude);
    }

    private void UpdateDodgeParameter()
    {
        animator.SetBool(HashIsDodging, movement.IsDodging);
    }

    #endregion
}
