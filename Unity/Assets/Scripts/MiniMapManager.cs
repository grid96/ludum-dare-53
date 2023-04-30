using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MiniMapManager : MonoBehaviour
{
    public static MiniMapManager Instance { get; private set; }

    [SerializeField] private RawImage rawImage;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Color emptyColor = Color.clear;
    [SerializeField] private Color filledColor = Color.gray;
    [SerializeField] private Transform entitiesContainer;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private Transform truck;
    
    private readonly Dictionary<TargetController, GameObject> targets = new();

    public MiniMapManager() => Instance = this;

    private void Start() => Generate();

    [ContextMenu("Generate")]
    public void Generate()
    {
        Texture2D texture = new Texture2D(16, 16, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point
        };
        for (int x = 0; x < texture.width; x++)
        for (int y = 0; y < texture.height; y++)
            texture.SetPixel(x, y, tilemap.GetTile(new Vector3Int(x - 8, y - 8)) == null ? emptyColor : filledColor);
        texture.Apply();
        rawImage.texture = texture;
        var entities = new List<Transform>();
        foreach (Transform entity in entitiesContainer)
            entities.Add(entity);
        foreach (Transform entity in entities)
            DestroyImmediate(entity.gameObject);
        targets.Clear();
        foreach (var target in MapManager.Instance.Targets)
        {
            var targetObject = Instantiate(targetPrefab, entitiesContainer);
            var position = target.transform.position;
            var targetPosition = new Vector3(position.x / 5, position.z / 5, 0);
            targetObject.transform.localPosition = targetPosition;
            targets.Add(target, targetObject);
        }
        
        Update();
    }

    private void Update()
    {
        foreach (var target in targets)
            if (target.Key == null)
                target.Value.SetActive(false);
        
        var truckPosition = TruckManager.Instance.transform.position;
        truck.localPosition = new Vector3(truckPosition.x / 5, truckPosition.z / 5, 0);
    }
}