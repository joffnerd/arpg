using ARPG.Scripts.Player;
using Godot;

namespace ARPG.Scripts.Enemies;

public partial class Enemy : CharacterBody2D
{
   [Signal]
   public delegate void EnemyDeathEventHandler(Enemy self);

   [Export]
   public Marker2D EndPoint;

   public Player.Player Target;
   public AnimationPlayer Animations;
   public Area2D HitBox;
   public Timer KnockbackTimer;
   public AudioStreamPlayer AudioEffects;
   public Timer DamageTimer;

   public Vector2 StartPosition;
   public Vector2 EndPosition;
   public Vector2 DeathPosition;

   public int currentHealth = 2;
   public int speed = 10;
   public float moveLimit = 0.5f;
   public bool isDead = false;
   public bool isTakingDamage = false;
   public bool isFollowing = false;

   public override void _Ready()
   {
      Animations = GetNode<AnimationPlayer>("AnimationPlayer");
      Animations.AnimationFinished += AnimationFinished;

      HitBox = GetNode<Area2D>("HitBox");

      AudioEffects = GetNode<AudioStreamPlayer>("AudioEffects");

      DamageTimer = new Timer()
      {
         WaitTime = 0.5
      };
      AddChild(DamageTimer);
      DamageTimer.Timeout += DamageTimer_Timeout; ;

      DeathPosition = GlobalPosition;
      StartPosition = GlobalPosition;
      EndPosition = EndPoint.GlobalPosition;
   }

   private void DamageTimer_Timeout()
   {
      isTakingDamage = false;
   }

   public override void _PhysicsProcess(double delta)
   {
      if (isDead) return;

      if (isTakingDamage) return;

      UpdateVelocity();
      UpdateAnimation();
      MoveAndSlide();
   }

   public virtual void AnimationFinished(StringName animName)
   {
      GD.Print(animName);

      if(animName == "Death")
      {
         CharacterBody2D enemy = this;
         EmitSignal(SignalName.EnemyDeath, enemy);

         QueueFree();
      }
   }

   public void OnHurtBoxAreaEntered(Area2D area)
   {
      if (area.GetParent() is Weapon)
      {
         TakeDamage(area);         
      }
   }

   public void OnFollowTriggerBodyEntered(Node2D body)
   {
      if (body is Player.Player)
      {
         isFollowing = true;
         Target = body as Player.Player;
      }
   }

   public void OnFollowTriggerBodyExited(Node2D body)
   {
      if (body == Target)
      {
         isFollowing = false;
         Target = null;
      }
   }

   public void TakeDamage(Area2D area)
   {
      DamageTimer.Start();
      isTakingDamage = true;

      AudioEffects.Play();

      currentHealth -= 1;

      GD.Print(string.Format("{0} Hurt: {1} -> {2}", Name, area.Name, currentHealth));

      if (currentHealth < 1)
      {
         DeathPosition = GlobalPosition;
         isDead = true;
         HitBox.SetDeferred("Monitorable", false);
         Animations.Play("Death");
      }
   }

   public void UpdateVelocity()
   {
      if (Target != null)
      {
         var d = Target.GlobalPosition - GlobalPosition;
         var v = d.Normalized() * speed;
         Velocity = v;
         return;
      }

      var moveDirection = EndPosition - GlobalPosition;
      if (moveDirection.Length() < moveLimit)
      {
         ChangeDirection();
      }
      Velocity = moveDirection.Normalized() * speed;
   }

   public void UpdateAnimation()
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
   }

   public void ChangeDirection()
   {
      Vector2 tempEnd = EndPosition;
      EndPosition = StartPosition;
      StartPosition = tempEnd;
   }
}
