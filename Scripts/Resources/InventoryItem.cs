using Godot;

namespace ARPG.Resources;

[GlobalClass]
public partial class InventoryItem : Resource
{
   [Export]
   public string Name;

   [Export]
   public Texture2D Texture;

   [Export]
   public int MaxStackAmount = 10;
}
