using UnityEngine;

public class FlyCam : MonoBehaviour
{

    public float speed = 1;
    public float mouseScrollAccel = 2;

    public bool localRotation;

    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;

    public bool canRotate = true;
    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis

    float initSpeed;
    float speedScrollMult = 1;

    public bool inFixedUpdate;

    [Header("Control")]
    public bool useNumpadArrows = true;
    public string horizontalAxisName = "Horizontal";
    public string verticalAxisName = "Vertical";

    public Transform parentable;
    public GameObject disableOtherCameraRig;

    Camera _cam;
    Camera cam { get { if (!_cam) _cam = GetComponent<Camera>(); return _cam; } }

    public Camera inCarCamera;

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;

        initSpeed = speed;
        speedScrollMult = Mathf.Sqrt(initSpeed);

        if (!parentable && transform.parent)
            parentable = transform.parent;

        if (disableOtherCameraRig)
            disableOtherCameraRig.SetActive(false);
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetMouseButtonDown(2)) canRotate = !canRotate;

        if (scroll != 0)
        {
            speedScrollMult += scroll * mouseScrollAccel;
            if (speedScrollMult < 0) speedScrollMult = 0;
            speed = speedScrollMult * speedScrollMult;
        }

        if (!inFixedUpdate)
            DoMouseLook();

        // TODO: This is quick and dirty
        if (Input.GetKey(KeyCode.Minus))
        {
            cam.fieldOfView += Time.deltaTime * 20;
            if (inCarCamera) inCarCamera.fieldOfView = cam.fieldOfView;
        }

        if (Input.GetKey(KeyCode.Equals))
        {
            cam.fieldOfView -= Time.deltaTime * 20;
            if (inCarCamera) inCarCamera.fieldOfView = cam.fieldOfView;
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
            ToggleParent();
    }

    void ToggleParent()
    {
        if (transform.parent == null)
            transform.parent = parentable;
        else
            transform.parent = null;
    }

    void FixedUpdate()
    {
        if (inFixedUpdate)
            DoMouseLook();
    }

    void DoMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        if (canRotate)
        {
            rotY += mouseX * mouseSensitivity * Time.deltaTime;
            rotX += mouseY * mouseSensitivity * Time.deltaTime;

            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

            Quaternion rot = Quaternion.Euler(rotX, rotY, 0.0f);

            if (!localRotation)
                transform.rotation = rot;
            else
                transform.localRotation = rot;
        }

        float z = 0;
        float x = 0;

        if (useNumpadArrows)
        {
            x = Input.GetKey(KeyCode.Keypad4) ? -1 : Input.GetKey(KeyCode.Keypad6) ? 1 : 0;
            z = Input.GetKey(KeyCode.Keypad5) ? -1 : Input.GetKey(KeyCode.Keypad8) ? 1 : 0;
        }
        else
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
        }

        transform.Translate(Vector3.forward * z * speed * Time.deltaTime, Space.Self);
        transform.Translate(Vector3.right * x * speed * Time.deltaTime, Space.Self);
    }
}
