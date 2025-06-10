using Godot;
using Godot.Collections;
using System;

namespace ARPG.Scripts.Objects;

public partial class Gem : Collectable
{
   public Sprite2D Sprite2D;
   public Array<string> GemNames;
   public Random Random;

   public bool spawnRandom = false;

   public enum GemType
   {
      Green = 0,
      Yellow = 1,
      Red = 2,
      Purple = 3
   }

   public override void _Ready()
   {
      AudioCollect = ResourceLoader.Load<AudioStream>("res://Audio/Effects/Coin.wav");
      Sprite2D = GetNode<Sprite2D>("Sprite2D");
      Random = new Random();

      string[] list = { "Green", "Yellow", "Red", "Purple", };
      GemNames = [];
      GemNames.AddRange(list);

      if (spawnRandom)
      {
         SpawnRandom();
      }
   }

   public void SpawnRandom()
   {
      int item = Random.Next(0, GemNames.Count);

      var gem = string.Format("res://Art/Objects/Valuable/Gem{0}.png", GemNames[item]);
      var texture = ResourceLoader.Load<CompressedTexture2D>(gem);
      Sprite2D.Texture = texture;
   }
}