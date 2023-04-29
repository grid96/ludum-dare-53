using UnityEngine;

public class EntityController : MonoBehaviour
{
    public EntityType Type { get; set; }
    public int VariantIndex { get; set; }

    public float Rotation
    {
        get => transform.rotation.eulerAngles.y;
        set => transform.rotation = Quaternion.Euler(0, value, 0);
    }

    public bool Mirrored
    {
        get => transform.localScale.x < 0;
        set => transform.localScale = new Vector3(value ? -1 : 1, 1, 1);
    }
}

public enum EntityType
{
    Target,
    House
}