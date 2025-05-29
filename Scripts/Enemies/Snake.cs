using ARPG.Scripts.Autoload;
using Godot;

namespace ARPG.Scripts.Enemies;

public partial class Snake : Enemy
{
   public Timer CoolDown;
   public PackedScene Projectile;

   public override void _Ready()
   {
      speed = 35;
      currentHealth = 4;

      base._Ready();

      Projectile = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/Projectile.tscn");

      CoolDown = new Timer()
      {
         WaitTime = 1,
         Autostart = true
      };
      CoolDown.Timeout += CoolDown_Timeout;

      GetTree().Root.AddChild(CoolDown);

      CoolDown.Start();

      isFollowing = true;
   }

   private void CoolDown_Timeout()
   {
      //Shoot();
   }

   public override void AnimationFinished(StringName animName)
   {
      base.AnimationFinished(animName);
   }

   public void Shoot()
   {
      if (isTakingDamage)
      {
         return;
      }

      if (isFollowing)
      {
         //GD.Print("Fire");
         var instance = Projectile.Instantiate() as Projectile;
         instance.Direction = Rotation;
         instance.SpawnPos = GlobalPosition;
         instance.SpawnRot = Rotation;

         SceneManager.Instance.Main.CallDeferred("add_child", instance);
      }
   }
}
