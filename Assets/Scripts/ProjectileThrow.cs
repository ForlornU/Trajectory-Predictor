using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TrajectoryPredictor))]
public class ProjectileThrow : MonoBehaviour
{
    TrajectoryPredictor trajectoryPredictor;

    [SerializeField]
    public Rigidbody objectToThrow;

    [SerializeField, Range(0.0f, 50.0f)]
    float throwForce;

    [SerializeField]
    Transform StartPosition;

    public InputAction fire;

    void Start()
    {
        trajectoryPredictor = GetComponent<TrajectoryPredictor>();

        if (StartPosition == null)
            StartPosition = transform;
    }

    void OnEnable()
    {
        fire.Enable();
        fire.performed += ThrowObject;
    }

    void Update()
    {
        Predict();
    }

    void Predict()
    {
        Rigidbody r = objectToThrow.GetComponent<Rigidbody>();
        ProjectileProperties properties = new ProjectileProperties();

        properties.direction = StartPosition.forward;
        properties.initialPosition = StartPosition.position;
        properties.initialSpeed = throwForce;
        properties.mass = r.mass;
        properties.drag = r.drag;

        trajectoryPredictor.PredictTrajectory(properties);
    }

    void ThrowObject(InputAction.CallbackContext ctx)
    {
        Rigidbody thrownObject = Instantiate(objectToThrow, StartPosition.position, Quaternion.identity);
        thrownObject.AddForce(StartPosition.forward * throwForce, ForceMode.Impulse);
    }
}