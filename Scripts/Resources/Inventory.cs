using ARPG.Scripts.Gui;
using Godot;
using Godot.Collections;
using System.Linq;

namespace ARPG.Resources;

[GlobalClass]
public partial class Inventory : Resource
{
    [Signal]
    public delegate void InventoryUpdatedEventHandler();

    [Export]
    public Array<InventorySlot> Slots;

    private int lastUsedItemIndex = -1;

    public void Insert(InventoryItem item)
    {
        var amount = 1;
        var invType = item.GetType().ToString();

        GD.Print(invType);

        switch (invType)
        {
            case "ARPG.Resources.InventoryGem":
                amount = ((InventoryGem)item).Value;
                break;
        }

        // already have it
        var existing = Slots.Where(x => x.Item == item).FirstOrDefault();
        if (existing != null)
        {
            existing.Amount += amount;
            EmitSignal(SignalName.InventoryUpdated);
            return;
        }

        // new item
        var empty = Slots.Where(x => x.Item == null).FirstOrDefault();
        if (empty != null)
        {
            empty.Item = item;
            empty.Amount = amount;
            EmitSignal(SignalName.InventoryUpdated);
            return;
        }

        // no space
        GD.Print("Inv Full!");
    }

    public void RemoveSlot(InventorySlot inventorySlot)
    {
        var index = Slots.IndexOf(inventorySlot);
        if (index < 0)
        {
            return;
        }

        RemoveSlotAtIndex(index);
    }

    public void RemoveSlotAtIndex(int index)
    {
        Slots[index] = new InventorySlot();
        EmitSignal(SignalName.InventoryUpdated);
    }

    public void InsertSlot(int index, InventorySlot inventorySlot)
    {
        Slots[index] = inventorySlot;
        EmitSignal(SignalName.InventoryUpdated);
    }

    public void UseItemAtSelectedIndex(int index)
    {
        if (index < 0 || index >= Slots.Count || Slots[index].Item == null)
        {
            return;
        }

        var slot = Slots[index];
        lastUsedItemIndex = index;
        slot.Item.UseItem();
    }

    public void RemoveLastUsedItem()
    {
        if (lastUsedItemIndex < 0)
        {
            return;
        }

        var slot = Slots[lastUsedItemIndex];

        if (slot.Amount > 1)
        {
            slot.Amount--;
            EmitSignal(SignalName.InventoryUpdated);
            return;
        }

        RemoveSlotAtIndex(lastUsedItemIndex);
    }
}