using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Transform itemContainer;
    public InventorySlotUI itemSlotPrefab;
    public TMP_Text equippedSwordText;
    public TMP_Text equippedShieldText;
    public TMP_Text statsPreviewText;

    private PlayerInventory playerInventory;

    void Awake()
    {
        if (inventoryPanel == null)
        {
            inventoryPanel = gameObject;
        }

        inventoryPanel.SetActive(false);
    }

    public void Toggle()
    {
        if (inventoryPanel == null) return;
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);

        if (inventoryPanel.activeSelf && playerInventory != null)
        {
            Refresh(playerInventory);
        }
    }

    public void Refresh(PlayerInventory inventory)
    {
        playerInventory = inventory;
        ClearSlots();

        if (playerInventory == null) return;

        if (itemContainer != null && itemSlotPrefab != null)
        {
            foreach (ItemInstance item in playerInventory.items)
            {
                InventorySlotUI slot = Instantiate(itemSlotPrefab, itemContainer);
                slot.Setup(item, this);
            }
        }

        EquipmentManager equipment = playerInventory.equipmentManager;

        if (equippedSwordText != null)
        {
            equippedSwordText.text = equipment != null && equipment.equippedSword != null ? equipment.equippedSword.DisplayName : "No sword equipped";
        }

        if (equippedShieldText != null)
        {
            equippedShieldText.text = equipment != null && equipment.equippedShield != null ? equipment.equippedShield.DisplayName : "No shield equipped";
        }

        UpdateStatsPreview(equipment != null ? equipment.playerStats : playerInventory.GetComponent<PlayerStats>());
    }

    public void Equip(ItemInstance item)
    {
        if (playerInventory == null || item == null) return;
        playerInventory.EquipItem(item);
    }

    private void ClearSlots()
    {
        if (itemContainer == null) return;

        for (int i = itemContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(itemContainer.GetChild(i).gameObject);
        }
    }

    private void UpdateStatsPreview(PlayerStats stats)
    {
        if (statsPreviewText == null || stats == null) return;

        statsPreviewText.text =
            "Strength: " + Mathf.RoundToInt(stats.strength) +
            "\nDefense: " + Mathf.RoundToInt(stats.defense) +
            "\nMagic: " + Mathf.RoundToInt(stats.magicPower) +
            "\nCrit: " + Mathf.RoundToInt(stats.criticalChance * 100f) + "%" +
            "\nBlock: " + Mathf.RoundToInt(stats.blockReduction * 100f) + "%";
    }
}
