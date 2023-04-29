using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Tile", menuName = "Level/Tile", order = 1)]
public class MapTile : Tile
{
    public TileType Type { get; set; }
    public int Rotation { get; set; }
    public bool Mirrored { get; set; }
}

public enum TileType
{
    NoTerrain = 0,
    Grass,
    NoRoad = 100,
    Road,
    RoadTurn,
    RoadIntersection,
    RoadBigIntersection,
    RoadDeadEnd
}