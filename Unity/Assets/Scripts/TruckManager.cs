using CarterGames.Assets.AudioManager;
using UnityEngine;

public class TruckManager : MonoBehaviour
{
    public static TruckManager Instance { get; private set; }

    private static Camera cam => Camera.main;

    [SerializeField] private Transform parcelsContainer;
    [SerializeField] private Rigidbody parcelPrefab;
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float maxSpeed = 20;
    [SerializeField] private float rotationSpeed = 180;
    [SerializeField] private float maxParcelRotationSpeed = 10;

    [SerializeField] private float driftFactor = 0.1f;
    [SerializeField] private float driftThreshold = 1.0f;
    // [SerializeField] private float leanFactor = 0.2f;

    private Rigidbody rb;
    private float driftAmount;

    public float MaxSpeed
    {
        get => maxSpeed;
        set => maxSpeed = value;
    }

    public TruckManager() => Instance = this;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void ThrowOutParcel()
    {
        var t = transform;
        Rigidbody parcel = Instantiate(parcelPrefab, t.position - t.forward * (t.localScale.z + parcelPrefab.transform.localScale.z / 50) / 2, Quaternion.identity, parcelsContainer);
        parcel.transform.rotation = t.rotation * parcelPrefab.transform.rotation;

        float randomRotationSpeed = Random.Range(0, maxParcelRotationSpeed);
        Vector3 randomDirection = Random.insideUnitSphere;
        parcel.angularVelocity = randomDirection * randomRotationSpeed;

        // parcel.AddForce(-transform.right * driftAmount * -10, ForceMode.VelocityChange);
        parcel.AddForce(-t.forward * 10, ForceMode.VelocityChange);

        if (parcelsContainer.childCount + ProgressManager.Instance.Progress == 100)
            _ = DialogManager.Instance.ManyParcelsDialog();

        ProgressManager.Instance.StartCooldown();
        
        AudioManager.instance.Play("Throw", 0.5f, Random.Range(0.9f, 1.1f));
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
        Transform t = transform;
        Vector3 position = t.position;
        targetPosition.y = position.y;
        Vector3 direction = (targetPosition - position).normalized;
        Quaternion toRotation = Quaternion.LookRotation(direction);
        Quaternion rotation = t.rotation;
        rotation = Quaternion.RotateTowards(rotation, toRotation, rotationSpeed * Time.deltaTime);

        // var right = t.right;
        // float leanAmount = Vector3.Dot(rb.velocity, right) * leanFactor;
        // rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, leanAmount);
        t.rotation = rotation;

        Transform camTransform = cam.transform;
        Vector3 camPosition = new Vector3(position.x, camTransform.position.y, position.z);
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        float clampedX = Mathf.Clamp(camPosition.x, -MapManager.Instance.MapWidth * 5 + camWidth, MapManager.Instance.MapWidth * 5 - camWidth);
        float clampedZ = Mathf.Clamp(camPosition.z, -MapManager.Instance.MapHeight * 5 + camHeight, MapManager.Instance.MapHeight * 5 - camHeight);
        camPosition = new Vector3(clampedX, camPosition.y, clampedZ);
        camTransform.position = camPosition;

        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && !DialogManager.Instance.IsShown && !ScoringManager.Instance.IsShown && !IntroManager.Instance.IsShown)
            if (!ProgressManager.Instance.OnCooldown)
                ThrowOutParcel();
            else
                _ = DialogManager.Instance.ParcelCooldownDialog();
    }
    
    /*private void FixedUpdate()
    {
        float moveAmount = moveSpeed * Time.fixedDeltaTime;
        if (!ScoringManager.Instance.IsShown)
            rb.AddForce(transform.forward * moveAmount, ForceMode.VelocityChange);

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        Vector3 right = transform.right;
        float currentSpeed = rb.velocity.magnitude;
        float dotProduct = Vector3.Dot(rb.velocity.normalized, right);
        driftAmount = Mathf.Abs(dotProduct) > driftThreshold ? dotProduct * driftFactor : 0;

        rb.AddForce(-right * driftAmount, ForceMode.VelocityChange);
    }*/

    private void FixedUpdate()
    {
        float moveAmount = moveSpeed * Time.fixedDeltaTime;
        if (!ScoringManager.Instance.IsShown)
            rb.AddForce(transform.forward * moveAmount, ForceMode.VelocityChange);

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        Vector3 right = transform.right;
        driftAmount = Vector3.Dot(rb.velocity, right) * driftFactor;
        rb.AddForce(-right * driftAmount, ForceMode.VelocityChange);
    }
}