using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Required for TextMeshPro

public class DamagePopup : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private float disappearTimer = 0.8f;
    private Color textColor;
    private Vector3 moveVector;

    private Transform mainCameraTransform;

    public void Setup(float damageAmount)
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();

        // Format to whole integer string (e.g., "14" instead of "14.234")
        textMesh.SetText(Mathf.RoundToInt(damageAmount).ToString());

        textColor = textMesh.color;

        // Randomize the float direction slightly so multiple numbers don't overlap perfectly
        moveVector = new Vector3(Random.Range(-0.7f, 0.7f), 2f, Random.Range(-0.7f, 0.7f));

        mainCameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        // 1. Float Upward
        transform.position += moveVector * Time.deltaTime;

        // 2. Billboarding: Force text to always look flat at the spectator camera
        if (mainCameraTransform != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - mainCameraTransform.position);
        }

        // 3. Fade Out and Shrink
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float fadeSpeed = 4f;
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;

            // Shrink over time
            transform.localScale -= Vector3.one * 1f * Time.deltaTime;

            if (textColor.a <= 0)
            {
                Destroy(gameObject); // Clean up memory completely
            }
        }
    }
}