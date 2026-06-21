using NUnit.Framework;
using UnityEngine;

public class PlayerInventoryTests
{
    private GameObject host;
    private PlayerInventory inventory;

    [SetUp]
    public void SetUp()
    {
        host = new GameObject("PlayerInventory (test)");
        host.AddComponent<EquipmentManager>();
        inventory = host.AddComponent<PlayerInventory>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(host);
    }

    [Test]
    public void AddItem_AddsToItemsList()
    {
        inventory.AddItem(new ItemInstance { itemName = "Iron Sword", itemType = ItemType.Sword });

        Assert.AreEqual(1, inventory.items.Count);
        Assert.AreEqual("Iron Sword", inventory.items[0].itemName);
    }

    [Test]
    public void AddItem_StoresAClone_NotTheOriginalReference()
    {
        ItemInstance original = new ItemInstance { itemName = "Iron Sword" };

        inventory.AddItem(original);
        original.itemName = "Changed After Adding";

        Assert.AreEqual("Iron Sword", inventory.items[0].itemName);
    }

    [Test]
    public void AddItem_Null_DoesNotAddAnything()
    {
        inventory.AddItem((ItemInstance)null);

        Assert.AreEqual(0, inventory.items.Count);
    }

    [Test]
    public void AddItem_InvokesOnItemAddedEvent()
    {
        ItemInstance received = null;
        inventory.onItemAdded.AddListener(item => received = item);

        inventory.AddItem(new ItemInstance { itemName = "Iron Sword" });

        Assert.IsNotNull(received);
        Assert.AreEqual("Iron Sword", received.itemName);
    }

    [Test]
    public void EquipItem_EquippableType_ReturnsTrueAndEquips()
    {
        ItemInstance sword = new ItemInstance { itemType = ItemType.Sword, damageBonus = 12f };

        bool equipped = inventory.EquipItem(sword);

        Assert.IsTrue(equipped);
        Assert.AreEqual(sword, inventory.equipmentManager.equippedSword);
    }
}
