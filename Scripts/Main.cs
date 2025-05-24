using ARPG.Scripts.Gui;
using Godot;

namespace ARPG.Scripts;

public partial class Main : Node
{
   public HeartsContainer HeartsContainer;
   public Player.Player Player;

   public override void _Ready()
   {
      HeartsContainer = GetNode<HeartsContainer>("GUI/HeartsContainer");
      Player = GetNode<Player.Player>("Player");

      HeartsContainer.SetMaxHearts(Player.maxHealth);
      HeartsContainer.UpdateHearts(Player.currentHealth);

      Player.HealthChanged += HeartsContainer.UpdateHearts;
   }

   public void OnInventoryClosed()
   {
      GetTree().Paused = false;
   }

   public void OnInventoryOpened()
   {
      GetTree().Paused = true;
   }
}
