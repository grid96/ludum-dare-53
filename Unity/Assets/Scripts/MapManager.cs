using System.Collections.Generic;
using System.Linq;
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
    public List<TargetController> Targets => entitiesContainer.GetComponentsInChildren<TargetController>().ToList();

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
            var entityController = entity.GetComponent<EntityController>();
            var entityData = new EntityData
            {
                Type = entityController.Type,
                Position = entity.position,
                VariantIndex = entityController.VariantIndex,
                IsSprite = entityController.IsSprite,
                Rotation = entityController.Rotation,
                Mirrored = entityController.Mirrored
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
        var entities = new List<Transform>();
        foreach (Transform entity in entitiesContainer)
            entities.Add(entity);
        foreach (Transform entity in entities)
            DestroyImmediate(entity.gameObject);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        Clear();
        var level = Resources.FindObjectsOfTypeAll<LevelHolder>().FirstOrDefault(l => l.Index == levelIndex);
        if (level == null)
            return;
        foreach (var tile in level.Tiles)
        {
            if (tile.Type >= TileType.Road)
            {
                roadMap.SetTile(tile.Position, tile.Tile);
                RotateTile(roadMap, tile.Position, tile.Rotation);
            }
            else
            {
                terrainMap.SetTile(tile.Position, tile.Tile);
                RotateTile(terrainMap, tile.Position, tile.Rotation);
            }
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

            if (prefab == null)
                continue;

            EntityController e = Instantiate(prefab, entity.Position, Quaternion.Euler(entity.IsSprite ? 90 : 0, entity.Rotation, 0), entitiesContainer);
            e.VariantIndex = entity.VariantIndex;
            // e.Rotation = entity.Rotation;
            // e.Mirrored = entity.Mirrored;
        }
        
        MiniMapManager.Instance.Generate();
    }

    public void RotateTile(Tilemap tilemap, Vector3Int position, int rotation)
    {
        if (!tilemap.HasTile(position))
            return;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90 * rotation), Vector3.one);
        tilemap.SetTransformMatrix(position, rotationMatrix);
    }
}