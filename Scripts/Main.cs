using ARPG.Scripts.Gui;
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
      SceneManager.Instance.Player = Player;

      GUI = GetNode<CanvasLayer>("GUI");
      HeartsContainer = GetNode<HeartsContainer>("GUI/HeartsContainer");

      HeartsContainer.SetMaxHearts(Player.maxHealth);
      HeartsContainer.UpdateHearts(Player.currentHealth);      
      Player.HealthChanged += HeartsContainer.UpdateHearts;      

      Init();
   }

   public override void _Input(InputEvent @event)
   {
      base._Input(@event);

      if (Input.IsActionJustPressed("ui_rclick"))
      {
         var temp = CurrentMap.GetGlobalMousePosition();

         GD.Print("X: " + temp.X);
         GD.Print("Y: " + temp.Y);
      }
   }

   public void Init()
   {
      SceneManager.Instance.ChangeScene(null, "Level1");      
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
