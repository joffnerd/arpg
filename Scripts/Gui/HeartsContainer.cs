using Godot;

namespace ARPG.Scripts.Gui;

public partial class HeartsContainer : HBoxContainer
{
   public PackedScene HeartScene;

   public override void _Ready()
   {
      HeartScene = GD.Load<PackedScene>("res://Scenes/Gui/Heart.tscn");
   }

   public void SetMaxHearts(int hearts)
   {
      for (int i = 0; i < hearts; i++)
      {
         var heart = HeartScene.Instantiate();
         AddChild(heart);
      }
   }

   public void UpdateHearts(int current)
   {
      if(current < 0)
      {
         return;
      }

      var hearts = GetChildren();

      for (int i = 0; i < current; i++)
      {
         hearts[i].Call("Update", true);
      }

      for (int i = current; i < hearts.Count; i++)
      {
         hearts[i].Call("Update", false);
      }
   }
}
