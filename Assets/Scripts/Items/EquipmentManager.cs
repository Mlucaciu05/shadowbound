using UnityEngine;
using UnityEngine.Events;

public class EquipmentManager : MonoBehaviour
{
    public PlayerInventory inventory;
    public PlayerStats playerStats;
    public ItemInstance equippedSword;
    public ItemInstance equippedShield;
    public UnityEvent onEquipmentChanged = new UnityEvent();

    void Awake()
    {
        if (inventory == null)
        {
            inventory = GetComponent<PlayerInventory>();
        }

        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }
    }

    public bool Equip(ItemInstance item)
    {
        if (item == null) return false;

        if (item.itemType == ItemType.Sword)
        {
            equippedSword = item;
        }
        else if (item.itemType == ItemType.Shield)
        {
            equippedShield = item;
        }
        else
        {
            return false;
        }

        NotifyEquipmentChanged();
        return true;
    }

    public void UnequipSword()
    {
        equippedSword = null;
        NotifyEquipmentChanged();
    }

    public void UnequipShield()
    {
        equippedShield = null;
        NotifyEquipmentChanged();
    }

    public bool HasShieldEquipped()
    {
        return equippedShield != null;
    }

    public float GetWeaponDamageBonus()
    {
        return equippedSword != null ? equippedSword.damageBonus : 0f;
    }

    public float GetShieldDefenseBonus()
    {
        return equippedShield != null ? equippedShield.defenseBonus : 0f;
    }

    private void NotifyEquipmentChanged()
    {
        if (playerStats != null)
        {
            playerStats.RecalculateStats();
        }

        if (inventory != null)
        {
            inventory.RefreshUI();
        }

        onEquipmentChanged?.Invoke();
    }
}
