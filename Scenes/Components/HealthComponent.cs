using Godot;
using GodotUtilities;

namespace ARPG.Components;

[Scene]
public partial class HealthComponent : Node2D
{
   [Signal]
   public delegate void HealthChangedEventHandler(HealthUpdate healthUpdate);

   [Signal]
   public delegate void EntityDiedEventHandler();

   [Node]
   private ProgressBar HealthBar { get; set; }

   [Export]
   public float MaxHealth
   {
      get => maxHealth;
      private set
      {
         maxHealth = value;
         if (CurrentHealth > maxHealth)
         {
            CurrentHealth = maxHealth;
         }
      }
   }

   [Export]
   private bool suppressDamageFloat;

   public bool HasHealthRemaining => !Mathf.IsEqualApprox(CurrentHealth, 0f);
   public float CurrentHealthPercent => MaxHealth > 0 ? currentHealth / MaxHealth : 0f;

   public float CurrentHealth
   {
      get => currentHealth;
      private set
      {
         var previousHealth = currentHealth;
         currentHealth = Mathf.Clamp(value, 0, MaxHealth);
         var healthUpdate = new HealthUpdate
         {
            PreviousHealth = previousHealth,
            CurrentHealth = currentHealth,
            MaxHealth = MaxHealth,
            HealthPercent = CurrentHealthPercent,
            IsHeal = previousHealth <= currentHealth
         };
         EmitSignal(SignalName.HealthChanged, healthUpdate);
         if (!HasHealthRemaining && !hasDied)
         {
            hasDied = true;
            EmitSignal(SignalName.EntityDied);
         }
      }
   }

   public bool IsDamaged => CurrentHealth < MaxHealth;

   private bool hasDied;
   private float currentHealth = 10;
   private float maxHealth = 10;
   //private TextFloat currentDamageFloat;

   public override void _Notification(int what)
   {
      if (what == NotificationSceneInstantiated)
      {
         WireNodes();
      }
   }

   public override void _Ready()
   {
      CallDeferred(nameof(InitHealth));
   }

   public void Damage(float damage, bool forceHideDamage = false)
   {
      CurrentHealth -= damage;
      if (!suppressDamageFloat && !forceHideDamage)
      {
         //currentDamageFloat = FloatingTextManager.CreateOrUseDamageFloat(currentDamageFloat, GlobalPosition, damage);
      }
   }

   public void Heal(float heal)
   {
      Damage(-heal, true);
   }

   public void SetMaxHealth(float health)
   {
      MaxHealth = health;
   }

   private void InitHealth()
   {
      CurrentHealth = MaxHealth;
   }

   public void ApplyScaling(Curve curve, float progress)
   {
      CallDeferred(nameof(ApplyScalingInternal), curve, progress);
   }

   private void ApplyScalingInternal(Curve curve, float progress)
   {
      var curveValue = curve.Sample(progress);
      MaxHealth += 1f + curveValue;
      CurrentHealth = MaxHealth;
   }

   public partial class HealthUpdate : RefCounted
   {
      public float PreviousHealth;
      public float CurrentHealth;
      public float MaxHealth;
      public float HealthPercent;
      public bool IsHeal;
   }
}
