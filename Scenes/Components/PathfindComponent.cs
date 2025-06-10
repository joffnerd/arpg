using Godot;
using GodotUtilities;

namespace ARPG.Components;

[Scene]
public partial class PathfindComponent : Node2D
{
   [Node]
   public NavigationAgent2D NavigationAgent2D { get; private set; }

   [Node]
   private Timer intervalTimer;

   [Export]
   private VelocityComponent velocityComponent;

   [Export]
   private bool debugDrawEnabled;

   public override void _Notification(int what)
   {
      if (what == NotificationSceneInstantiated)
      {
         WireNodes();
      }
   }

   public override void _Ready()
   {
      NavigationAgent2D.Connect("velocity_computed", new Callable(this, nameof(OnVelocityComputed)));
      SetProcess(OS.IsDebugBuild() && debugDrawEnabled);
   }

   public override void _Draw()
   {
      if (OS.IsDebugBuild() && debugDrawEnabled)
      {
         for (int i = 0; i < NavigationAgent2D.GetCurrentNavigationPath().Length; i++)
         {
            var point = NavigationAgent2D.GetCurrentNavigationPath()[i];
            DrawCircle(ToLocal(point), 3f, Colors.Orange);
            if (i > 0)
            {
               var previousPoint = NavigationAgent2D.GetCurrentNavigationPath()[i - 1];
               DrawLine(ToLocal(previousPoint), ToLocal(point), Colors.Orange, 2f);
            }
         }
      }
   }

   public override void _Process(double delta)
   {
      QueueRedraw();
   }

   public void SetTargetPosition(Vector2 targetPosition)
   {
      if (!intervalTimer.IsStopped()) return;

      intervalTimer.Call("start");
      NavigationAgent2D.TargetPosition = targetPosition;
   }

   public void FollowPath()
   {
      if (NavigationAgent2D.IsNavigationFinished())
      {
         velocityComponent.Decelerate();
         return;
      }

      var direction = (NavigationAgent2D.GetNextPathPosition() - GlobalPosition).Normalized();
      velocityComponent.AccelerateInDirection(direction);
      NavigationAgent2D.Velocity = velocityComponent.Velocity;
   }

   public void OnVelocityComputed(Vector2 velocity)
   {
      var newDirection = velocity.Normalized();
      var currentDirection = velocityComponent.Velocity.Normalized();
      var halfway = newDirection.Lerp(currentDirection, 1f - Mathf.Exp(velocityComponent.AccelerationCoefficient * (float)1)); // ????
      velocityComponent.Velocity = halfway * velocityComponent.Velocity.Length();
   }
}
