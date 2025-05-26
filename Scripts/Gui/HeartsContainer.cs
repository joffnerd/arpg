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
      hearts = hearts / 4;
      for (int i = 0; i < hearts; i++)
      {
         var heart = HeartScene.Instantiate();
         AddChild(heart);
      }
   }

   public void UpdateHearts(int currentHealth)
   {
      if(currentHealth < 0)
      {
         return;
      }

      var hearts = GetChildren();
      var fullHearts = currentHealth / 4;


      for (int i = 0; i < fullHearts; i++)
      {
         hearts[i].Call("Update", 4);
      }

      if(fullHearts == hearts.Count)
      {
         return;
      }

      var remainder = currentHealth % 4;
      hearts[fullHearts].Call("Update", remainder);

      for (int i = (fullHearts+1); i < hearts.Count; i++)
      {
         hearts[i].Call("Update", 0);
      }
   }
}
