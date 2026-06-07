using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    public GameObject owner;
    public PlayerCombat combat;

    [Header("Damage Settings")]
    public float damageDealt = 20f;
    public float damageMultiplier = 1f;

    public GameObject dmgPopupPrefab;

    private List<GameObject> hitEnemies = new List<GameObject>();

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetHitList()
    {
        hitEnemies.Clear();
    }

    public float ProcessDamage(float damageDealt, float damageMultiplier)
    {
        float randomisedDmg = Random.Range(damageDealt - 3, damageDealt + 1);


        if(combat.currentAttackType == "light")
        {
            damageMultiplier *= 0.7f;
        }
        else if(combat.currentAttackType == "block-attack")
        {
            damageMultiplier *= 0.3f;
        }
        else if( combat.currentAttackType == "heavy")
        {
            damageMultiplier *= 1.2f;
        }

            return (int)Mathf.Round(randomisedDmg * damageMultiplier);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner) return;

        if (hitEnemies.Contains(other.gameObject)) { return; }

        GenericHealth enemy = other.GetComponent<GenericHealth>();
        if (enemy == null) return;

        if (other.gameObject.CompareTag("Enemy")) { 
        float finalDmg = ProcessDamage(damageDealt, damageMultiplier);

            Debug.Log($"HIT");
            hitEnemies.Add(other.gameObject);
            enemy.TakeDamage(finalDmg);

            if (dmgPopupPrefab != null)
            {
                // 1. Core target position (center of enemy)
                Vector3 spawnPosition = other.transform.position;

                // 2. Vertical offset: push it up 1.8 meters (roughly head height)
                spawnPosition.y += 1.8f;

                // 3. Horizontal Offset: Find which way is "Right" from the camera's perspective
                if (Camera.main != null)
                {
                    Vector3 cameraRight = Camera.main.transform.right;
                    cameraRight.y = 0f; // Keep it purely horizontal on the flat plane

                    // Randomly choose to pop up on the left (-1) or right (+1) side of the enemy shoulder
                    float sideChooser = Random.value > 0.5f ? 1f : -1f;
                    float horizontalDistance = 1.5f; // How many meters to the side it shoots out

                    spawnPosition += cameraRight.normalized * (horizontalDistance * sideChooser);
                }

                // 4. Spawn the number in the clean calculated air space
                GameObject popupInstance = Instantiate(dmgPopupPrefab, spawnPosition, Quaternion.identity);
                DamagePopup popupScript = popupInstance.GetComponent<DamagePopup>();

                if (popupScript != null)
                {
                    popupScript.Setup(finalDmg);
                }
            }

        }
    }
}
