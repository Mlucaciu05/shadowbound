using NUnit.Framework;
using UnityEngine;

public class EquipmentManagerTests
{
    private GameObject host;
    private EquipmentManager equipment;

    [SetUp]
    public void SetUp()
    {
        host = new GameObject("EquipmentManager (test)");
        equipment = host.AddComponent<EquipmentManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(host);
    }

    [Test]
    public void Equip_Sword_SetsEquippedSword()
    {
        ItemInstance sword = new ItemInstance { itemType = ItemType.Sword, damageBonus = 15f };

        bool equipped = equipment.Equip(sword);

        Assert.IsTrue(equipped);
        Assert.AreEqual(sword, equipment.equippedSword);
        Assert.AreEqual(15f, equipment.GetWeaponDamageBonus());
    }

    [Test]
    public void Equip_Shield_SetsEquippedShield()
    {
        ItemInstance shield = new ItemInstance { itemType = ItemType.Shield, defenseBonus = 20f };

        bool equipped = equipment.Equip(shield);

        Assert.IsTrue(equipped);
        Assert.IsTrue(equipment.HasShieldEquipped());
        Assert.AreEqual(20f, equipment.GetShieldDefenseBonus());
    }

    [Test]
    public void Equip_NonEquippableItemType_ReturnsFalse()
    {
        ItemInstance consumable = new ItemInstance { itemType = ItemType.Consumable };

        bool equipped = equipment.Equip(consumable);

        Assert.IsFalse(equipped);
    }

    [Test]
    public void Equip_Null_ReturnsFalse()
    {
        Assert.IsFalse(equipment.Equip(null));
    }

    [Test]
    public void UnequipSword_ClearsEquippedSwordAndDamageBonus()
    {
        equipment.Equip(new ItemInstance { itemType = ItemType.Sword, damageBonus = 15f });

        equipment.UnequipSword();

        Assert.IsNull(equipment.equippedSword);
        Assert.AreEqual(0f, equipment.GetWeaponDamageBonus());
    }

    [Test]
    public void UnequipShield_ClearsHasShieldEquipped()
    {
        equipment.Equip(new ItemInstance { itemType = ItemType.Shield, defenseBonus = 20f });

        equipment.UnequipShield();

        Assert.IsFalse(equipment.HasShieldEquipped());
    }
}
