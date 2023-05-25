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

    }

    void OnEnable()
    {
        trajectoryPredictor = GetComponent<TrajectoryPredictor>();

        if (StartPosition == null)
            StartPosition = transform;

        fire.Enable();
        fire.performed += ThrowObject;
    }

    void Update()
    {
        Predict();
    }

    void Predict()
    {
        trajectoryPredictor.PredictTrajectory(ProjectileData());
    }

    ProjectileProperties ProjectileData()
    {
        ProjectileProperties properties = new ProjectileProperties();
        Rigidbody r = objectToThrow.GetComponent<Rigidbody>();

        properties.direction = StartPosition.forward;
        properties.initialPosition = StartPosition.position;
        properties.initialSpeed = throwForce;
        properties.mass = r.mass;
        properties.drag = r.drag;

        return properties;
    }

    void ThrowObject(InputAction.CallbackContext ctx)
    {
        Rigidbody thrownObject = Instantiate(objectToThrow, StartPosition.position, Quaternion.identity);
        thrownObject.AddForce(StartPosition.forward * throwForce, ForceMode.Impulse);
    }
}