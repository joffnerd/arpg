using ARPG.Scripts.Autoload;
using Godot;

namespace ARPG.Scripts.Menus;

public partial class StartScreen : Control
{
   public Label VersionNum;
   public AudioStreamPlayer Music;

   private string start = "res://Scenes/Main.tscn";

   public override void _Ready()
   {
      Music = GetNode<AudioStreamPlayer>("Music");

      VersionNum = GetNode<Label>("VersionNum");
      VersionNum.Text = SceneManager.Instance.VERSION;
      //GD.Print(string.Format("Version: {0}", SceneManager.Instance.VERSION));

      var cl = new CanvasLayer
      {
         Name = "LSHolder"
      };
      GetTree().Root.CallDeferred("add_child", cl);      
   }

   public override void _Process(double delta)
   {
      if (Input.IsActionJustPressed("ui_accept"))
      {
         Start();
      }
   }


   public void OnEnterButtonUp()
   {
      Start();
   }

   public void Start()
   {
      var sound = ResourceLoader.Load<AudioStream>("res://Audio/Effects/Accept5.wav");
      Music.Stream = sound;
      Music.Play();
      SceneManager.Instance.SwapScenes(start, GetTree().Root, this, "wipe_to_right");
   }
}
