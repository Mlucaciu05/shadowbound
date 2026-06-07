using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI; // Required for accessing the UI Slider component

public class UIGenericHealthBar : MonoBehaviour
{
    private Slider GenericHealthSlider;
    private Transform mainCameraTransform;
    private bool isWorldSpace = false;

    public GameObject bruh;

    void Awake()
    {
        GenericHealthSlider = GetComponent<Slider>();

        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null && parentCanvas.renderMode == RenderMode.WorldSpace)
        {
            isWorldSpace = true;
            mainCameraTransform = Camera.main.transform;
        }

        // AUTO-WIRING ENGINE: Automatically look up and bind to your GenericHealth stats
        GenericHealth parentGenericHealth = GetComponentInParent<GenericHealth>();
        if (parentGenericHealth == null)
        {
            parentGenericHealth = GameObject.FindGameObjectWithTag("Player")?.GetComponent<GenericHealth>();
        }

        if (parentGenericHealth != null)
        {
            // Subscribe to our take damage event directly via code
            parentGenericHealth.onTakeDamage.AddListener(UpdateGenericHealthDisplay);

            // Instantly fill the bar up to full on frame 1 so it's not black!
            GenericHealthSlider.maxValue = parentGenericHealth.maxHealth;
            GenericHealthSlider.value = parentGenericHealth.currentHealth;
        }
    }

    public void UpdateGenericHealthDisplay(float currentGenericHealth)
    {
        if (GenericHealthSlider != null)
        {
            GenericHealthSlider.value = currentGenericHealth;
        }
    }

    void LateUpdate()
    {
        if (isWorldSpace && mainCameraTransform != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - mainCameraTransform.position);
        }
    }
}