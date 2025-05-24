using Godot;

namespace ARPG.Resources;

[GlobalClass]
public partial class InventoryHealth : InventoryItem
{
   [Export]
   public int HealthAmount = 1;
}

