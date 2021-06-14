using UnityEngine;

public class Root : MonoBehaviour
{
    [Range(1,10)]
    public int StepAngle;
    public bool UseJob;

    private Raycast m_RaycastExample;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_RaycastExample = new Raycast(StepAngle,UseJob);
        m_RaycastExample.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        m_RaycastExample.Update();
    }

    private void OnDestroy()
    {
        m_RaycastExample.DisposeNativeArrays();
    }
}
