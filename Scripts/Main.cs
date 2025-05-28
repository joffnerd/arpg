using ARPG.Scripts.Autoload;
using ARPG.Scripts.Gui;
using ARPG.Scripts.Levels;
using ARPG.Scripts.Menus;
using ARPG.Scripts.Player;
using Godot;

namespace ARPG.Scripts;

[Tool]
public partial class Main : Node2D
{
   [Signal]
   public delegate void PlayerReadyEventHandler();

   public Node2D CurrentMap;
   public Player.Player Player;
   public CanvasLayer GUI;   
   public HeartsContainer HeartsContainer;
   public FollowCam FollowCam;

   public BaseLevel currentLevel;

   public override void _Ready()
   {
      //GD.Print("M  - Ready");

      SceneManager.Instance.LoadStart += LoadStart;
      SceneManager.Instance.LoadComplete += LevelLoaded;
      SceneManager.Instance.SceneAdded += LevelAdded;

      Player = GetNode("Player") as Player.Player;
      SceneManager.Instance.Player = Player;
      EmitSignal(SignalName.PlayerReady);
      //GD.Print("M  - PlayerReady " + player);

      GUI = GetNode<CanvasLayer>("GUI");
      HeartsContainer = GetNode<HeartsContainer>("GUI/HeartsContainer");
      HeartsContainer.SetMaxHearts(SceneManager.Instance.Player.maxHealth);
      HeartsContainer.UpdateHearts(SceneManager.Instance.Player.currentHealth);
      SceneManager.Instance.Player.HealthChanged += HeartsContainer.UpdateHearts;

      FollowCam = GetNode<FollowCam>("FollowCam");

      CurrentMap = GetNode<Node2D>("CurrentMap");
      currentLevel = CurrentMap.GetChild(0) as BaseLevel;
   }

   private void LoadStart(LoadingScreen loadingScreen)
   {
      //GD.Print("M  - LoadStart");

      /*
      loadingScreen.Reparent(this);
      MoveChild(loadingScreen, GUI.GetIndex());
      */
   }

   private void LevelLoaded(Node2D level)
   {
      //GD.Print("M  - LevelLoaded");

      if (level is BaseLevel)
      {
         currentLevel = (BaseLevel)level;
      }
   }

   private void LevelAdded(Node2D level, LoadingScreen loadingScreen)
   {
      //GD.Print("M  - LevelAdded");

      /*
      if (loadingScreen != null) {
         var loadingParent = loadingScreen.GetParent() as Node;
         loadingParent.MoveChild(loadingScreen, loadingParent.GetChildCount() - 1);
      }

      MoveChild(GUI, GetChildCount() - 1);
      */
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
