using UnityEngine;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public enum Mode
{
    Edit_Environment,
    Spawn_Green_Dominos,
    Spawn_Red_Dominos,
    Spawn_Ball
};

public enum CubeType
{
    Black,
    White
}

public class GameRoot : MonoBehaviour
{
    public GameObject DominosRoot;
    public int Rows;
    public int Columns;
    public GameObject GridRoot;
    public GameObject GameCamera;

    private RaycastHit m_HitObject;
    private Mode m_Mode = Mode.Edit_Environment;
    
    private GameObject m_GreenDomino;
    private GameObject m_RedDomino;
    private GameObject m_Ball;

    private GameObject m_Cell;
    private float m_CellSize;
    private Material m_Black;
    private CameraType m_CameraType;

    // Start is called before the first frame update
    void Start()
    {
        GameEventManager.Instance.SubscribeEventListener(EventId.KeyPad_1_Pressed, ProcessButtonPress);
        GameEventManager.Instance.SubscribeEventListener(EventId.KeyPad_2_Pressed, ProcessButtonPress);
        GameEventManager.Instance.SubscribeEventListener(EventId.KeyPad_3_Pressed, ProcessButtonPress);
        GameEventManager.Instance.SubscribeEventListener(EventId.KeyPad_4_Pressed, ProcessButtonPress);
        GameEventManager.Instance.SubscribeEventListener(EventId.Spacebar_Pressed, ProcessButtonPress);

        m_GreenDomino = Resources.Load<GameObject>("Prefabs/DominoCube_Green");
        m_RedDomino = Resources.Load<GameObject>("Prefabs/DominoCube_Red");
        m_Ball = Resources.Load<GameObject>("Prefabs/Ball");
        m_Cell = Resources.Load<GameObject>("Prefabs/CubeCell");
        m_CellSize = m_Cell.transform.localScale.x;

        m_Black = Resources.Load<Material>("Material/Black");
        m_CameraType = GameCamera.GetComponent<CameraController>().Type;
        CreateBaseGrid();
    }

    void CreateBaseGrid()
    {
        Transform referenceObjectTransform = GridRoot.transform;
        Vector3 startPosition = referenceObjectTransform.position;
        startPosition.x -= Rows * 0.5f * m_CellSize;
        startPosition.z -= Columns * 0.5f * m_CellSize;

        bool useBlack;
        bool startWithBlack = false;

        for (int i = 0; i < Rows; i++)
        {
            useBlack = startWithBlack;
            for (int j = 0; j < Columns; j++)
            {
                GameObject temp = GameObject.Instantiate(m_Cell);
                temp.transform.parent = referenceObjectTransform.transform;
                temp.transform.position = startPosition;
                startPosition.z += m_CellSize;
                temp.GetComponent<CubeBehavior>().Type = useBlack ? CubeType.Black : CubeType.White;
                if (useBlack)
                {
                    temp.GetComponentInChildren<Renderer>().material = m_Black;
                }
                useBlack = !useBlack;
            }
            startWithBlack = !startWithBlack;
            startPosition.x += m_CellSize;
            startPosition.z = referenceObjectTransform.position.x - Columns * 0.5f * m_CellSize;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetMouseButtonDown(1) && m_Mode == Mode.Edit_Environment)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out m_HitObject))
            {
                if (m_HitObject.transform.gameObject.CompareTag("Ground") && m_HitObject.transform.position.y > m_CellSize*0.5f)
                {
                    GameObject.Destroy(m_HitObject.transform.gameObject);
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out m_HitObject))
            {
                if( m_HitObject.transform.gameObject.CompareTag("Ground"))
                {
                    Vector3 position = m_HitObject.transform.position;

                    if (m_Mode == Mode.Edit_Environment)
                    {
                        CubeBehavior behavior = m_HitObject.transform.parent.gameObject.GetComponent<CubeBehavior>();

                        if (!Physics.Raycast(new Ray(position, Vector3.up), out m_HitObject))
                        {
                            position.y += m_CellSize * 0.5f;
                            GameObject temp = GameObject.Instantiate(m_Cell);
                            temp.transform.parent = GridRoot.transform;
                            temp.transform.position = position;

                            if (behavior.Type == CubeType.White)
                            {
                                temp.GetComponent<CubeBehavior>().Type = CubeType.Black;
                                temp.GetComponentInChildren<Renderer>().material = m_Black;
                            }
                            else
                            {
                                //No need to change material as default material is white
                                temp.GetComponent<CubeBehavior>().Type = CubeType.White;
                            }
                        }
                    }
                    else if (m_Mode == Mode.Spawn_Green_Dominos)
                    {
                        position.y += m_CellSize * 0.6f;//Little bit of drop effect . 0.5 will make it spawn exactly on ground . 0.6 bit above the ground
                        GameObject temp = GameObject.Instantiate(m_GreenDomino);
                        temp.transform.parent = DominosRoot.transform;
                        temp.transform.position = position;
                        temp.transform.right = GameCamera.transform.right;
                    }
                    else if (m_Mode == Mode.Spawn_Red_Dominos)
                    {
                        position.y += m_CellSize * 0.6f;
                        GameObject temp = GameObject.Instantiate(m_RedDomino);
                        temp.transform.parent = DominosRoot.transform;
                        temp.transform.position = position;
                        temp.transform.right = GameCamera.transform.right;
                    }
                    else if( m_Mode == Mode.Spawn_Ball )
                    {
                        position.y += m_CellSize * 1.5f ;
                        GameObject ballObject = GameObject.Instantiate(m_Ball);
                        ballObject.transform.position = position;
                        Rigidbody rigidbody = ballObject.GetComponent<Rigidbody>();
                        rigidbody?.AddForce(GameCamera.transform.right * 170, ForceMode.Force);
                    }
                }
            }
        }
    }

    void ProcessButtonPress(object data)
    {
        if(data.ToString() == EventId.KeyPad_1_Pressed )
        {
            m_Mode = Mode.Spawn_Green_Dominos;
        }
        else if( data.ToString() == EventId.KeyPad_2_Pressed)
        {
            m_Mode = Mode.Spawn_Red_Dominos;
        }
        else if (data.ToString() == EventId.KeyPad_3_Pressed)
        {
            m_Mode = Mode.Edit_Environment;
        }
        else if (data.ToString() == EventId.KeyPad_4_Pressed)
        {
            m_Mode = Mode.Spawn_Ball;
        }
        else if( data.ToString() == EventId.Spacebar_Pressed)
        {
            if (m_CameraType == CameraType.FlyBy)
            {
                GameObject ballObject = GameObject.Instantiate(m_Ball);
                ballObject.transform.position = GameCamera.transform.position + 1.5f * GameCamera.transform.forward;
                Rigidbody rigidbody = ballObject.GetComponent<Rigidbody>();
                rigidbody?.AddForce(GameCamera.transform.forward * 150, ForceMode.Force);
            }
        }
    }
}
