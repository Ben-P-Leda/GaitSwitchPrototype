using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Transform _transform;
    private Rigidbody _rigidBody;
    private Terrain _terrain;
    private Animator _animator;

    private bool _buttonWasPressed;
    private Rect _displayArea;
    private GUIStyle _guiStyle;

    public Vector3 CenterPoint;
    public float InnerBoundary;
    public float OuterBoundary;

    private void Start()
    {
        _transform = transform;
        _rigidBody = GetComponent<Rigidbody>();
        _animator = _transform.FindChild("Model").GetComponent<Animator>();

        _buttonWasPressed = false;

        _guiStyle = new GUIStyle();
        _displayArea = new Rect(0, 0, 800, 600);
    }

    private void Update()
    {
        Vector3 movementInput = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")) * Movement_Speed;
        _rigidBody.velocity = new Vector3(movementInput.x, _rigidBody.velocity.y, movementInput.z);
        if (movementInput != Vector3.zero)
        {
            _transform.LookAt(_transform.position + movementInput);
        }

        bool buttonIsPressed = (Input.GetAxis("Jump") > 0.0f);
        if ((buttonIsPressed) && (!_buttonWasPressed))
        {
            _buttonWasPressed = true;
            _animator.SetBool("OnAllFours", !_animator.GetBool("OnAllFours"));
        }
        else if (!buttonIsPressed)
        {
            _buttonWasPressed = false;
        }

        ClampToFloor();
    }

    private void ClampToFloor()
    {
        float floor = Terrain.activeTerrain.SampleHeight(_transform.position);
        _transform.position = new Vector3(_transform.position.x, Mathf.Max(_transform.position.y, floor + Floor_Offset), _transform.position.z);
    }

    private void OnGUI()
    {
        Vector3 flatPosition = new Vector3(_transform.position.x, 0.0f, _transform.position.z);
        Vector3 center = new Vector3(CenterPoint.x, 0.0f, CenterPoint.z);

        float innerDistance = InnerBoundary - Vector3.Distance(center, flatPosition);
        float difference = OuterBoundary - InnerBoundary;

        string boundaryMessage = innerDistance >= 0.0f
            ? ("Distance to inner boundary: " + innerDistance)
            : ("Ouside inner boundary! Opacity should be " + (1.0f - Mathf.Clamp01(-(innerDistance / difference))));

        string diagnostic =
            "WSAD to move\n" +
            (_animator.GetBool("OnAllFours") ? "Four-legged mode" : "Two-legged mode") + " - space to change...\n" +
            boundaryMessage;
        

        GUI.Label(_displayArea, diagnostic, _guiStyle);
    }

    private const float Movement_Speed = 5.0f;
    private const float Floor_Offset = 1.0f;
}
