using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    public int MapWidth { get; private set; } = 16;
    public int MapHeight { get; private set; } = 16;

    public MapManager() => Instance = this;
}