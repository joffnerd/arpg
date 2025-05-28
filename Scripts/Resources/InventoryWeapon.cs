using ARPG.Scripts.Autoload;
using Godot;

namespace ARPG.Resources;

[GlobalClass]
public partial class InventoryWeapon : InventoryItem
{
   public InventoryWeapon()
   {
      Success = ResourceLoader.Load<AudioStream>("res://Audio/Effects/LevelUp1.wav");
   }

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