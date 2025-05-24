using Godot;

namespace ARPG.Scripts.Player;

public partial class FollowCam : Camera2D
{
   public TileMapLayer TileMapLayer;
   public Node2D CurrentMap;

   public override void _Ready()
   {
      CurrentMap = GetTree().Root.GetNode<Node2D>("Main/CurrentMap");
   }

   public void SetLimits()
   {
      TileMapLayer ground = CurrentMap.GetChildren()[0].GetNode<Node2D>("Layers").GetChildren()[0] as TileMapLayer;
      TileMapLayer = ground;

      var mapRect = TileMapLayer.GetUsedRect();
      var tileSize = TileMapLayer.RenderingQuadrantSize;
      var worldSizeinPx = mapRect.Size * tileSize;
      LimitRight = worldSizeinPx.X;
      LimitBottom = worldSizeinPx.Y;
   }
}
