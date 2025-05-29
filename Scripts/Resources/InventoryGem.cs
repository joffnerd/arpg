using ARPG.Resources;
using Godot;

namespace ARPG.Resources;

[GlobalClass]
public partial class InventoryGem : InventoryItem
{
    [Export]
    public int Value = 5;
}
