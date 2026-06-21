using NUnit.Framework;

public class ItemGeneratorTests
{
    [Test]
    public void Generate_Sword_SetsSwordStatsOnly()
    {
        ItemInstance item = ItemGenerator.Generate(ItemType.Sword, ItemRarity.Common, 5);

        Assert.Greater(item.damageBonus, 0f);
        Assert.Greater(item.strengthBonus, 0f);
        Assert.AreEqual(0f, item.defenseBonus);
        Assert.AreEqual(0f, item.healthBonus);
    }

    [Test]
    public void Generate_Shield_SetsShieldStatsOnly()
    {
        ItemInstance item = ItemGenerator.Generate(ItemType.Shield, ItemRarity.Common, 5);

        Assert.Greater(item.defenseBonus, 0f);
        Assert.Greater(item.healthBonus, 0f);
        Assert.AreEqual(0f, item.damageBonus);
        Assert.AreEqual(0f, item.strengthBonus);
    }

    [Test]
    public void Generate_LevelBelowOne_ClampsToOne()
    {
        ItemInstance zeroLevel = ItemGenerator.Generate(ItemType.Sword, ItemRarity.Common, 0);
        ItemInstance negativeLevel = ItemGenerator.Generate(ItemType.Sword, ItemRarity.Common, -10);

        Assert.AreEqual(1, zeroLevel.level);
        Assert.AreEqual(1, negativeLevel.level);
    }

    [Test]
    public void Generate_HigherRarity_ProducesHigherDamageBonus()
    {
        ItemInstance common = ItemGenerator.Generate(ItemType.Sword, ItemRarity.Common, 5);
        ItemInstance legendary = ItemGenerator.Generate(ItemType.Sword, ItemRarity.Legendary, 5);

        Assert.Greater(legendary.damageBonus, common.damageBonus);
    }

    [Test]
    public void Generate_Legendary_GrantsMagicPowerBonus()
    {
        ItemInstance legendarySword = ItemGenerator.Generate(ItemType.Sword, ItemRarity.Legendary, 5);
        ItemInstance rareSword = ItemGenerator.Generate(ItemType.Sword, ItemRarity.Rare, 5);

        Assert.Greater(legendarySword.magicPowerBonus, 0f);
        Assert.AreEqual(0f, rareSword.magicPowerBonus);
    }

    [Test]
    public void LevelForDifficulty_ReturnsValueWithinExpectedRange()
    {
        for (int i = 0; i < 50; i++)
        {
            Assert.That(ItemGenerator.LevelForDifficulty(BossDifficulty.SmallBoss), Is.InRange(1, 4));
            Assert.That(ItemGenerator.LevelForDifficulty(BossDifficulty.MediumBoss), Is.InRange(4, 7));
            Assert.That(ItemGenerator.LevelForDifficulty(BossDifficulty.HighBoss), Is.InRange(8, 12));
        }
    }
}
