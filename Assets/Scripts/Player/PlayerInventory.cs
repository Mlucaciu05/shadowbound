using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ItemInstanceEvent : UnityEvent<ItemInstance>
{
}

public class PlayerInventory : MonoBehaviour
{
    public KeyCode toggleInventoryKey = KeyCode.I;
    public List<ItemInstance> items = new List<ItemInstance>();
    public EquipmentManager equipmentManager;
    public InventoryUI inventoryUI;
    public UnityEvent onInventoryChanged = new UnityEvent();
    public ItemInstanceEvent onItemAdded = new ItemInstanceEvent();

    void Awake()
    {
        if (equipmentManager == null)
        {
            equipmentManager = GetComponent<EquipmentManager>();
        }

        if (inventoryUI == null)
        {
            inventoryUI = FindObjectOfType<InventoryUI>();
        }
    }

    void Start()
    {
        RefreshUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleInventoryKey))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (inventoryUI != null)
        {
            inventoryUI.Toggle();
        }
    }

    public void AddItem(ItemData itemData)
    {
        if (itemData == null) return;
        AddItem(itemData.CreateInstance());
    }

    public void AddItem(ItemInstance item)
    {
        if (item == null) return;

        ItemInstance storedItem = item.Clone();
        items.Add(storedItem);
        onItemAdded?.Invoke(storedItem);
        onInventoryChanged?.Invoke();
        RefreshUI();
    }

    public bool EquipItem(ItemInstance item)
    {
        if (equipmentManager == null || item == null) return false;

        bool equipped = equipmentManager.Equip(item);
        if (equipped)
        {
            onInventoryChanged?.Invoke();
            RefreshUI();
        }

        return equipped;
    }

    public void RefreshUI()
    {
        if (inventoryUI != null)
        {
            inventoryUI.Refresh(this);
        }
    }
}
