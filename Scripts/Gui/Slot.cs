using Godot;

namespace ARPG.Scripts.Gui;

public partial class Slot : Button
{
   public Sprite2D Background;
   public CenterContainer CenterContainer;
   public Label Label;

   public Resources.Inventory PlayerInventory;
   public ItemStack ItemStack;

   public int Index { get; set; }

   public override void _Ready()
   {
      Background = GetNode<Sprite2D>("Background");
      CenterContainer = GetNode<CenterContainer>("CenterContainer");
      Label = GetNode<Label>("Label");

      PlayerInventory = GD.Load<Resources.Inventory>("res://Scenes/Inventory/PlayerInventory.tres");
   }

   public void Insert(ItemStack itemStack)
   {
      ItemStack = itemStack;
      Background.Frame = 1;
      CenterContainer.AddChild(ItemStack);      

      if(ItemStack.inventorySlot == null || PlayerInventory.Slots[Index] == ItemStack.inventorySlot)
      {
         return;
      }

      PlayerInventory.InsertSlot(Index, ItemStack.inventorySlot);
   }

   public ItemStack TakeItem()
   {
      var item = ItemStack;

      PlayerInventory.RemoveSlot(ItemStack.inventorySlot);
      CenterContainer.RemoveChild(ItemStack);
      ItemStack = null;
      Background.Frame = 0;

      return item;
   }

   public bool IsEmpty()
   {
      return ItemStack == null;
   }
}
