using Godot;

namespace ARPG.Scripts.Player;

public partial class FollowCam : Camera2D
{
   public TileMapLayer TileMapLayer;

   public override void _Ready()
   {
      Node2D world = GetTree().Root.GetNode<Node2D>("Main/World");
      TileMapLayer ground = world.GetNode<Node2D>("Layers").GetChildren()[0] as TileMapLayer;
      TileMapLayer = ground;

      var mapRect = TileMapLayer.GetUsedRect();
      var tileSize = TileMapLayer.RenderingQuadrantSize;
      var worldSizeinPx = mapRect.Size * tileSize;
      LimitRight = worldSizeinPx.X;
      LimitBottom = worldSizeinPx.Y;
   }
}
