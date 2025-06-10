using ARPG.Scripts.Autoload;
using ARPG.Scripts.Enemies;
using ARPG.Scripts.Objects;
using Godot;
using Godot.Collections;
using System;
using System.Linq;

namespace ARPG.Scripts.Levels;

public partial class BaseLevel : Node2D
{
   public Node2D Layers;
   public TileMapLayer Items;

   public Node2D Enemies;
   public Array<Enemy> EnemyList;

   public Node2D SceneTriggers;
   public Array<Area2D> SceneTriggerList;

   public Node2D EntranceMarkers;
   public Array<Marker2D> EntranceMarkerList;

   public PackedScene LifeHeart;
   public PackedScene Gem;
   public PackedScene Coin;

   public LevelDataHandoff Data;

   public RandomNumberGenerator Rng;
   public Random Rnd;

   public override void _Ready()
   {
      Layers = GetNode<Node2D>("Layers");
      Items = Layers.GetNode<TileMapLayer>("Items");
      Enemies = GetNodeOrNull<Node2D>("Enemies");
      SceneTriggers = GetNode<Node2D>("SceneTriggers");
      EntranceMarkers = GetNode<Node2D>("EntranceMarkers");

      var triggers = SceneTriggers.GetChildren().Cast<SceneTrigger>();
      SceneTriggerList = [.. triggers];

      var markers = EntranceMarkers.GetChildren().Cast<Marker2D>();
      EntranceMarkerList = [.. markers];

      LifeHeart = ResourceLoader.Load<PackedScene>("res://Scenes/Collectables/LifeHeart.tscn");
      Gem = ResourceLoader.Load<PackedScene>("res://Scenes/Collectables/Gem.tscn");
      Coin = ResourceLoader.Load<PackedScene>("res://Scenes/Collectables/Coin.tscn");

      Rng = new RandomNumberGenerator();
      Rng.Randomize();

      Rnd = new Random();
   }

   public void InitScene()
   {
      //GD.Print("BL - InitScene");

      InitPlayerLocation();

      SetEnemies();
   }

   public void StartScene()
   {
      //GD.Print("BL - StartScene");

      SceneManager.Instance.Player.Enable();

      ConnectToTriggers();
   }

   public void InitPlayerLocation()
   {
      //GD.Print("BL - InitPlayerLocation");

      SceneManager.Instance.Player.Visible = true;

      if (Data != null)
      {
         //GD.Print("BL - EntryDoorName " + Data.EntryDoorName);

         var marker = EntranceMarkerList.Where(t => t.Name == Data.EntryDoorName).FirstOrDefault();
         if (marker != null)
         {
            SceneManager.Instance.Player.GlobalPosition = marker.GlobalPosition;
         }
      }
      else
      {
         var position = EntranceMarkers.GetChildren().Where(x => x.Name == "Start").FirstOrDefault() as Marker2D;
         SceneManager.Instance.Player.GlobalPosition = position.GlobalPosition;
      }

      //SceneManager.Instance.Main.FollowCam.SetCameraLimits();
   }

   private void OnPlayerEnterTrigger(SceneTrigger trigger)
   {
      DisconnectFromTriggers();

      Data = new LevelDataHandoff
      {
         EntryDoorName = trigger.EntryDoorName,
         MoveDir = trigger.GetMoveDir()
      };

      SetProcess(false);
   }

   private void ConnectToTriggers()
   {
      foreach (SceneTrigger trigger in SceneTriggerList)
      {
         if (!trigger.IsConnected(SceneTrigger.SignalName.PlayerEnteredDoor, Callable.From((SceneTrigger t) => OnPlayerEnterTrigger(t))))
         {
            trigger.Connect(SceneTrigger.SignalName.PlayerEnteredDoor, Callable.From((SceneTrigger t) => OnPlayerEnterTrigger(t)));
         }
      }
   }

   private void DisconnectFromTriggers()
   {
      foreach (SceneTrigger trigger in SceneTriggerList)
      {
         if (trigger.IsConnected(SceneTrigger.SignalName.PlayerEnteredDoor, Callable.From((SceneTrigger t) => OnPlayerEnterTrigger(t))))
         {
            trigger.Disconnect(SceneTrigger.SignalName.PlayerEnteredDoor, Callable.From((SceneTrigger t) => OnPlayerEnterTrigger(t)));
         }
      }
   }

   private void SetEnemies()
   {
      //GD.Print("BL - SetEnemies");

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
      if (SceneManager.Instance.Player.currentHealth < SceneManager.Instance.Player.maxHealth)
      {
         SpawnHealth(enemy.DeathPosition);
      }
      SpawnGem(enemy.DeathPosition);

      SpawnCoin(enemy.DeathPosition);
   }

   public void SpawnHealth(Vector2 position)
   {
      float randomValue = Rng.RandfRange(0,1);
      if (randomValue < 0.5)
      {
         return;
      }

      var heart = LifeHeart.Instantiate() as Area2D;
      heart.GlobalPosition = position + RandomDrop();
      Items.AddChild(heart);

      GD.Print("Heart drop " + heart.GlobalPosition);
   }

   public void SpawnGem(Vector2 position)
   {
      var gem = Gem.Instantiate() as Gem;
      gem.spawnRandom = true;
      gem.GlobalPosition = position + RandomDrop();
      Items.AddChild(gem);

      GD.Print("Gem drop " + gem.GlobalPosition);
   }

   public void SpawnCoin(Vector2 position)
   {
      var coin = Coin.Instantiate() as Collectable;
      coin.GlobalPosition = position + RandomDrop();
      Items.AddChild(coin);

      GD.Print("Coin drop " + coin.GlobalPosition);
   }

   public Vector2 RandomDrop()
   {
      var radius = Rnd.NextDouble() * (10 - 5) + 5;
      var angle = Rnd.NextDouble() * 360;

      GD.Print("radius " + radius);
      GD.Print("angle " + angle);

      var angleRadians = (Math.PI * angle) / 180.0;

      Vector2 cartesianPoint = Vector2.FromAngle((float)angleRadians) * (float)radius;      

      return cartesianPoint;
   }

   public LevelDataHandoff GetData()
   {
      //GD.Print("BL - GetData");

      return Data;
   }

   public void ReceiveData(Node data)
   {
      if (data == null)
      {
         return;
      }

      //GD.Print("BL - ReceiveData " + data);

      if (data is LevelDataHandoff)
      {
         Data = (LevelDataHandoff)data;
      }
      else
      {
         GD.PushWarning("Level is is receiving data it cannot process " + Name);
      }
   }
}
