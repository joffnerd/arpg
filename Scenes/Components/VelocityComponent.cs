using Godot;
using GodotUtilities;
using System.Collections.Generic;
using System.Linq;

namespace ARPG.Components;

[Scene]
public partial class VelocityComponent : Node
{
   [Export]
   private float maxSpeed = 100;

   [Export]
   private float accelerationCoefficient = 10;

   [Export]
   private bool debugMode;

   public Vector2 Velocity { get; set; }
   public Vector2? VelocityOverride { get; set; }
   public float SpeedMultiplier { get; set; } = 1f;
   public float SpeedPercentModifier => speedPercentModifiers.Values.Sum();
   public float AccelerationCoefficientMultiplier { get; set; } = 1f;
   public float AccelerationCoefficient => accelerationCoefficient;
   public float SpeedPercent => Mathf.Min(Velocity.Length() / (CalculatedMaxSpeed > 0f ? CalculatedMaxSpeed : 1f), 1f);
   public float CalculatedMaxSpeed => maxSpeed * (1f + SpeedPercentModifier) * SpeedMultiplier;

   private Dictionary<string, float> speedPercentModifiers = new();

   public override void _Notification(int what)
   {
      if (what == NotificationSceneInstantiated)
      {
         WireNodes();
      }
   }

   public override void _Ready()
   {
      SetProcess(false);
      if(OS.IsDebugBuild() && debugMode)
      {
         (Owner as Node2D)?.Connect("draw", Callable.From(() => OnDebugDraw(Owner as Node2D)));
      }
   }

   public override void _Process(double delta)
   {
      (Owner as Node2D)?.QueueRedraw();
   }

   public void AccelerateToVelocity(Vector2 velocity)
   {
      // unknown float on end
      Velocity = Velocity.Lerp(velocity, 1f - Mathf.Exp(-accelerationCoefficient * AccelerationCoefficientMultiplier * (float)SpeedPercent));
   }

   public void AccelerateInDirection(Vector2 direction)
   {
      AccelerateToVelocity(direction * CalculatedMaxSpeed);
   }

   public Vector2 GetMaxVelocity(Vector2 direction)
   {
      return direction * CalculatedMaxSpeed;
   }

   public void MaximiseVelocity(Vector2 direction)
   {
      Velocity = GetMaxVelocity(direction);
   }

   public void Decelerate()
   {
      AccelerateToVelocity(Vector2.Zero);
   }

   public void Move(CharacterBody2D characterBody2D)
   {
      characterBody2D.Velocity = VelocityOverride ?? Velocity;
      characterBody2D.MoveAndSlide();
   }

   public void SetMaxSpeed(float newSpeed)
   {
      maxSpeed = newSpeed;
   }

   public void AddSpeedPercentModifier(string name, float change)
   {
      speedPercentModifiers.TryGetValue(name, out var currentValue);
      currentValue += change;
      speedPercentModifiers[name] = currentValue;
   }

   public void SetSpeedPercentModifier(string name, float val)
   {
      speedPercentModifiers[name] = val;
   }

   public float GetSpeedPercentModifier(string name)
   {
      speedPercentModifiers.TryGetValue(name, out var currentValue);
      return currentValue;
   }

   private void OnDebugDraw(Node2D owner)
   {
      owner.DrawLine(Vector2.Zero, Velocity, Colors.Cyan, 2f);
   }
}
