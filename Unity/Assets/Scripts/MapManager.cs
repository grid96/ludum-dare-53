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
    [SerializeField] private Transform parcelsContainer;
    [SerializeField] private HouseController[] housePrefabs;
    [SerializeField] private TargetController[] targetPrefabs;
    [SerializeField] private int levelIndex;

    public LevelHolder Level { get; private set; }
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
        var parcels = new List<Transform>();
        foreach (Transform parcel in parcelsContainer)
            parcels.Add(parcel);
        foreach (Transform parcel in parcels)
            DestroyImmediate(parcel.gameObject);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        Load(levelIndex);
    }

    public void Load(int levelIndex)
    {
        _ = DialogManager.Instance.Hide();
        var level = Resources.FindObjectsOfTypeAll<LevelHolder>().FirstOrDefault(l => l.Index == levelIndex);
        if (level == null)
            level = Resources.Load<LevelHolder>($"Levels/Level{levelIndex}");
        Level = level;
        if (level == null)
        {
            if (levelIndex <= 0)
                return;
            Load(levelIndex - 1);
            _ = DialogManager.Instance.EndOfGameDialog();
            return;
        }

        Clear();
        this.levelIndex = levelIndex;
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

        TruckManager.Instance.MaxSpeed = level.TruckSpeed;
        if (level.Index == 1)
            _ = DialogManager.Instance.FasterTruckDialog();

        var truckTransform = TruckManager.Instance.transform;
        truckTransform.localPosition = new Vector3(7, 1.2f, -90);
        truckTransform.localRotation = Quaternion.identity;

        MiniMapManager.Instance.Generate();

        ProgressManager.Instance.Init(level.Entities.Count(e => e.Type == EntityType.Target));
    }

    public void RotateTile(Tilemap tilemap, Vector3Int position, int rotation)
    {
        if (!tilemap.HasTile(position))
            return;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 90 * rotation), Vector3.one);
        tilemap.SetTransformMatrix(position, rotationMatrix);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.PageUp))
            Load(levelIndex + 1);
        if (Input.GetKeyDown(KeyCode.PageDown))
            Load(levelIndex - 1);
        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.R))
            Load(levelIndex);
    }
}