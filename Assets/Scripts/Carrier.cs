using UnityEngine;

public abstract class Carrier : MonoBehaviour, IDamageable
{
    [Header("BASE")]
    [Header("Parameters")]
    [SerializeField, Min(1)] private int maxHealth = 1;
    [SerializeField, Min(1)] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 1f;

    private Health _health;
    private Animator _animator;
    private float _lastAttackTime;

    protected Health Health => _health;

    protected virtual void Awake()
    {
        _health = new Health(0, maxHealth);
        _animator = GetComponent<Animator>();
    }

    public virtual void Attack(IDamageable target)
    {
        if (Time.time < _lastAttackTime + attackCooldown) return;

        _lastAttackTime = Time.time;

        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
        }

        target.TakeDamage(attackDamage);
    }

    public virtual void TakeDamage(int damageAmount)
    {
        if (damageAmount <= 0) return;

        _health.AffectValue(-damageAmount);

        if (_health.CurrentValue <= 0)
        {
            OnDeath();
        }
    }

    public virtual void HealthRegen(int healthAmount)
    {
        if (healthAmount <= 0) return;

        _health.AffectValue(healthAmount);
    }


    protected virtual void OnDeath()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Die");
        }

        DisableControls();
    }

    protected virtual void DisableControls()
    {
        
    }
}
