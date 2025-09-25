using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private bool destroyOnHit = false;
    [SerializeField] private bool continuousDamage = false;
    [SerializeField] private float damageInterval = 1f;

    [Header("Knockback Settings")]
    [SerializeField] private bool applyKnockback = true;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackUpward = 2f;

    [Header("Impact FX")]
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private AudioClip hitSound;

    private float _lastDamageTime;
    private AudioSource _audioSource;

    private void Awake()
    {
        if (hitSound != null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }
    }

    private void ApplyDamageAndKnockback(IDamageable target, Collider other)
    {
        target.TakeDamage(damageAmount);

        if (applyKnockback)
        {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            direction.y += knockbackUpward;

            if (other.attachedRigidbody != null)
            {
                other.attachedRigidbody.AddForce(direction * knockbackForce, ForceMode.Impulse);
            }
            else if (other.TryGetComponent<Carrier>(out var carrier))
            {
                carrier.ApplyKnockback(direction, knockbackForce);
            }
        }

        if (hitParticles != null)
        {
            Instantiate(hitParticles, other.transform.position, Quaternion.identity);
        }

        if (_audioSource != null && hitSound != null)
        {
            _audioSource.PlayOneShot(hitSound);
        }

        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!continuousDamage && other.TryGetComponent<IDamageable>(out var target))
        {
            ApplyDamageAndKnockback(target, other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (continuousDamage && other.TryGetComponent<IDamageable>(out var target))
        {
            if (Time.time >= _lastDamageTime + damageInterval)
            {
                ApplyDamageAndKnockback(target, other);
                _lastDamageTime = Time.time;
            }
        }
    }
}
