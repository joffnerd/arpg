using ARPG.Scripts.Autoload;
using Godot;

namespace ARPG.Resources;

[GlobalClass]
public partial class InventoryWeapon : InventoryItem
{
   public InventoryWeapon()
   {
      Success = ResourceLoader.Load<AudioStream>("res://Audio/Effects/LevelUp1.wav");
   }
}