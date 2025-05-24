using Godot;

namespace ARPG.Scripts.Enemies;

public partial class Enemy : CharacterBody2D
{
   [Signal]
   public delegate void EnemyDeathEventHandler(Enemy self);

   [Export]
   public Marker2D EndPoint;

   public AnimationPlayer Animations;
   public Area2D HitBox;
   public Timer KnockbackTimer;

   public Vector2 StartPosition;
   public Vector2 EndPosition;
   public Vector2 DeathPosition;

   public int speed = 10;
   public float moveLimit = 0.5f;
   public bool isDead = false;

   public override void _Ready()
   {
      Animations = GetNode<AnimationPlayer>("AnimationPlayer");
      Animations.AnimationFinished += AnimationFinished;

      HitBox = GetNode<Area2D>("HitBox");

      DeathPosition = Position;
      StartPosition = Position;
      EndPosition = EndPoint.GlobalPosition;
   }

   public override void _PhysicsProcess(double delta)
   {
      if (isDead) return;

      UpdateVelocity();
      UpdateAnimation();
      MoveAndSlide();
   }

   public virtual void AnimationFinished(StringName animName)
   {
      QueueFree();

      CharacterBody2D enemy = this;
      EmitSignal(SignalName.EnemyDeath, enemy);
   }

   public void OnHurtBoxAreaEntered(Area2D area)
   {
      GD.Print(string.Format("Slime Hurt: {0} -> {1}", area.GetParent().Name, area.Name));

      if (area.GetParent().Name == "Weapon")
      {
         DeathPosition = Position;
         isDead = true;
         HitBox.SetDeferred("Monitorable", false);
         Animations.Play("Death");
      }
   }

   public void UpdateVelocity()
   {
      var moveDirection = EndPosition - Position;
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
