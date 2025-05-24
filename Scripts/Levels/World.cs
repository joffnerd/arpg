using ARPG.Scripts.Enemies;
using Godot;
using Godot.Collections;
using System.Linq;

namespace ARPG.Scripts.Levels;

public partial class World : Node2D
{
   public Player.Player Player;
   public Array<Enemy> Enemies;
   public Node2D Layers;

   public override void _Ready()
   {
      Player = GetTree().Root.GetNode<Player.Player>("Main/Player");
      Layers = GetNode<Node2D>("Layers");

      var enemies = GetNode<Node2D>("Enemies").GetChildren().Cast<Enemy>();
      Enemies = [.. enemies];

      foreach (Enemy enemy in enemies)
      {
         enemy.EnemyDeath += WorldEnemyDeath;
      }
   }

   private void WorldEnemyDeath(Enemy enemy)
   {
      if(Player.currentHealth == Player.maxHealth)
      {
         return;
      }

      var rng = new RandomNumberGenerator();
      float randomValue = rng.Randf();
      GD.Print(randomValue);
      if(randomValue < 0.5)
      {
         return;
      }

      PackedScene scene = ResourceLoader.Load<PackedScene>("res://Scenes/Objects/LifeHeart.tscn");
      var heart = scene.Instantiate() as Area2D;
      heart.Position = enemy.DeathPosition;
      Layers.AddChild(heart);
   }


}
