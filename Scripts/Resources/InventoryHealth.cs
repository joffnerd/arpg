using ARPG.Scripts.Autoload;
using Godot;

namespace ARPG.Resources;

[GlobalClass]
public partial class InventoryHealth : InventoryItem
{
   [Export]
   public int HealthAmount = 1;

   public override void UseItem()
   {
      base.UseItem();

      if (CanUseItem())
      {
         SceneManager.Instance.Player.UpdateHealth(HealthAmount);
         SceneManager.Instance.Player.Inventory.RemoveLastUsedItem();
      }
   }

   public override bool CanUseItem()
   {
      var canUse = SceneManager.Instance.Player.currentHealth < SceneManager.Instance.Player.maxHealth;
      return canUse;
   }
}