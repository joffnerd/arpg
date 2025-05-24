using Godot;
using Godot.Collections;

namespace ARPG.Resources;

[GlobalClass]
public partial class Inventory : Resource
{
   [Signal]
   public delegate void InventoryUpdatedEventHandler();

   [Export]
   public Array<InventorySlot> Slots;

   public void Insert(InventoryItem item)
   {
      foreach (InventorySlot slot in Slots)
      {
         if (slot.Item == item)
         {
            slot.Amount++;
            EmitSignal(SignalName.InventoryUpdated);
            return;
         }
      }

      for (int i = 0; i < Slots.Count; i++)
      {
         if (Slots[i].Item == null)
         {
            Slots[i].Item = item;
            Slots[i].Amount = 1;
            EmitSignal(SignalName.InventoryUpdated);
            return;
         }
      }
   }

   public void RemoveSlot(InventorySlot inventorySlot)
   {
      var index = Slots.IndexOf(inventorySlot);
      if (index < 0) { return; }

      Slots[index] = new InventorySlot();
      EmitSignal(SignalName.InventoryUpdated);
   }

   public void InsertSlot(int index, InventorySlot inventorySlot)
   {
      Slots[index] = inventorySlot;
      EmitSignal(SignalName.InventoryUpdated);
   }
}
