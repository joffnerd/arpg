using ARPG.Scripts.Levels;
using ARPG.Scripts.Player;
using Godot;

namespace ARPG.Scripts;

public partial class SceneManager : Node
{
   public static SceneManager Instance { get; private set; }

   public Node2D CurrentMap;
   public Player.Player Player;
   public string LastScene;

   private string sceneDir = "res://Scenes/Levels/";

   public override void _Ready()
   {
      Instance = this;

      CurrentMap = GetNode<Node2D>("/root/Main/CurrentMap");
   }

   public void ChangeScene(Node from, string to)
   {
      if (from != null)
      {
         LastScene = from.Name;
      }

      var path = sceneDir + to + ".tscn";

      var scene = ResourceLoader.Load<PackedScene>(path);
      var newMap = scene.Instantiate() as BaseLevel; // House / Level
      newMap.GlobalPosition = new Vector2(0, 0);

      if (CurrentMap.GetChildCount() > 0)
      {
         var currentMap = CurrentMap.GetChild(0);
         CurrentMap.CallDeferred("remove_child", currentMap);
      }

      var camera = GetNode<Camera2D>("/root/Main/FollowCam") as FollowCam;
      camera.SetPositionSmoothing(false);

      CurrentMap.CallDeferred("add_child", newMap);

      camera.CallDeferred("SetPositionSmoothing", true);      
      camera.CallDeferred("SetLimits");
   }
}
