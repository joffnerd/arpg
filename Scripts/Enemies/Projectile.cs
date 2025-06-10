using ARPG.Scripts.Autoload;
using ARPG.Scripts.Gui;
using Godot;

namespace ARPG.Scripts.Enemies;

public partial class Projectile : CharacterBody2D
{
   [Export]
   public int Speed = 75;

   public Sprite2D Sprite2D;

   public float RotationSpeed = 3;
   public Vector2 SpawnPosition;
   

   public override void _Ready()
   {
      Sprite2D = GetNode<Sprite2D>("Sprite2D");

      GlobalPosition = SpawnPosition;

      var distanceVector = SpawnPosition - SceneManager.Instance.Player.GlobalPosition;
      Velocity = distanceVector.Normalized() * -Speed;

      ZIndex = 5;

      SceneManager.Instance.Player.ProjectileHit += ProjectileHit;
   }

   public override void _PhysicsProcess(double delta)
   {
      Sprite2D.RotationDegrees += RotationSpeed;
      MoveAndSlide();
   }

   public void ProjectileHit(Projectile projectile)
   {
      GD.Print("ProjectileHit");
      projectile.QueueFree();
   }
}
