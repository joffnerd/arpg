using Godot;

namespace ARPG.Scripts.Gui;

public partial class GuiRoot : CanvasLayer
{
   public Inventory Inventory;

   public override void _Ready()
   {
      Inventory = GetNode<Inventory>("Inventory");
      Inventory.Close();
   }

   public override void _Input(InputEvent @event)
   {
      base._Input(@event);

      if (@event.IsActionPressed("ui_inv"))
      {
         if (Inventory.isOpen)
         {
            Inventory.Close();
         }
         else
         {
            Inventory.Open();
         }
      }
   }
}
