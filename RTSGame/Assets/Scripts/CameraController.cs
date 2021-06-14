using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float NormalSpeed;
    public float FastSpeed;
    public float MovementTime;
    public float RotationSpeed;
    public Vector3 ZoomAmount;
    public Transform CameraTransform;
    public Transform FollowTransform;

    private float m_MovementSpeed;
    private Vector3 m_NewPosition;
    private Quaternion m_NewRotation;
    private Vector3 m_NewZoom;

    private Vector3 m_DragStartPosition;
    private Vector3 m_DragCurrentPosition;
    private Vector3 m_RotateStartPosition;
    private Vector3 m_RotateCurrentPosition;

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        m_NewPosition = transform.position;
        m_NewRotation = transform.rotation;
        m_NewZoom = CameraTransform.localPosition;
    }

    void Update()
    {
        if (FollowTransform != null)
        {
            transform.position = FollowTransform.position;
        }
        else
        {
            HandleKeyboardInput();
            HandleMouseInput();

            transform.position = Vector3.Lerp(transform.position, m_NewPosition, Time.deltaTime * MovementTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, m_NewRotation, Time.deltaTime * MovementTime);
            CameraTransform.localPosition = Vector3.Lerp(CameraTransform.localPosition, m_NewZoom, Time.deltaTime * MovementTime);
        }
        if( Input.GetKeyDown(KeyCode.Escape))
        {
            FollowTransform = null;
        }
    }

    public void HandleMouseInput()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            m_NewZoom += Input.mouseScrollDelta.y * ZoomAmount * 10.0f;
        }

        /*if ( Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;
            if( plane.Raycast( ray , out entry ))
            {
                m_DragStartPosition = ray.GetPoint(entry);
            }
        }
        if( Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;
            if (plane.Raycast(ray, out entry))
            {
                m_DragCurrentPosition = ray.GetPoint(entry);
                m_NewPosition = transform.position + m_DragStartPosition - m_DragCurrentPosition;
            }
        }
        if( Input.GetMouseButtonDown(1))
        {
            m_RotateStartPosition = Input.mousePosition;
        }
        if( Input.GetMouseButton(1))
        {
            m_RotateCurrentPosition = Input.mousePosition;
            Vector3 delta = m_RotateCurrentPosition - m_RotateStartPosition;
            m_RotateStartPosition = m_RotateCurrentPosition;
            m_NewRotation *= Quaternion.Euler(Vector3.up * (delta.x / 30.0f));
        }
        */
    }

    public void HandleKeyboardInput()
    {
        m_MovementSpeed = NormalSpeed;
        if( Input.GetKey(KeyCode.LeftShift))
        {
            m_MovementSpeed = FastSpeed;
        }

        if( Input.GetKey(KeyCode.Q))
        {
            m_NewRotation *= Quaternion.Euler(Vector3.up * RotationSpeed );
        }
        if (Input.GetKey(KeyCode.E))
        {
            m_NewRotation *= Quaternion.Euler(Vector3.up * -RotationSpeed);
        }

        if ( Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            m_NewPosition += transform.forward * m_MovementSpeed;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            m_NewPosition -= transform.forward * m_MovementSpeed;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            m_NewPosition -= transform.right * m_MovementSpeed;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            m_NewPosition += transform.right * m_MovementSpeed;
        }

        if( Input.GetKey(KeyCode.R))
        {
            m_NewZoom += ZoomAmount;
        }
        if (Input.GetKey(KeyCode.F))
        {
            m_NewZoom -= ZoomAmount;
        }   
    }
}
