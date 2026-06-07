using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorCamera : MonoBehaviour
{
    private const float YMin = -50.0f;
    private const float YMax = 50.0f;

    public Transform lookAt;
    public float distance = 10.0f;
    public float sensivity = 4.0f;

    [Header("Collision Settings")]
    public LayerMask collisionLayers; // Select what layers block the camera (Default, Ground, Wall, etc.)
    public float cameraRadius = 0.2f;  // Keeps the camera lens from clipping slightly through walls

    private float currentX = 0.0f;
    private float currentY = 0.0f;

    void Start()
    {
        // Lock and hide the cursor so it doesn't wander off the game screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (lookAt == null) return;

        // 1. Gather mouse inputs
        currentX += Input.GetAxis("Mouse X") * sensivity * Time.deltaTime;
        currentY -= Input.GetAxis("Mouse Y") * sensivity * Time.deltaTime; // Inverted subtraction so dragging mouse UP tilts camera UP

        currentY = Mathf.Clamp(currentY, YMin, YMax);

        // 2. Calculate the intended target orbital positioning
        Vector3 targetDirection = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 intendedPosition = lookAt.position + rotation * targetDirection;

        // 3. Collision Prevention: Raycast out to see if the path is clear
        Vector3 rayDirection = intendedPosition - lookAt.position;
        RaycastHit hitInfo;

        // Using SphereCast instead of a standard thin Raycast to simulate camera thickness 
        // so corners don't clip into walls!
        if (Physics.SphereCast(lookAt.position, cameraRadius, rayDirection.normalized, out hitInfo, distance, collisionLayers))
        {
            // Snap camera right to the obstacle point, pulling it slightly closer to prevent clipping textures
            transform.position = lookAt.position + rayDirection.normalized * (hitInfo.distance - 0.05f);
        }
        else
        {
            // Path is fully clear, move to target orbit node
            transform.position = intendedPosition;
        }

        // 4. Force the lens system to stare back at the pivot target
        transform.LookAt(lookAt.position);
    }
}