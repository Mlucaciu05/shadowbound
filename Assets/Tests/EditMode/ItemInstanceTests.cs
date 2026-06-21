using NUnit.Framework;

public class ItemInstanceTests
{
    [Test]
    public void DisplayName_WithExplicitName_ReturnsThatName()
    {
        ItemInstance item = new ItemInstance { itemName = "Sword of Dawn" };

        Assert.AreEqual("Sword of Dawn", item.DisplayName);
    }

    [Test]
    public void DisplayName_WithNoName_FallsBackToRarityAndType()
    {
        ItemInstance item = new ItemInstance { itemType = ItemType.Sword, rarity = ItemRarity.Rare };

        Assert.AreEqual("Rare Sword", item.DisplayName);
    }

    [Test]
    public void Clone_ProducesIndependentCopy()
    {
        ItemInstance original = new ItemInstance { itemName = "Original", damageBonus = 10f };
        ItemInstance clone = original.Clone();
        clone.itemName = "Modified";
        clone.damageBonus = 99f;

        Assert.AreEqual("Original", original.itemName);
        Assert.AreEqual(10f, original.damageBonus);
        Assert.AreEqual("Modified", clone.itemName);
        Assert.AreEqual(99f, clone.damageBonus);
    }

    [Test]
    public void GetRarityMultiplier_IncreasesWithRarity()
    {
        float basic = ItemInstance.GetRarityMultiplier(ItemRarity.Basic);
        float common = ItemInstance.GetRarityMultiplier(ItemRarity.Common);
        float rare = ItemInstance.GetRarityMultiplier(ItemRarity.Rare);
        float epic = ItemInstance.GetRarityMultiplier(ItemRarity.Epic);
        float legendary = ItemInstance.GetRarityMultiplier(ItemRarity.Legendary);

        Assert.Less(basic, common);
        Assert.Less(common, rare);
        Assert.Less(rare, epic);
        Assert.Less(epic, legendary);
    }
}
