using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [SerializeField] private Tilemap terrainMap;
    [SerializeField] private Tilemap roadMap;
    [SerializeField] private Transform entitiesContainer;
    [SerializeField] private HouseController[] housePrefabs;
    [SerializeField] private TargetController[] targetPrefabs;
    [SerializeField] private int levelIndex;

    public int MapWidth { get; private set; } = 16;
    public int MapHeight { get; private set; } = 16;

    public MapManager() => Instance = this;

    [ContextMenu("Save")]
    public void Save()
    {
        var newLevel = ScriptableObject.CreateInstance<LevelHolder>();
        newLevel.Index = levelIndex;
        newLevel.Tiles = GetTilesFromMap();
        newLevel.Entities = GetEntitiesFromMap();
        newLevel.SaveAsset();
    }

    private List<TileData> GetTilesFromMap()
    {
        var tiles = new List<TileData>();
        foreach (var position in terrainMap.cellBounds.allPositionsWithin)
        {
            var tile = terrainMap.GetTile<MapTile>(position);
            if (tile == null)
                continue;
            var tileData = new TileData
            {
                Type = tile.Type,
                Tile = tile,
                Position = position,
                Rotation = tile.Rotation,
                Mirrored = tile.Mirrored
            };
            tiles.Add(tileData);
        }

        foreach (var position in roadMap.cellBounds.allPositionsWithin)
        {
            var tile = roadMap.GetTile<MapTile>(position);
            if (tile == null)
                continue;
            var tileData = new TileData
            {
                Type = tile.Type,
                Tile = tile,
                Position = position,
                Rotation = tile.Rotation,
                Mirrored = tile.Mirrored
            };
            tiles.Add(tileData);
        }

        return tiles;
    }

    private List<EntityData> GetEntitiesFromMap()
    {
        var entities = new List<EntityData>();
        foreach (Transform entity in entitiesContainer)
        {
            var houseController = entity.GetComponent<EntityController>();
            var entityData = new EntityData
            {
                Type = EntityType.House,
                Position = entity.position,
                VariantIndex = houseController.VariantIndex,
                Rotation = houseController.Rotation,
                Mirrored = houseController.Mirrored
            };
            entities.Add(entityData);
        }

        return entities;
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        terrainMap.ClearAllTiles();
        roadMap.ClearAllTiles();
        foreach (Transform entity in entitiesContainer)
            DestroyImmediate(entity.gameObject);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        Clear();
        var level = Resources.Load<LevelHolder>($"Levels/Level{levelIndex}.asset");
        if (level == null)
            return;
        foreach (var tile in level.Tiles)
        {
            if (tile.Type >= TileType.Road)
                roadMap.SetTile(tile.Position, tile.Tile);
            else
                terrainMap.SetTile(tile.Position, tile.Tile);
        }

        foreach (var entity in level.Entities)
        {
            EntityController prefab = null;
            switch (entity.Type)
            {
                case EntityType.House:
                    prefab = housePrefabs[entity.VariantIndex];
                    break;
                case EntityType.Target:
                    prefab = targetPrefabs[entity.VariantIndex];
                    break;
            }

            EntityController e = Instantiate(prefab, entity.Position, Quaternion.Euler(0, entity.Rotation, 0), entitiesContainer);
            e.VariantIndex = entity.VariantIndex;
            e.Rotation = entity.Rotation;
            e.Mirrored = entity.Mirrored;
        }
    }
}