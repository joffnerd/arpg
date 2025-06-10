using ARPG.Resources;
using Godot;
using ARPG.Scripts.Enemies;

namespace ARPG.Scripts.Player;

public partial class Player : CharacterBody2D
{
   [Export]
   public Inventory Inventory;

   [Signal]
   public delegate void HealthChangedEventHandler(int health);

   [Signal]
   public delegate void ProjectileHitEventHandler(Projectile projectile);

   public AnimationPlayer Animations;
   public AnimationPlayer VisualEffects;
   public AudioStreamPlayer AudioEffects;
   public AudioStream AudioAttack;
   public AudioStream AudioAttackFail;
   public AudioStream AudioHurt;
   public Timer HitTimer;
   public Area2D HurtBox;
   public Weapon Weapon;

   public int maxHealth = 12;
   public int currentHealth;
   public bool haveWeapon = false;

   private int speed = 50;
   private int knockbackAmount = 600;
   private bool isHit = false;
   private bool isAttacking = false;
   private string lastAnimDirection = "Down";
   private bool inputEnabled = true;

   public override void _Ready()
   {
      currentHealth = maxHealth;

      Animations = GetNode<AnimationPlayer>("AnimationPlayer");
      Animations.AnimationFinished += AnimationFinished;

      AudioEffects = GetNode<AudioStreamPlayer>("AudioEffects");
      AudioAttack = ResourceLoader.Load<AudioStream>("res://Audio/Effects/Slash.wav");
      AudioAttackFail = ResourceLoader.Load<AudioStream>("res://Audio/Effects/Sword2.wav");
      AudioHurt = ResourceLoader.Load<AudioStream>("res://Audio/Effects/Impact.wav");

      HitTimer = GetNode<Timer>("HitTimer");
      HitTimer.Timeout += HitTimerTimeout;

      HurtBox = GetNode<Area2D>("HurtBox");

      VisualEffects = GetNode<AnimationPlayer>("VisualEffects");
      VisualEffects.Play("RESET");

      Weapon = GetNode<Weapon>("Weapon");
      Weapon.Disable();
   }

   public override void _PhysicsProcess(double delta)
   {
      if (!inputEnabled)
      {
         return;
      }

      HandleInput();
      MoveAndSlide();
      UpdateAnimation();
   }

   public void HandleInput()
   {
      var currentSpeed = speed;
      if (Input.IsActionPressed("ui_run"))
      {
         currentSpeed = 100;
      }

      var moveDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
      Velocity = moveDirection * currentSpeed;

      if (Input.IsActionJustPressed("ui_attack"))
      {
         Attack();
      }
   }

   public void Attack()
   {
      if (!haveWeapon)
      {
         AudioEffects.Stream = AudioAttackFail;
         AudioEffects.Play();

         return;
      }


      Animations.Play("attack" + lastAnimDirection);
      isAttacking = true;

      AudioEffects.Stream = AudioAttack;
      AudioEffects.Play();

      Weapon.Enable();
   }

   public void UpdateHealth(int amount)
   {
      if (currentHealth < maxHealth)
      {
         currentHealth += amount;

         if (currentHealth > maxHealth)
         {
            currentHealth = maxHealth;
         }

         EmitSignal(SignalName.HealthChanged, currentHealth);
      }
   }

   public void TakeDamage(Node origin)
   {
      //GD.Print("Player Hurt: " + area.GetParent().Name + " -> " + area.Name);

      currentHealth -= 1;
      if (currentHealth < 1)
      {
         GD.Print("GAME OVER");
         QueueFree();
         return;
      }

      if(origin is Enemy)
      {
         var enemy = origin as CharacterBody2D;
         KnockBack(enemy.Velocity);
      }

      EmitSignal(SignalName.HealthChanged, currentHealth);
      isHit = true;

      VisualEffects.Play("HurtBlink");
      HitTimer.Start();

      AudioEffects.Stream = AudioHurt;
      AudioEffects.Play();
   }

   public void KnockBack(Vector2 enemyVelocity)
   {
      var knockbackDir = enemyVelocity.Normalized() * knockbackAmount;
      Velocity = knockbackDir;
      MoveAndSlide();
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

   public void AnimationFinished(StringName animName)
   {
      if (animName.ToString().StartsWith("attack"))
      {
         Weapon.Disable();
         isAttacking = false;
         Animations.Play("walk" + lastAnimDirection);
      }
   }

   public void HitTimerTimeout()
   {
      VisualEffects.Play("RESET");
      isHit = false;
   }

   public void OnHurtBoxAreaEntered(Area2D area)
   {
      var origin = area.GetParent();

      if (isHit)
      {
         return;
      }

      if (origin is Enemy || origin is Projectile)
      {
         TakeDamage(origin);
      }

      if(origin is Projectile)
      {
         EmitSignal(SignalName.ProjectileHit, origin);
      }
   }

   public void ToggleWeapon(bool have)
   {
      haveWeapon = have;
   }

   public void Disable()
   {
      inputEnabled = false;
      VisualEffects.Play("RESET");
   }

   public void Enable()
   {
      inputEnabled = true;
      Visible = true;
   }
}
