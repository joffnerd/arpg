using Godot;

namespace ARPG.Scripts.Enemies;

public partial class Snake : Enemy
{
   public override void _Ready()
   {
      speed = 35;
      currentHealth = 4;

      base._Ready();
   }

   public override void AnimationFinished(StringName animName)
   {
      base.AnimationFinished(animName);
   }
}
