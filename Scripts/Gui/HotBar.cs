using Godot;
using Godot.Collections;
using System.Data;
using System.Linq;

namespace ARPG.Scripts.Gui;

public partial class HotBar : Panel
{
   public Resources.Inventory PlayerInventory;
   public HBoxContainer Container;
   public Array<HotBarSlot> HotBarSlots;
   public Sprite2D Selector;

   private int selectedIndex = 0;

   public override void _Ready()
   {
      PlayerInventory = ResourceLoader.Load<Resources.Inventory>("res://Scenes/Inventory/PlayerInventory.tres");

      Container = GetNode<HBoxContainer>("Container");
      Selector = GetNode<Sprite2D>("Selector");

      var slots = Container.GetChildren().Cast<HotBarSlot>();
      HotBarSlots = [.. slots];

      PlayerInventory.InventoryUpdated += UpdateInventory;

      UpdateInventory();      
   }

   public void UpdateInventory()
   {
      for (int i = 0; i < HotBarSlots.Count; i++) {
         var inventorySlot = PlayerInventory.Slots[i];
         HotBarSlots[i].UpdateSlot(inventorySlot);
      }
   }

   public void ChangeSelection()
   {
      selectedIndex = (selectedIndex + 1) % HotBarSlots.Count;
      Selector.GlobalPosition = HotBarSlots[selectedIndex].GlobalPosition;
   }

   public override void _UnhandledInput(InputEvent @event)
   {
      base._UnhandledInput(@event);

      if (@event.IsActionPressed("ui_use"))
      {
         PlayerInventory.UseItemAtSelectedIndex(selectedIndex);
      }

      if (@event.IsActionPressed("ui_change_selection"))
      {
         ChangeSelection();
      }
   }
}
