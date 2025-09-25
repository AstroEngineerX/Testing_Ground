using UnityEngine;

public class HealingCube : MonoBehaviour, IPickable
{
    [Header("Healing Settings")]
    [SerializeField] private int healAmount = 20;

    [Header("Visual FX")]
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float bobbingHeight = 0.25f;
    [SerializeField] private float bobbingSpeed = 2f;

    [Header("Glow Settings")]
    [SerializeField] private Color glowColor = Color.green;
    [SerializeField] private float glowIntensity = 2f;

    [Header("Pickup FX")]
    [SerializeField] private ParticleSystem pickupParticles;
    [SerializeField] private AudioClip pickupSound;

    private Vector3 _startPosition;
    private Renderer _renderer;
    private Material _material;
    private AudioSource _audioSource;

    private void Start()
    {
        _startPosition = transform.position;

        // Setup glow
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            _material = _renderer.material;
            _material.EnableKeyword("_EMISSION");
            _material.SetColor("_EmissionColor", glowColor * glowIntensity);
        }

        // Setup audio
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }

    private void Update()
    {
        // Rotate around Y-axis
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        // Bobbing effect
        float newY = _startPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public void OnPickUp(Carrier carrier)
    {
        carrier.HealthRegen(healAmount);

        // Play pickup FX
        if (pickupParticles != null)
        {
            Instantiate(pickupParticles, transform.position, Quaternion.identity);
        }

        if (pickupSound != null)
        {
            _audioSource.PlayOneShot(pickupSound);
        }

        // Destroy after sound finishes
        Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Carrier>(out var carrier))
        {
            OnPickUp(carrier);
        }
    }
}
