using ARPG.Scripts;
using Godot;

namespace ARPG.Resources;

[GlobalClass]
public partial class InventoryWeapon : InventoryItem
{
   public override void UseItem()
   {
      base.UseItem();

      if (CanUseItem())
      {
         SceneManager.Instance.Player.ToggleWeapon();
      }
   }

   public override bool CanUseItem()
   {
      return !SceneManager.Instance.Player.haveWeapon;
   }
}