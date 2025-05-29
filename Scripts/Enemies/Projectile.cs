using Godot;

namespace ARPG.Scripts.Enemies;

public partial class Projectile : CharacterBody2D
{
   [Export]
   public int Speed = 50;

   public float Direction;
   public Vector2 SpawnPos;
   public float SpawnRot;

   public override void _Ready()
   {
      GlobalPosition = SpawnPos;
      GlobalRotation = SpawnRot;
   }

   public override void _PhysicsProcess(double delta)
   {
      Velocity = new Vector2(0,-Speed).Rotated(Direction);
      MoveAndSlide();
   }

   public void OnProjectileBodyEntered(Node2D body)
   {
      if(body is Player.Player)
      {
         GD.Print("Projectile Collision " + body.Name);
         QueueFree();
      }      
   }
}
