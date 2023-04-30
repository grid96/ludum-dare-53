using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelHolder : ScriptableObject
{
    public int Index;
    public List<TileData> Tiles;
    public List<EntityData> Entities;

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
    public TileType Type;
    public MapTile Tile;
    public Vector3Int Position;
    public int Rotation;
    public bool Mirrored;
}

[Serializable]
public class EntityData
{
    public EntityType Type;
    public Vector3 Position;
    public int VariantIndex;
    public float Rotation;
    public bool Mirrored;
}