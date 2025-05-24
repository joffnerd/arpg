using Godot;
using Godot.Collections;
using System.Data;
using System.Linq;

namespace ARPG.Scripts.Gui;

public partial class HotBar : HBoxContainer
{
   public Resources.Inventory PlayerInventory;
   public Array<HotBarSlot> HotBarSlots;

   public override void _Ready()
   {
      PlayerInventory = ResourceLoader.Load<Resources.Inventory>("res://Scenes/Inventory/PlayerInventory.tres");

      var slots = GetChildren().Cast<HotBarSlot>();
      HotBarSlots = [.. slots];
   }

   public void Update()
   {
      for (int i = 0; i < HotBarSlots.Count; i++) {
         var inventorySlot = PlayerInventory.Slots[i];
         HotBarSlots[i].UpdateSlot(inventorySlot);
      }
   }
}
