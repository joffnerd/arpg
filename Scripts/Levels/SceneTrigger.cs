using Godot;

namespace ARPG.Scripts.Levels;

public partial class SceneTrigger : Area2D
{
   [Export]
   public string ConnectedScene;

   public void OnBodyEntered(Node2D area)
   {
      var sceneFolder = "res://Scenes/Levels/";
      var path = sceneFolder + ConnectedScene + ".tscn";
      GetTree().ChangeSceneToFile(path);
   }
}