using ARPG.Scripts.Autoload;
using Godot;

namespace ARPG.Scripts.Enemies;

public partial class Snake : Enemy
{
   public PackedScene Projectile;
   private double timer;

   public override void _Ready()
   {
      speed = 35;
      currentHealth = 4;
      canFollow = true;

      base._Ready();

      Projectile = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/Projectile.tscn");
   }

   public override void _PhysicsProcess(double delta)
   {
      base._PhysicsProcess(delta);

      CheckTimer(delta);
   }

   public void CheckTimer(double delta)
   {
      if (isTakingDamage || !perimeterBreached)
      {
         return;
      }

      timer += delta;
      if (timer > 1)
      {
         timer = 0;
         ShootProjectile();
      }
   }

   public void ShootProjectile()
   {
      var instance = Projectile.Instantiate() as Projectile;
      instance.SpawnPosition = GlobalPosition;
      instance.Speed = 90;

      SceneManager.Instance.Main.CallDeferred("add_child", instance);
   }
}
