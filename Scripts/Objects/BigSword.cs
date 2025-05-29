using ARPG.Resources;
using Godot;

namespace ARPG.Scripts.Objects;

public partial class BigSword : Collectable
{
   public AnimationPlayer Animations;

   public override void _Ready()
   {
      Animations = GetNode<AnimationPlayer>("AnimationPlayer");
      Animations.AnimationFinished += AnimationFinished;

      AudioCollect = ResourceLoader.Load<AudioStream>("res://Audio/Effects/LevelUp1.wav");
   }

   private void AnimationFinished(StringName animName)
   {
      GD.Print("AnimationFinished");

      base.Collect();
   }

   public override void Collect()
   {
      Animations.Play("Spin");
   }
}