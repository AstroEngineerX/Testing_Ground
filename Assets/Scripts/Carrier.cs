using UnityEngine;

public abstract class Carrier : MonoBehaviour, IDamageable
{
    [Header("BASE")]
    [Header("Parameters")]
    [SerializeField, Min(1)] private int maxHealth = 100;
    [SerializeField, Min(1)] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Hit Reaction")]
    [SerializeField] private float stunDuration = 0.5f;

    [Header("UI References")]
    [SerializeField] private HealthBar healthBar;

    private Health _health;
    private Animator _animator;
    private float _lastAttackTime;

    private CharacterController _characterController;
    private Vector3 knockbackVelocity;
    private float stunTimer;

    [SerializeField] private int currentHealthDebug; // visible in inspector

    protected Health Health => _health;

    public int CurrentHealth => _health != null ? _health.CurrentValue : 0;
    public int MaxHealth => _health != null ? _health.MaxValue : maxHealth;

    protected virtual void Awake()
    {
        _health = new Health(0, maxHealth);
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();

        currentHealthDebug = _health.CurrentValue;

        if (healthBar != null)
        {
            healthBar.SetHealth(CurrentHealth, MaxHealth);
        }
    }

    protected virtual void Update()
    {
        currentHealthDebug = _health.CurrentValue;

        if (healthBar != null)
        {
            healthBar.SetHealth(CurrentHealth, MaxHealth);
        }

        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;

            if (_characterController != null && knockbackVelocity.magnitude > 0.1f)
            {
                _characterController.Move(knockbackVelocity * Time.deltaTime);
                knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 5f);
            }
        }

        if (_animator != null)
        {
            if (Input.GetKeyDown(KeyCode.E)) _animator.SetTrigger("MeleeAttack");
            if (Input.GetMouseButtonDown(0)) _animator.SetTrigger("MagicAttack");
        }
    }

    public virtual void Attack(IDamageable target)
    {
        if (Time.time < _lastAttackTime + attackCooldown) return;

        _lastAttackTime = Time.time;
        target.TakeDamage(attackDamage);
    }

    public virtual void TakeDamage(int damageAmount)
    {
        if (damageAmount <= 0) return;

        _health.AffectValue(-damageAmount);
        currentHealthDebug = _health.CurrentValue;

        if (healthBar != null)
        {
            healthBar.SetHealth(CurrentHealth, MaxHealth);
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
        currentHealthDebug = _health.CurrentValue;

        if (healthBar != null)
        {
            healthBar.SetHealth(CurrentHealth, MaxHealth);
        }

        Debug.Log($"{gameObject.name} healed {healthAmount}. Current health: {_health.CurrentValue}");
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        knockbackVelocity = direction.normalized * force;
        stunTimer = stunDuration;
    }

    protected virtual void OnDeath()
    {
        DisableControls();
    }

    protected virtual void DisableControls()
    {
        // Override in child to disable input
    }

    public bool IsStunned => stunTimer > 0;
}
