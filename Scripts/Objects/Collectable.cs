using ARPG.Resources;
using ARPG.Scripts.Autoload;
using Godot;
using Godot.Collections;

namespace ARPG.Scripts.Objects;

public partial class Collectable : Area2D
{
   [Export]
   public InventoryItem ItemResource;

   public Dictionary<string, bool> MetaData = [];

   public override void _Ready()
   {

   }

   public virtual void Collect(Inventory inventory)
   {
      MetaData.TryGetValue("isWeapon", out bool isWeapon);
      MetaData.TryGetValue("isHealth", out bool isHealth);
      MetaData.TryGetValue("isInvItem", out bool isInvItem);
      MetaData.TryGetValue("isConsumable", out bool isConsumable);

      if (isInvItem)
      {
         inventory.Insert(ItemResource);
      }

      if (!isInvItem && isHealth && isConsumable)
      {
         var amount = ((InventoryHealth)ItemResource).HealthAmount;
         SceneManager.Instance.Player.UpdateHealth(amount);
      }

      QueueFree();
   }

   public void BuildMeta(Area2D area)
   {
      var meta = area.GetMetaList();
      foreach (var item in meta)
      {
         var val = (bool)area.GetMeta(item.ToString());
         MetaData.Add(item, val);
      }
   }
}
