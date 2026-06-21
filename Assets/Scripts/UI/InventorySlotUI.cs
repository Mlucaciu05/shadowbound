using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text statsText;
    public Image iconImage;
    public Image rarityFrame;
    public Button equipButton;

    private ItemInstance item;
    private InventoryUI inventoryUI;

    void Awake()
    {
        if (equipButton == null)
        {
            equipButton = GetComponent<Button>();
        }

        if (equipButton != null)
        {
            equipButton.onClick.AddListener(Equip);
        }
    }

    public void Setup(ItemInstance itemInstance, InventoryUI owner)
    {
        item = itemInstance;
        inventoryUI = owner;

        if (nameText != null)
        {
            nameText.text = item != null ? item.DisplayName : "Empty";
        }

        if (statsText != null)
        {
            statsText.text = item != null ? BuildStatsText(item) : "";
        }

        if (iconImage != null)
        {
            iconImage.sprite = item != null ? item.icon : null;
            iconImage.enabled = item != null && item.icon != null;
        }

        if (rarityFrame != null && item != null)
        {
            rarityFrame.color = GetRarityColor(item.rarity);
        }
    }

    private void Equip()
    {
        if (inventoryUI != null && item != null)
        {
            inventoryUI.Equip(item);
        }
    }

    private string BuildStatsText(ItemInstance itemInstance)
    {
        string stats = itemInstance.itemType + "  Lv." + itemInstance.level;

        if (itemInstance.damageBonus > 0f) stats += "\nDamage +" + Mathf.RoundToInt(itemInstance.damageBonus);
        if (itemInstance.defenseBonus > 0f) stats += "\nDefense +" + Mathf.RoundToInt(itemInstance.defenseBonus);
        if (itemInstance.strengthBonus > 0f) stats += "\nStrength +" + Mathf.RoundToInt(itemInstance.strengthBonus);
        if (itemInstance.magicPowerBonus > 0f) stats += "\nMagic +" + Mathf.RoundToInt(itemInstance.magicPowerBonus);
        if (itemInstance.criticalChanceBonus > 0f) stats += "\nCrit +" + Mathf.RoundToInt(itemInstance.criticalChanceBonus * 100f) + "%";

        return stats;
    }

    public static Color GetRarityColor(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common:
                return new Color(0.7f, 0.9f, 0.7f);
            case ItemRarity.Rare:
                return new Color(0.35f, 0.55f, 1f);
            case ItemRarity.Epic:
                return new Color(0.75f, 0.35f, 1f);
            case ItemRarity.Legendary:
                return new Color(1f, 0.6f, 0.15f);
            default:
                return Color.gray;
        }
    }
}
