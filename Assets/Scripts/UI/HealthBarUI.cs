using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GenericHealth targetHealth;
    public Slider healthSlider;
    public bool followWorldTarget = false;
    public Vector3 worldOffset = new Vector3(0f, 2f, 0f);
    public Camera uiCamera;

    void Awake()
    {
        if (healthSlider == null)
        {
            healthSlider = GetComponent<Slider>();
        }

        if (uiCamera == null)
        {
            uiCamera = Camera.main;
        }
    }

    void OnEnable()
    {
        Bind(targetHealth);
    }

    public void Bind(GenericHealth health)
    {
        if (targetHealth != null)
        {
            targetHealth.onTakeDamage.RemoveListener(UpdateValue);
        }

        targetHealth = health;

        if (targetHealth != null)
        {
            targetHealth.onTakeDamage.AddListener(UpdateValue);
            if (healthSlider != null)
            {
                healthSlider.maxValue = targetHealth.maxHealth;
            }

            UpdateValue(targetHealth.currentHealth);
        }
    }

    void LateUpdate()
    {
        if (!followWorldTarget || targetHealth == null || uiCamera == null) return;

        transform.position = uiCamera.WorldToScreenPoint(targetHealth.transform.position + worldOffset);
    }

    private void UpdateValue(float currentHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }
}
