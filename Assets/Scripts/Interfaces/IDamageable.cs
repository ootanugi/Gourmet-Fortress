/// <summary>
/// Interface สำหรับทุก object ที่รับดาเมจได้ — ทำให้ PlayerAttack ไม่ต้องรู้จัก EnemyBase โดยตรง
/// </summary>
public interface IDamageable
{
    void TakeDamage(float amount);
    bool IsDead { get; }
}
