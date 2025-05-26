using ARPG.Scripts;
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

   public AudioStream Success;
   public AudioStream Failure;

   public InventoryItem()
   {
      Success = ResourceLoader.Load<AudioStream>("res://Audio/Effects/Accept5.wav");
      Failure = ResourceLoader.Load<AudioStream>("res://Audio/Effects/Cancel2.wav");
   }

   public virtual void UseItem()
   {
      var audio = SceneManager.Instance.Player.AudioEffects;
      AudioStream result;

      if (CanUseItem())
      {
         result = Success;
      }
      else
      {
         result = Failure;
      }

      audio.Stream = result;
      audio.Play();
   }

   public virtual bool CanUseItem() {
      return true;
   }
}
