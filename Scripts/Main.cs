using ARPG.Scripts.Gui;
using ARPG.Scripts.Levels;
using ARPG.Scripts.Player;
using Godot;

namespace ARPG.Scripts;

public partial class Main : Node
{
   public Node2D CurrentMap;
   public Player.Player Player;
   public CanvasLayer GUI;
   public HeartsContainer HeartsContainer;

   public override void _Ready()
   {
      CurrentMap = GetNode<Node2D>("CurrentMap");
      Player = GetNode<Player.Player>("Player");

      GUI = GetNode<CanvasLayer>("GUI");
      HeartsContainer = GetNode<HeartsContainer>("GUI/HeartsContainer");

      HeartsContainer.SetMaxHearts(Player.maxHealth);
      HeartsContainer.UpdateHearts(Player.currentHealth);      
      Player.HealthChanged += HeartsContainer.UpdateHearts;

      LoadMap();
   }

   public void LoadMap()
   {
      var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Levels/World.tscn");
      var currentMap = scene.Instantiate() as World;
      currentMap.GlobalPosition = new Vector2(0,0);
      CurrentMap.AddChild(currentMap);

      var camera = Player.GetNode("FollowCam") as FollowCam;
      camera.SetLimits();
      //MoveChild(currentMap, 0);
   }

   public void OnInventoryClosed()
   {
      GetTree().Paused = false;
   }

   public void OnInventoryOpened()
   {
      GetTree().Paused = true;
   }
}
