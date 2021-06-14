using UnityEngine;

public enum CameraType
{
    FlyBy,
    Platform_2D,
}
public class CameraController : MonoBehaviour
{
    [Range(1.0f, 10.0f)]
    public float MovementSpeed;
    [Range(1.0f, 50.0f)]
    public float RotationSpeed;
    [Range(0.0f,1.0f)]
    public float SmoothFactor;
    public CameraType Type = CameraType.FlyBy;

    private Vector3 mPreviousDirection;
    private Vector2 mPreviousMousePosition;
    private float mActualSpeed = 0.0f;
    private const float mAcceleration = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        mPreviousMousePosition = Input.mousePosition;
        mPreviousDirection = Vector3.zero;

        if( Type == CameraType.Platform_2D )
        {
            Vector3 position = new Vector3(-1, 2.5f, 0);
            Vector3 lookAtPoint = new Vector3(0, 2.0f, 0);
            transform.position = position;
            transform.forward = Vector3.Normalize(lookAtPoint - position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Transform transform = gameObject.transform;
        Vector3 right = transform.right;
        Vector3 forward = transform.forward;

        if( Type == CameraType.Platform_2D )
        {
            Vector3 targetPoint = transform.position + forward;
            targetPoint.y = transform.position.y;
            forward = Vector3.Normalize(targetPoint - transform.position);
        }

        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            direction += forward;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            direction -= forward;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            direction += right;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            direction -= right;
        }
        direction.Normalize();

        if( direction != Vector3.zero && direction != mPreviousDirection )
        {
            mActualSpeed = 0.5f;
        }

        if(direction != Vector3.zero)
        {
            if( mActualSpeed < 1)
            {
                mActualSpeed += mAcceleration * Time.deltaTime * 20.0f;
            }
            else
            {
                mActualSpeed = 1.0f;
            }
            mPreviousDirection = direction;
        }
        else
        {
            if( mActualSpeed > 0 )
            {
                mActualSpeed -= mAcceleration * Time.deltaTime * 50.0f;
            }
            else
            {
                mActualSpeed = 0.0f;
            }
        }

        if (Type == CameraType.FlyBy)
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 currentPosition = Input.mousePosition;
                Vector2 delta = currentPosition - mPreviousMousePosition;
                delta *= SmoothFactor;
                if (delta.magnitude > 0)
                {
                    forward = Quaternion.AngleAxis(RotationSpeed * Time.deltaTime * delta.x, transform.up) * forward;
                    forward = Quaternion.AngleAxis(-RotationSpeed * Time.deltaTime * delta.y, transform.right) * forward;
                    transform.forward = forward;
                }
            }
        }
        mPreviousMousePosition = Input.mousePosition;
        Vector3 position = transform.position;
        position += mPreviousDirection * mActualSpeed * MovementSpeed * Time.deltaTime;
        transform.position = position;
    }
}