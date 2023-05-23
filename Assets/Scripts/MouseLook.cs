using UnityEngine;

public class MouseLook : MonoBehaviour
{
    #region variables
    Vector2 _mouseFinal;
    Vector2 _smoothMouse;

    public Vector2 clampInDegrees = new Vector2(360, 180);
    public bool lockCursor;

    public Vector2 sensitivity = new Vector2(2, 2);
    public Vector2 smoothing = new Vector2(3, 3);
    Vector2 targetDirection;
    Vector2 targetCharacterDirection;

    public GameObject characterBody;

    public PlayerInputActions _input;
    #endregion
    
    private void OnEnable()
    {
        _input = new PlayerInputActions();
        _input.Enable();
    }

    void Start()
    {
        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;

        // Set target direction for the character body to its inital state.
        if (characterBody)
            targetCharacterDirection = characterBody.transform.localRotation.eulerAngles;

    }

    Vector2 ScaleAndSmooth(Vector2 _delta)
    {
        //Apply sensetivity
        _delta = Vector2.Scale(_delta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        //Lerp from last frame
        _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, _delta.x, 1f / smoothing.x);
        _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, _delta.y, 1f / smoothing.y);

        return _smoothMouse;
    }

    void LateUpdate()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        Vector2 mouseDelta = _input.pActionMap.MouseLook.ReadValue<Vector2>();
        _mouseFinal += ScaleAndSmooth(mouseDelta);

        ClampValues();
        AlignToBody();
    }

    void ClampValues()
    {
        // Clamp and apply the local x value first
        if (clampInDegrees.x < 360)
            _mouseFinal.x = Mathf.Clamp(_mouseFinal.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        // Then clamp y value.
        if (clampInDegrees.y < 360)
            _mouseFinal.y = Mathf.Clamp(_mouseFinal.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(targetDirection);
        transform.localRotation = Quaternion.AngleAxis(-_mouseFinal.y, targetOrientation * Vector3.right) * targetOrientation;

    }

    void AlignToBody()
    {
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        // If there's a character body that acts as a parent to the camera
        if (characterBody)
        {
            var yRotation = Quaternion.AngleAxis(_mouseFinal.x, Vector3.up);
            characterBody.transform.localRotation = yRotation * targetCharacterOrientation;
        }
        else
        {
            var yRotation = Quaternion.AngleAxis(_mouseFinal.x, transform.InverseTransformDirection(Vector3.up));
            transform.localRotation *= yRotation;
        }
    }
}