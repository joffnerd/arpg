using ARPG.Resources;
using Godot;

namespace ARPG.Scripts.Objects;

public partial class Gem : Collectable
{
    public Sprite2D Sprite;
    public override void _Ready()
    {
        AudioCollect = ResourceLoader.Load<AudioStream>("res://Audio/Effects/Coin.wav");
    }
}
