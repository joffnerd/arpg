using ARPG.Scripts.Enemies;
using Godot;
using Godot.Collections;
using System.Linq;

namespace ARPG.Scripts.Levels;

public partial class BaseLevel : Node2D
{
   public Node2D Enemies;
   public Array<Enemy> EnemyList;

   public Node2D Layers;
   public Node2D EntranceMarkers;

   public override void _Ready()
   {
      Enemies = GetNodeOrNull<Node2D>("Enemies");

      Layers = GetNode<Node2D>("Layers");
      EntranceMarkers = GetNode<Node2D>("EntranceMarkers");

      PositionPlayer();

      SetEnemies();
   }

   private void PositionPlayer()
   {
      var lastScene = SceneManager.Instance.LastScene;
      if (string.IsNullOrEmpty(lastScene))
      {
         lastScene = "Start";
      }

      var markers = EntranceMarkers.GetChildren();

      var position = EntranceMarkers.GetChildren().Where(x => x.Name == lastScene).FirstOrDefault() as Marker2D;
      if (position == null)
      {
         position = EntranceMarkers.GetChildren().Where(x => x.Name == "Start").FirstOrDefault() as Marker2D;
      }
      SceneManager.Instance.Player.GlobalPosition = position.GlobalPosition;
   }

   private void SetEnemies()
   {
      if (Enemies == null)
      {
         return;
      }

      var enemies = Enemies.GetChildren().Cast<Enemy>();
      EnemyList = [.. enemies];
      foreach (Enemy enemy in enemies)
      {
         enemy.EnemyDeath += EnemyDeath;
      }
   }

   private void EnemyDeath(Enemy enemy)
   {
      if (SceneManager.Instance.Player.currentHealth == SceneManager.Instance.Player.maxHealth)
      {
         return;
      }

      var rng = new RandomNumberGenerator();
      float randomValue = rng.Randf();
      GD.Print(randomValue);
      if (randomValue < 0.5)
      {
         return;
      }

      PackedScene scene = ResourceLoader.Load<PackedScene>("res://Scenes/Objects/LifeHeart.tscn");
      var heart = scene.Instantiate() as Area2D;
      heart.Position = enemy.DeathPosition;
      Layers.AddChild(heart);
   }
}
