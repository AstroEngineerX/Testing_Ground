using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Image damageFill;

    [Header("Animation")]
    [SerializeField] private float updateSpeed = 0.2f; // smooth bar speed
    [SerializeField] private float damageLagSpeed = 0.5f; // slower trailing effect

    private float targetFillAmount;

    public void SetHealth(int current, int max)
    {
        targetFillAmount = (float)current / max;
        healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, targetFillAmount, updateSpeed);
    }

    private void Update()
    {
        // Smoothly move health bar
        healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, targetFillAmount, Time.deltaTime * 10f);

        // Damage bar lags behind (like Dark Souls / Diablo style)
        if (damageFill.fillAmount > healthFill.fillAmount)
        {
            damageFill.fillAmount = Mathf.Lerp(damageFill.fillAmount, healthFill.fillAmount, Time.deltaTime * damageLagSpeed);
        }
        else
        {
            damageFill.fillAmount = healthFill.fillAmount;
        }

        // Optional: color gradient based on health %
        healthFill.color = Color.Lerp(Color.red, Color.green, healthFill.fillAmount);
    }
}
