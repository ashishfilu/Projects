using UnityEngine;

public class Main : MonoBehaviour
{
    private Raycast _raycastExample;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        _raycastExample = new Raycast();
        _raycastExample.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        _raycastExample.Update();
    }
}

public class MainInstance
{
    public static Main Instance()
    {
        GameObject root = GameObject.Find("Root");
        Debug.Assert(root != null);
        return root.GetComponent<Main>();
    }
}

