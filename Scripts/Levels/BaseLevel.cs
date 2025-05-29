using ARPG.Scripts.Autoload;
using ARPG.Scripts.Enemies;
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

    public LevelDataHandoff Data;

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

        LifeHeart = ResourceLoader.Load<PackedScene>("res://Scenes/Objects/LifeHeart.tscn");
        Gem = ResourceLoader.Load<PackedScene>("res://Scenes/Objects/Gem.tscn");
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
        GD.Print(enemy.Name);

        if (SceneManager.Instance.Player.currentHealth < SceneManager.Instance.Player.maxHealth)
        {
            SpawnHealth(enemy.DeathPosition);
        }

        SpawnGems(enemy.DeathPosition);
    }

    public void SpawnHealth(Vector2 position)
    {
        var rng = new RandomNumberGenerator();
        float randomValue = rng.Randf();
        GD.Print(randomValue);
        if (randomValue < 0.5)
        {
            return;
        }

        var heart = LifeHeart.Instantiate() as Area2D;
        heart.Position = position;
        Items.AddChild(heart);
    }

    public void SpawnGems(Vector2 position)
    {
        var gem = Gem.Instantiate() as Area2D;
        gem.Position = position;
        Items.AddChild(gem);
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
