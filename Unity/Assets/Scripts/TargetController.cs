using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetController : EntityController
{
    private readonly List<Collider> colliders = new();

    public TargetController()
    {
        Type = EntityType.Target;
        IsSprite = true;
    }

    private void Awake()
    {
        var localRotation = transform.localRotation;
        localRotation = Quaternion.Euler(90, localRotation.eulerAngles.y, localRotation.eulerAngles.z);
        transform.localRotation = localRotation;
    }
    
    private void Update()
    {
        var parcel = colliders.FirstOrDefault(c => c.GetComponent<ParcelController>() && c.GetComponent<Rigidbody>()?.velocity.magnitude < 0.001f);
        if (parcel == null)
            return;
        ProgressManager.Instance.MakeProgress();
        colliders.Remove(parcel);
        Destroy(parcel.gameObject);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!colliders.Contains(collider))
            colliders.Add(collider);
    }

    private void OnTriggerExit(Collider collider)
    {
        colliders.Remove(collider);
    }
}