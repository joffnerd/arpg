using Godot;

namespace ARPG.Scripts.Player;

public partial class FollowCam : Camera2D
{
   [Export]
   public Player Player;

   public TileMapLayer TileMapLayer;
   public Node2D CurrentMap;

   public override void _Ready()
   {
      CurrentMap = GetTree().Root.GetNode<Node2D>("Main/CurrentMap");
   }

   public override void _PhysicsProcess(double delta)
   {
      GlobalPosition = Player.GlobalPosition;
   }

   public void SetPositionSmoothing(bool smoothing)
   {
      PositionSmoothingEnabled = smoothing;
   }

   public void SetCameraLimits()
   {
      TileMapLayer ground = CurrentMap.GetChildren()[0].GetNode<Node2D>("Layers").GetChildren()[0] as TileMapLayer;
      TileMapLayer = ground;

      var mapRect = TileMapLayer.GetUsedRect();
      var tileSize = TileMapLayer.RenderingQuadrantSize;
      var levelSizeinPx = mapRect.Size * tileSize;
      LimitRight = levelSizeinPx.X;
      LimitBottom = levelSizeinPx.Y;
   }
}
