using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Player player;
    float currentHealth;
    float maxHealth;


    private void Update()
    {
    
    }
    private void Start()
    {
        maxHealth = player.GetMaxHealth();
        currentHealth = maxHealth;
        fillImage.fillAmount = 1;
    }
    public void SetHealth(float health)
    {
        currentHealth = health;
        fillImage.fillAmount = currentHealth / maxHealth;
    }
}