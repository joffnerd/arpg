using Godot;

namespace ARPG.Scripts.Menus;

public partial class LoadingScreen : Node2D
{
   [Signal]
   public delegate void TransitionInCompleteEventHandler();

   public ProgressBar ProgressBar;
   public AnimationPlayer AnimationPlayer;
   public Timer Timer;

   private string startingAnimationName;

   public override void _Ready()
   {
      ProgressBar = GetNode<ProgressBar>("%ProgressBar");
      ProgressBar.Visible = false;

      AnimationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");

      Timer = GetNode<Timer>("Timer");  
   }

   public void OnTimerTimeout()
   {
      //GD.Print("LS - OnTimerTimeout");

      ProgressBar.Visible = true;
   }

   public void StartTransition(string animationName)
   {
      //GD.Print("LS - StartTransition");

      if (!AnimationPlayer.HasAnimation(animationName))
      {
         GD.PushWarning("animation does not exist - " + animationName);
         animationName = "fade_to_black";
      }

      startingAnimationName = animationName;

      AnimationPlayer.Play(animationName);
      Timer.Start();
   }

   public async void FinishTransition()
   {
      //GD.Print("LS - FinishTransition");

      if (Timer.TimeLeft > 0)
      {
         Timer.Stop();
      }

      var endingAnimationName = startingAnimationName.Replace("to", "from");
      if (!AnimationPlayer.HasAnimation(endingAnimationName))
      {
         GD.PushWarning("animation does not exist - " + endingAnimationName);
         endingAnimationName = "fade_from_black";         
      }
      AnimationPlayer.Play(endingAnimationName);

      await ToSignal(AnimationPlayer, "animation_finished");

      QueueFree();
   }

   public void ReportMidpoint()
   {
      //GD.Print("LS - ReportMidpoint");

      EmitSignal(SignalName.TransitionInComplete);
   }

   public void UpdateBar(float value)
   {
      //GD.Print("LS - UpdateBar");

      ProgressBar.Value = value;
   }
}
