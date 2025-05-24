using Godot;

namespace ARPG.Scripts.Player;

public partial class Weapon : Node2D
{
   public Area2D CurrentWeapon = null;
   public CollisionShape2D CollisionShape;

   public override void _Ready()
   {
      if (GetChildCount() == 0) return;

      CurrentWeapon = GetChildOrNull<Area2D>(0);
      CollisionShape = CurrentWeapon.GetNode<CollisionShape2D>("CollisionShape2D");
   }

   public void Enable()
   {
      if (CurrentWeapon != null)
      {
         CurrentWeapon.Visible = true;
         CollisionShape.Disabled = false;
      }
   }

   public void Disable()
   {
      if (CurrentWeapon != null)
      {
         CurrentWeapon.Visible = false;
         CollisionShape.Disabled = true;
      }
   }
}
