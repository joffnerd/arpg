using ARPG.Resources;
using Godot;

namespace ARPG.Scripts.Gui;

public partial class ItemStack : Panel
{   
   public Sprite2D Item;
   public Label Amount;

   public InventorySlot inventorySlot;

   public override void _Ready()
   {
      Item = GetNode<Sprite2D>("Item");
      Amount = GetNode<Label>("Amount");

      Amount.Visible = false;
   }

   public void Update()
   {
      if(inventorySlot == null || inventorySlot.Item == null)
      {
         return;
      }

      Item.Visible = true;
      Item.Texture = inventorySlot.Item.Texture;      

      if (inventorySlot.Amount > 1)
      {
         Amount.Visible = true;
         Amount.Text = inventorySlot.Amount.ToString();
      }
   }
}
