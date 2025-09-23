using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damageAmount);
    void HealthRegen(int healthAmount);
}
