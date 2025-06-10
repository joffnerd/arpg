using Godot;

namespace ARPG.Scripts.Enemies;

public partial class Slime : Enemy
{
   public override void _Ready()
   {
      speed = 25;
      canFollow = true;

      base._Ready();      
   }

   public override void AnimationFinished(StringName animName)
   {
      base.AnimationFinished(animName);
   }
}
