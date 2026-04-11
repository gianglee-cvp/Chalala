using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Character player;
    [SerializeField] private float smoothSpeed = 5f;

    float currentHealth;
    float maxHealth;
    private float targetFill;

    private void Start()
    {
        maxHealth = player.GetMaxHealth();
        currentHealth = maxHealth;
        targetFill = 1f;
        fillImage.fillAmount = 1f;
    }

    private void Update()
    {
        // Lerp mượt tới target
        fillImage.fillAmount = Mathf.Lerp(
            fillImage.fillAmount,
            targetFill,
            Time.deltaTime * smoothSpeed
        );
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
        targetFill = currentHealth / maxHealth;
    }
}