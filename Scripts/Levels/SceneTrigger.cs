using Godot;

namespace ARPG.Scripts.Levels;

public partial class SceneTrigger : Area2D
{
   [Export]
   public string ConnectedScene;

   public void OnBodyEntered(Node2D body)
   {
      if(body is Player.Player)
      {
         Node2D owner = GetOwner() as Node2D; // Level
         SceneManager.Instance.ChangeScene(owner, ConnectedScene);
      }
   }
}