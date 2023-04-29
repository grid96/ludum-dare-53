using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelHolder : ScriptableObject
{
    public int Index { get; set; }
    public List<TileData> Tiles { get; set; }
    public List<EntityData> Entities { get; set; }
    
    public void SaveAsset()
    {
        AssetDatabase.CreateAsset(this, $"Assets/Resources/Levels/Level{Index}.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

[Serializable]
public class TileData
{
    public TileType Type { get; set; }
    public MapTile Tile { get; set; }
    public Vector3Int Position { get; set; }
    public int Rotation { get; set; }
    public bool Mirrored { get; set; }
}

[Serializable]
public class EntityData
{
    public EntityType Type { get; set; }
    public Vector3 Position { get; set; }
    public int VariantIndex { get; set; }
    public float Rotation { get; set; }
    public bool Mirrored { get; set; }
}
