using UnityEngine;

public class MouseLook : MonoBehaviour
{
    #region variables
    Vector2 mouseFinal;
    Vector2 smoothMouse;
    Vector2 targetDirection;
    Vector2 targetCharacterDirection;

    public Vector2 clampInDegrees = new Vector2(360f, 180f);
    public Vector2 sensitivity = new Vector2(0.1f, 0.1f);
    public Vector2 smoothing = new Vector2(1f, 1f);

    public bool lockCursor;
    public GameObject characterBody;

    public PlayerInputActions input;
    #endregion
    
    private void OnEnable()
    {
        input = new PlayerInputActions();
        input.Enable();
    }

    void Start()
    {
        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;

        // Set target direction for the character body to its inital state.
        if (characterBody)
            targetCharacterDirection = characterBody.transform.localRotation.eulerAngles;

    }

    Vector2 ScaleAndSmooth(Vector2 delta)
    {
        //Apply sensetivity
        delta = Vector2.Scale(delta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        //Lerp from last frame
        smoothMouse.x = Mathf.Lerp(smoothMouse.x, delta.x, 1f / smoothing.x);
        smoothMouse.y = Mathf.Lerp(smoothMouse.y, delta.y, 1f / smoothing.y);

        return smoothMouse;
    }

    void LateUpdate()
    {
        if (lockCursor)
            Cursor.lockState = CursorLockMode.Locked;

        Vector2 mouseDelta = input.pActionMap.MouseLook.ReadValue<Vector2>();
        mouseFinal += ScaleAndSmooth(mouseDelta);

        ClampValues();
        AlignToBody();
    }

    void ClampValues()
    {
        // Clamp and apply the local x value first
        if (clampInDegrees.x < 360)
            mouseFinal.x = Mathf.Clamp(mouseFinal.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        // Then clamp y value.
        if (clampInDegrees.y < 360)
            mouseFinal.y = Mathf.Clamp(mouseFinal.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(targetDirection);
        transform.localRotation = Quaternion.AngleAxis(-mouseFinal.y, targetOrientation * Vector3.right) * targetOrientation;

    }

    void AlignToBody()
    {
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);
        Quaternion yRotation = Quaternion.identity;

        // If there's a character body that acts as a parent to the camera
        if (characterBody)
        {
            yRotation = Quaternion.AngleAxis(mouseFinal.x, Vector3.up);
            characterBody.transform.localRotation = yRotation * targetCharacterOrientation;
        }
        else
        {
            yRotation = Quaternion.AngleAxis(mouseFinal.x, transform.InverseTransformDirection(Vector3.up));
            transform.localRotation *= yRotation;
        }
    }
}