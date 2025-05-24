using Godot;

namespace ARPG.Scripts.Gui;

public partial class Heart : Panel
{
    public Sprite2D Sprite;

    public override void _Ready()
    {
        Sprite = GetNode<Sprite2D>("Sprite2D");
    }

    public void Update(bool whole)
    {
        if (whole)
        {
            Sprite.Frame = 4;
        }
        else
        {
            Sprite.Frame = 0;
        }
    }
}
