using ARPG.Components;
using Godot;
using GodotUtilities;
using GodotUtilities.Logic;

[Scene]
public partial class Temp : CharacterBody2D
{
   [Node]
   private PathfindComponent pathfindComponent;

   [Node]
   private VelocityComponent velocityComponent;

   private DelegateStateMachine stateMachine = new();

   public override void _Notification(int what)
   {
      if(what == NotificationSceneInstantiated)
      {
         WireNodes();
      }
   }

   public override void _Ready()
   {
      stateMachine.AddStates(StateNormal);
      stateMachine.SetInitialState(StateNormal);
   }

   public override void _Process(double delta)
   {
      stateMachine.Update();
   }

   private void StateNormal()
   {
      var target = this.GetPlayer()?.GlobalPosition ?? GlobalPosition;
      pathfindComponent.SetTargetPosition(target);
      pathfindComponent.FollowPath();
      velocityComponent.Move(this);
   }
}
