using ARPG.Resources;
using Godot;

namespace ARPG.Scripts.Objects;

public partial class Collectable : Area2D
{
   [Export]
   public InventoryItem ItemResource;

   public override void _Ready() {
      
   }

   public virtual void Collect(Inventory inventory, bool addToInv = true)
   {
      if (addToInv) {
         inventory.Insert(ItemResource);
      }      
      QueueFree();
   }
}
