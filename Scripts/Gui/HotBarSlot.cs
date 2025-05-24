using ARPG.Resources;
using Godot;

namespace ARPG.Scripts.Gui;

public partial class HotBarSlot : Button
{
   public Sprite2D Background;
   public ItemStack ItemStack;

   public override void _Ready()
   {
      Background = GetNode<Sprite2D>("Background");
      ItemStack = GetNode<ItemStack>("CenterContainer/ItemStack");
   }

   public void UpdateSlot(InventorySlot slot)
   {
      if(slot.Item == null)
      {
         ItemStack.Visible = false;
         Background.Frame = 0;
         return;
      }

      ItemStack.inventorySlot = slot;
      ItemStack.Update();
      ItemStack.Visible = true;

      Background.Frame = 1;
   }
}
