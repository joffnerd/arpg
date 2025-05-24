using ARPG.Scripts.Objects;
using ARPG.Resources;
using Godot;

namespace ARPG.Scripts.Player;

public partial class Player : CharacterBody2D
{
   [Export]
   public Inventory Inventory;

   [Signal]
   public delegate void HealthChangedEventHandler(int health);

   public AnimationPlayer Animations;
   public AnimationPlayer Effects;
   public Timer HitTimer;
   public Area2D HurtBox;
   public Weapon Weapon;

   public int maxHealth = 9;
   public int currentHealth;

   private int speed = 50;
   private int knockbackAmount = 600;
   private bool isHit = false;
   private bool isAttacking = false;
   private bool haveWeapon = false;
   private string lastAnimDirection = "Down";

   public override void _Ready()
   {
      currentHealth = maxHealth;

      Animations = GetNode<AnimationPlayer>("AnimationPlayer");
      Animations.AnimationFinished += AnimationFinished;

      HitTimer = GetNode<Timer>("HitTimer");
      HitTimer.Timeout += HitTimerTimeout;

      HurtBox = GetNode<Area2D>("HurtBox");

      Effects = GetNode<AnimationPlayer>("Effects");
      Effects.Play("RESET");

      Weapon = GetNode<Weapon>("Weapon");
      Weapon.Disable();
   }

   public override void _PhysicsProcess(double delta)
   {
      HandleInput();
      MoveAndSlide();
      UpdateAnimation();

      if (!isHit)
      {
         foreach (Area2D area in HurtBox.GetOverlappingAreas())
         {
            if (area.Name == "HitBox")
            {
               GD.Print("Player Hurt: " + area.GetParent().Name + " -> " + area.Name);
               TakeDamage(area);
            }
         }
      }
   }

   private void HitTimerTimeout()
   {
      Effects.Play("RESET");
      isHit = false;
   }

   private void AnimationFinished(StringName animName)
   {
      if (animName.ToString().StartsWith("attack"))
      {
         Weapon.Disable();
         isAttacking = false;
         Animations.Play("walk" + lastAnimDirection);
      }
   }

   public void OnHurtBoxAreaEntered(Area2D area)
   {
      if(!area.HasMeta("isCollectable"))
      {
         return;
      }

      var isCollectable = (bool)area.GetMeta("isCollectable");
      if (isCollectable)
      {
         var item = area as Collectable;

         var isHealth = (bool)area.GetMeta("isHealth");
         if (isHealth)
         {
            UpdateHealth(item);
         }

         var isWeapon = (bool)area.GetMeta("isWeapon");
         if (isWeapon && !haveWeapon)
         {
            UpdateWeapon(item);
         }

         var isInvItem = (bool)area.GetMeta("isInvItem");
         item.Collect(Inventory, isInvItem);
      }
   }

   public void OnHurtBoxAreaExited(Area2D area)
   {
      //
   }

   public void UpdateHealth(Collectable item)
   {
      if (currentHealth < maxHealth)
      {
         var health = (InventoryHealth)item.ItemResource;
         currentHealth += health.HealthAmount;

         if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
         }

         EmitSignal(SignalName.HealthChanged, currentHealth);
      }
   }

   public void UpdateWeapon(Collectable item)
   {
      haveWeapon = true;      
   }

   public void TakeDamage(Area2D area)
   {
      currentHealth -= 1;
      if (currentHealth < 0)
      {
         GD.Print("GAME OVER");
         QueueFree();
      }

      EmitSignal(SignalName.HealthChanged, currentHealth);
      isHit = true;

      var enemy = area.GetParent() as CharacterBody2D;
      KnockBack(enemy.Velocity);
      Effects.Play("HurtBlink");
      HitTimer.Start();
   }

   public void HandleInput()
   {
      var moveDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
      Velocity = moveDirection * speed;

      if (Input.IsActionJustPressed("ui_attack"))
      {
         Attack();
      }
   }

   public void Attack()
   {
      if (!haveWeapon)
      {
         return;
      }

      Animations.Play("attack" + lastAnimDirection);
      isAttacking = true;

      Weapon.Enable();
   }

   public void UpdateAnimation()
   {
      if (isAttacking) return;

      if (Velocity.Length() == 0)
      {
         Animations.Stop();
      }
      else
      {
         var dir = "Down";
         if (Velocity.X < 0)
         {
            dir = "Left";
         }
         else if (Velocity.X > 0)
         {
            dir = "Right";
         }
         else if (Velocity.Y < 0)
         {
            dir = "Up";
         }

         Animations.Play("walk" + dir);
         lastAnimDirection = dir;
      }
   }

   public void KnockBack(Vector2 enemyVelocity)
   {
      var knockbackDir = enemyVelocity.Normalized() * knockbackAmount;
      Velocity = knockbackDir;
      MoveAndSlide();
   }
}
