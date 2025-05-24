
using ARPG.Resources;
using Godot;

namespace ARPG.Scripts.Objects;

public partial class BigSword : Collectable
{
   public AnimationPlayer Animations;

   private Inventory Inventory;

   public override void _Ready()
   {
      Animations = GetNode<AnimationPlayer>("AnimationPlayer");
      Animations.AnimationFinished += AnimationFinished;
   }

   private void AnimationFinished(StringName animName)
   {
      base.Collect(Inventory, true);
   }

   public override void Collect(Inventory inventory, bool addToInv)
   {
      Inventory = inventory;
      Animations.Play("Spin");
   }
}