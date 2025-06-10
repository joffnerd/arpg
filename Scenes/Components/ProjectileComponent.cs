using Godot;
using GodotUtilities;
using System.Collections.Generic;

namespace ARPG.Components;

public partial class ProjectileComponent : Node2D
{
   public Dictionary<string, float> Stats;

   public Vector2 Direction;

   public override void _Ready()
   {
      Stats.Add("DamagePerProjectile", 1f);
   }

   public partial class ProjectileHitContext : RefCounted
   {
      public ProjectileComponent ProjectileComponent;
      public float TransformedDamage;
   }
}
