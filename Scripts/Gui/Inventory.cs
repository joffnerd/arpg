
using Godot;
using Godot.Collections;
using System.Linq;

namespace ARPG.Scripts.Gui;

public partial class Inventory : Control
{
   [Signal]
   public delegate void OpenedEventHandler();

   [Signal]
   public delegate void ClosedEventHandler();

   public bool isOpen = false;

   public Resources.Inventory PlayerInventory;
   public PackedScene ItemStack;
   public Array<Slot> Slots;

   private ItemStack itemInHand;
   private int oldIndex = -1;
   private bool inventoryLocked = false;

   public override void _Ready()
   {
      PlayerInventory = GD.Load<Resources.Inventory>("res://Scenes/Inventory/PlayerInventory.tres");
      ItemStack = GD.Load<PackedScene>("res://Scenes/Gui/Inventory/ItemStack.tscn");

      var hotBarSlots = GetNode<BoxContainer>("Background/HotBarSlots").GetChildren().Cast<Slot>();
      var slots = GetNode<GridContainer>("Background/NormalSlots").GetChildren().Cast<Slot>();

      Slots = [];
      Slots.AddRange([.. hotBarSlots]);
      Slots.AddRange([.. slots]);

      PlayerInventory.InventoryUpdated += UpdateInventory;

      ConnectSlots();
      UpdateInventory();
   }

   public override void _Input(InputEvent @event)   {
      base._Input(@event);

      if (itemInHand != null && !inventoryLocked && Input.IsActionJustPressed("ui_rclick")) {
         ReturnItems();
      }

      UpdateItemInHand();
   }

   public void ConnectSlots()
   {
      for (int i = 0; i < Slots.Count; i++)
      {
         var slot = Slots[i];
         slot.Index = i;
         slot.Label.Text = i.ToString();

         var button = (Button)slot;
         button.Pressed += () => SlotPressed(slot);
      }
   }

   public void SlotPressed(Slot slot)
   {
      if (inventoryLocked) return;

      if (slot.IsEmpty())
      {
         if (itemInHand == null)
         {
            return;
         }

         InsertItemInSlot(slot);
         return;
      }

      if (itemInHand == null)
      {
         TakeItemFromSlot(slot);
         return;
      }

      if(slot.ItemStack.inventorySlot.Item.Name == itemInHand.inventorySlot.Item.Name)
      {
         StackItems(slot);
         return;
      }

      SwapItems(slot);
   }

   public void TakeItemFromSlot(Slot slot)
   {
      itemInHand = slot.TakeItem();
      AddChild(itemInHand);
      UpdateItemInHand();

      oldIndex = slot.Index;
   }

   public void InsertItemInSlot(Slot slot)
   {
      var item = itemInHand;
      RemoveChild(itemInHand);
      itemInHand = null;
      slot.Insert(item);

      oldIndex = -1;
   }

   public void SwapItems(Slot slot)
   {
      var tempItem = slot.TakeItem();
      InsertItemInSlot(slot);
      itemInHand = tempItem;
      AddChild(itemInHand);
      UpdateItemInHand();
   }

   public void StackItems(Slot slot)
   {
      var slotItem = slot.ItemStack;
      var maxAmount = slotItem.inventorySlot.Item.MaxStackAmount;
      var totalAmount = slotItem.inventorySlot.Amount + itemInHand.inventorySlot.Amount;

      if (slotItem.inventorySlot.Amount == maxAmount)
      {
         SwapItems(slot);
         return;
      }

      if (totalAmount <= maxAmount)
      {
         slotItem.inventorySlot.Amount = totalAmount;
         RemoveChild(itemInHand);
         itemInHand = null;
         oldIndex = -1;
      }
      else
      {
         slotItem.inventorySlot.Amount = maxAmount;
         itemInHand.inventorySlot.Amount = totalAmount - maxAmount;
      }

      slotItem.Update();
      itemInHand?.Update();
   }

   public void ReturnItems()
   {
      inventoryLocked = true;
      if (oldIndex < 0)
      {
         var emptyslot = Slots.Where(x => x.IsEmpty()).FirstOrDefault();
         if (emptyslot != null)
         {
            oldIndex = emptyslot.Index;
         }
      }

      var targetSlot = Slots[oldIndex];

      var tween = CreateTween();
      var targetPosition = targetSlot.GlobalPosition + (targetSlot.Size / 2);
      tween.TweenProperty(itemInHand, "global_position", targetPosition, 0.2);
      tween.Finished += () => ReturnItemsTweenFinished(targetSlot);      
   }

   private void ReturnItemsTweenFinished(Slot slot)
   {
      InsertItemInSlot(slot);
      inventoryLocked = false;
   }

   public void UpdateItemInHand()
   {
      if (itemInHand == null)
      {
         return;
      }

      itemInHand.GlobalPosition = GetGlobalMousePosition() - (itemInHand.Size / 2);
   }

   public void UpdateInventory()
   {
      var min = Mathf.Min(PlayerInventory.Slots.Count, Slots.Count);

      for (int i = 0; i < min; i++)
      {
         var inventorySlot = PlayerInventory.Slots[i];

         if (inventorySlot.Item == null)
         {
            Slots[i].Clear();
            continue;
         }

         var itemStack = Slots[i].ItemStack;
         if (itemStack == null)
         {
            itemStack = ItemStack.Instantiate() as ItemStack;
            Slots[i].Insert(itemStack);
         }

         itemStack.inventorySlot = inventorySlot;
         itemStack.Update();
      }
   }

   public void Open()
   {
      isOpen = true;
      Visible = isOpen;
      EmitSignal(SignalName.Opened);
   }

   public void Close()
   {
      isOpen = false;
      Visible = isOpen;
      EmitSignal(SignalName.Closed);
   }
}
