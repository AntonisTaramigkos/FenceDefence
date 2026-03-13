using UnityEngine;
using UnityEngine.UI;

public class HealthBarTestScript : MonoBehaviour
{
    public Slider slider;
    [SerializeField] int maxHealth = 100;

    public void SetMaxValue(int maxHealth)
    {
        slider.maxValue = maxHealth; 
        slider.value = maxHealth;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
