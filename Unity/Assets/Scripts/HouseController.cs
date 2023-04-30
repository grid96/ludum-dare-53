using UnityEngine;

// [ExecuteInEditMode]
public class HouseController : EntityController
{
    public HouseController()
    {
        Type = EntityType.House;
    }

    private void Awake()
    {
        var t = transform;
        var localPosition = t.localPosition;
        localPosition = new Vector3(localPosition.x, 2, localPosition.z);
        t.localPosition = localPosition;
    }
}
