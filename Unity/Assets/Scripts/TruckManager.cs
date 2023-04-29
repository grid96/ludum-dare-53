using UnityEngine;

public class TruckManager : MonoBehaviour
{
    public static TruckManager Instance { get; private set; }
    
    private static Camera cam => Camera.main;

    [SerializeField] private Rigidbody parcelPrefab;
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float driftFactor = 0.1f;
    [SerializeField] private float rotationSpeed = 180;
    [SerializeField] private float maxSpeed = 20;

    private Rigidbody rb;
    
    public TruckManager() => Instance = this;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void ThrowOutParcel()
    {
        var t = transform;
        Rigidbody parcel = Instantiate(parcelPrefab, t.position - t.forward * (t.localScale.z + parcelPrefab.transform.localScale.z) / 2, Quaternion.identity);
        parcel.transform.rotation = t.rotation;
        
        var right = transform.right;
        float driftAmount = Vector3.Dot(rb.velocity, right) * driftFactor;
        parcel.AddForce(-right * driftAmount - t.forward * 10f, ForceMode.VelocityChange);
    }

    private void Update()
    {
        if (cam == null)
            return;
        
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.down, Vector3.zero);
        if (!plane.Raycast(ray, out var distance))
            return;
        
        Vector3 targetPosition = ray.GetPoint(distance);
        var position = transform.position;
        targetPosition.y = position.y;
        Vector3 direction = (targetPosition - position).normalized;
        Quaternion toRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        
        var camTransform = cam.transform;
        camTransform.position = new Vector3(position.x, camTransform.position.y, position.z);

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            ThrowOutParcel();
    }

    private void FixedUpdate()
    {
        float moveAmount = moveSpeed * Time.fixedDeltaTime;
        rb.AddForce(transform.forward * moveAmount, ForceMode.VelocityChange);

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        var right = transform.right;
        float driftAmount = Vector3.Dot(rb.velocity, right) * driftFactor;
        rb.AddForce(-right * driftAmount, ForceMode.VelocityChange);
    }
}