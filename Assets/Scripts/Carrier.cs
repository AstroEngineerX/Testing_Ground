using UnityEngine;

public abstract class Carrier : MonoBehaviour, IDamageable
{
    [Header("BASE")]
    [Header("Parameters")]
    [SerializeField, Min(1)] private int maxHealth = 100;
    [SerializeField, Min(1)] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Hit Reaction")]
    [SerializeField] private float stunDuration = 0.5f; // time stunned after damage

    private Health _health;
    private Animator _animator;
    private float _lastAttackTime;

    private CharacterController _characterController;
    private Vector3 knockbackVelocity;
    private float stunTimer;

    protected Health Health => _health;

    protected virtual void Awake()
    {
        _health = new Health(0, maxHealth);
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
    }

    protected virtual void Update()
    {
        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;

            if (_characterController != null && knockbackVelocity.magnitude > 0.1f)
            {
                _characterController.Move(knockbackVelocity * Time.deltaTime);
                knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 5f);
            }
        }
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

        if (_animator != null)
        {
            _animator.SetTrigger("Hit");
        }

        stunTimer = stunDuration;

        if (_health.CurrentValue <= 0)
        {
            OnDeath();
        }
    }

    public virtual void HealthRegen(int healthAmount)
    {
        if (healthAmount <= 0) return;
        _health.AffectValue(healthAmount);

        Debug.Log($"{gameObject.name} healed {healthAmount}. Current health: {_health.CurrentValue}");
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        knockbackVelocity = direction.normalized * force;
        stunTimer = stunDuration;
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
        // Override in child to disable input
    }

    public bool IsStunned => stunTimer > 0;
}
