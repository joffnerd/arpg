using ARPG.Scripts.Autoload;
using Godot;

namespace ARPG.Scripts.Levels;

public partial class SceneTrigger : Area2D
{
   [Signal]
   public delegate void PlayerEnteredDoorEventHandler(SceneTrigger trigger);

   [Export]
   public Direction EntryDirection;

   [Export]
   public TransitionType Transition = TransitionType.fade_to_black;

   [Export]
   public string ConnectedScene;

   [Export]
   public string EntryDoorName = "Start";

   private int pushDistance = 16;
   private string sceneDir = "res://Scenes/Levels/";

   public void OnBodyEntered(Node2D body)
   {
      if (body is not Player.Player)
      {
         return;
      }

      EmitSignal(SignalName.PlayerEnteredDoor, this);      

      //GD.Print("ST - OnBodyEntered " + body.Name);

      var main = GetTree().GetNodesInGroup("MainGroup")[0] as Main;
      var unload = main.currentLevel;

      //GD.Print("ST - Unload " + unload.Name);

      SceneManager.Instance.Player.Disable();
      SceneManager.Instance.Player.Visible = false;

      var scene = sceneDir + ConnectedScene + ".tscn";
      if (Transition == TransitionType.zelda)
      {
         // not implemented
         // needs body.move_dir
      }
      else
      {
         SceneManager.Instance.SwapScenes(scene, main.CurrentMap, unload, "wipe_to_right");
      }
      QueueFree();
   }

   public Vector2 GetPlayerEntryVector()
   {
      var vector = Vector2.Left;
      switch (EntryDirection)
      {
         case Direction.north:
            vector = Vector2.Up;
            break;
         case Direction.east:
            vector = Vector2.Right;
            break;
         case Direction.south:
            vector = Vector2.Down;
            break;
      }

      return (vector * pushDistance) + Position;
   }

   public Vector2 GetMoveDir()
   {
      var dir = Vector2.Right;
      switch (EntryDirection)
      {
         case Direction.north:
            dir = Vector2.Down;
            break;
         case Direction.east:
            dir = Vector2.Left;
            break;
         case Direction.south:
            dir = Vector2.Up;
            break;
      }
      return dir;
   }
}

public enum Direction
{
   north = 0,
   east = 1,
   south = 2,
   west = 3
}

public enum TransitionType
{
   fade_to_black = 0,
   fade_to_white = 1,
   wipe_to_right = 2,
   zelda = 3,
   no_transition = 4
}