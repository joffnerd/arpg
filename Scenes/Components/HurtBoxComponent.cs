using ARPG.Scripts.Enemies;
using Godot;
using GodotUtilities;
using System.Collections.Generic;
using static ARPG.Components.ProjectileComponent;

namespace ARPG.Components;

[Scene]
public partial class HurtBoxComponent : Area2D
{
   public const string GROUP_ENEMY_HURTBOX = "enemy_hitbox";

   [Signal]
   public delegate void HitByProjectileEventHandler();

   [Signal]
   public delegate void HitByHitBoxEventHandler(HitBoxComponent hitBoxComponent);

   [Export]
   private HealthComponent healthComponent;

   [Export]
   private StatusReceiverComponent statusReceiverComponent;

   [Export]
   private PackedScene projectileImpactScene;

   [Export]
   private bool detectOnly;

   public override void _Notification(int what)
   {
      if (what == NotificationSceneInstantiated)
      {
         WireNodes();
      }
   }

   public override string[] _GetConfigurationWarnings()
   {
      if (Owner is CharacterBody2D body && (body.CollisionLayer & 1) == 1)
      {
         return ["Owner CharacterBody2D has terrain layer bit set."];
      }
      /*
      if (statusReceiverComponent == null)
      {
         return [$"{nameof(StatusReceiverComponent)} is not set"];
      }
      */
      return [string.Empty];
   }

   public override void _Ready()
   {
      if(CollisionLayer== (1 << 3))
      {
         AddToGroup(GROUP_ENEMY_HURTBOX);
      }
      Connect("area_entered", new Callable(this, nameof(OnAreaEntered)));
   }

   public bool CanAcceptProjectileCollision()
   {
      return healthComponent?.HasHealthRemaining ?? true;
   }

   public void HandleProjectileCollision(ProjectileComponent projectile)
   {
      GameEvents.EmitEntityCollision(new EntityCollisionEvent
      {
         EntityHandle = Owner as Node2D,
         ProjectileStats = projectile.Stats, //.Duplicate(),
         ProjectileComponent = projectile,
         Tree = GetTree()
      });

      var damage = 0f;
      if (detectOnly)
      {
         //damage = projectile.Stats.DamagePerProjectile;
         projectile.Stats.TryGetValue("DamagePerProjectile", out damage);
         damage = DealDamageWithTransforms(damage);
      }

      var impact = projectileImpactScene?.InstanceOrFree<Node2D>();
      if (impact != null)
      {
         //GetMain().Projectiles.AddChild(impact);
         GetTree().Root.GetNode("Main").GetNode("Projectiles").AddChild(impact);
         impact.GlobalPosition = projectile.GlobalPosition;
         impact.Rotation = (-projectile.Direction).Angle();
      }

      // unknown ProjectileHitContext
      EmitSignal(SignalName.HitByProjectile, new ProjectileHitContext
      {
         ProjectileComponent = projectile,
         TransformedDamage = damage
      });
   }

   private float DealDamageWithTransforms(float damage)
   {
      var finalDamage = statusReceiverComponent?.ApplyDamageTransformation(damage) ?? damage;
      healthComponent?.Damage(finalDamage);
      return finalDamage;
   }

   private void OnAreaEntered(Area2D otherArea)
   {
      if(otherArea is HitBoxComponent hitBoxComponent)
      {
         if (!detectOnly)
         {
            DealDamageWithTransforms(hitBoxComponent.Damage);
         }
         EmitSignal(SignalName.HitByProjectile, hitBoxComponent);
      }
   }
}

public partial class GameEvents : Node
{
   public static void EmitEntityCollision(EntityCollisionEvent entityCollisionEvent)
   {

   }
}

public partial class EntityCollisionEvent : RefCounted
{
   public Node2D EntityHandle;
   public Dictionary<string, float> ProjectileStats;
   public ProjectileComponent ProjectileComponent;
   public SceneTree Tree;
}
