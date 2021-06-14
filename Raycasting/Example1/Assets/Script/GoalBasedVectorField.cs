
#define DRAW_VECTOR_FIELD

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum GridType
{
    WOOD,
    ROCK,
}

public sealed class CustomParticle
{
    public GameObject Instance;
    public VectorField VectorField;
    public float Speed;
    public Vector3 Acceleration = Vector3.zero;
    public Vector3 Velocity = Vector3.zero;
}

public sealed class VectorField
{
    public Vector2 TopLeft, TopRight, BottomRight, BottomLeft;
    
    public Vector2 Position { get; private set; }
    public int IndexX { get; private set; }
    public int IndexY{ get; private set; }
    public bool Active{ get; private set; }
    public float Distance{ get; private set; }
    public Vector2 Direction{ get; private set; }
    public bool Visited { get; private set; }
    public GridType GridType { get; set; }

    private GameObject _directionLine,_text;
    private InstanceData _image;
    private GameObject _parent;
    
    
    public VectorField(Vector2 topLeft , Vector2 topRight , Vector2 bottomRight , Vector2 bottomLeft,
                        int indexX , int indexY,GameObject lineResource,
                        GameObject textResource , GameObject parent)
    {
        TopLeft = topLeft;
        TopRight = topRight;
        BottomLeft = bottomLeft;
        BottomRight = bottomRight;

        IndexX = indexX;
        IndexY = indexY;
        
        Active = true;
        Distance = 0.0f;
        Direction = Vector2.zero;
        _parent = parent;
        
        Position = new Vector2(Mathf.Lerp(topLeft.x, topRight.x, 0.5f),Mathf.Lerp(topLeft.y, bottomLeft.y, 0.5f));
        GridType = GridType.WOOD;
        _image = ObjectPool.Instance.GetObject("Prefabs/Grass");
        
#if DRAW_VECTOR_FIELD 
        _directionLine = GameObject.Instantiate(lineResource);
        _text = GameObject.Instantiate(textResource);
        _image.Instance.transform.parent = parent.transform; 
        _directionLine.transform.parent = parent.transform;
        
        _text.transform.parent = parent.transform;
        
        _text.transform.localPosition = new Vector3(Position.x,Position.y+10,1);
        _directionLine.transform.localPosition = Position;
        _directionLine.transform.localScale = new Vector3(20,1,1);
        _image.Instance.transform.localPosition = Position;
        _image.Instance.transform.localScale = new Vector3(topRight.x - topLeft.x,topLeft.y-bottomLeft.y,1);
#endif
    }

    public void SetActive(bool active)
    {
        Active = active;
        _text.SetActive(active);
        _directionLine.SetActive(active);
        GridType = active?GridType.WOOD:GridType.ROCK;
#if DRAW_VECTOR_FIELD
        if (_image != null)
        {
            if (!active)
            {
                ObjectPool.Instance.ReturnToPool(_image);
                _image = ObjectPool.Instance.GetObject("Prefabs/Rock");
                _image.Instance.transform.parent = _parent.transform;
                _image.Instance.transform.localPosition = Position;
                _image.Instance.transform.localScale = new Vector3(TopRight.x - TopLeft.x,TopLeft.y-BottomLeft.y,1);
            }
        }
#endif
       
    }

    public void SetDistance(float distance)
    {
        Distance = distance;
        _text.gameObject.GetComponent<Text>().text = $"{distance}";
        
    }

    public void SetVisited(bool visited)
    {
        Visited = visited;
    }

    public void SetDirection(Vector2 direction)
    {
        Direction = direction.normalized;
        float angle = Mathf.Acos(Vector2.Dot(Direction, Vector2.right)) * Mathf.Rad2Deg ;
        Vector3 crossProduct = Vector3.Cross(Vector2.right, direction);
        if (crossProduct.z < 0)
        {
            angle *= -1.0f;
        }
        _directionLine.transform.localRotation = Quaternion.Euler(0,0,angle);
    }

    public void ActivateDirectionPointer(bool activate)
    {
        _directionLine.SetActive(activate);
    }
}
    

public class GoalBasedVectorField : MonoBehaviour
{
    private VectorField[,] _vectorFields;

    public float _gridWidth;
    public float _gridHeight;
    public RectTransform _backgroundTransform;
    public GameObject _debugLineRoot;

    private GameObject _lineResource;
    private GameObject _textResource;
    private int _numberOfGridsInXDirection, _numberOfGridsInYDirection;
    private List<CustomParticle> _particles;
    private bool _updateParticles = false;
    private VectorField _goalGrid = null;
    
    // Start is called before the first frame update
    void Start()
    {
        _lineResource = Resources.Load("Prefabs/GridLine")as GameObject;
        _textResource = Resources.Load("Prefabs/Text")as GameObject;
        _particles = new List<CustomParticle>();
        
        _numberOfGridsInXDirection = Mathf.CeilToInt(_backgroundTransform.rect.width / _gridWidth);
        _numberOfGridsInYDirection = Mathf.CeilToInt(_backgroundTransform.rect.height / _gridHeight);
        InitializeVectorFields();
        SimulateObstacle();
        InitializeParticles();
    }

    private void InitializeVectorFields()
    {
        _vectorFields = new VectorField[_numberOfGridsInXDirection,_numberOfGridsInYDirection];
        Vector2 startPoint = new Vector2(-0.5f * (_backgroundTransform.rect.width - _gridWidth ), 0.5f * (_backgroundTransform.rect.height - _gridHeight));

        Vector2 topLeft, topRight, bottomRight, bottomLeft;
        for (int i = 0; i < _numberOfGridsInXDirection; i++)
        {
            for (int j = 0; j < _numberOfGridsInYDirection; j++)
            {
                topLeft = new Vector2(startPoint.x-_gridWidth*0.5f,startPoint.y+_gridWidth*0.5f);
                topRight = new Vector2(startPoint.x+_gridWidth*0.5f,startPoint.y+_gridWidth*0.5f);
                bottomRight = new Vector2(startPoint.x+_gridWidth*0.5f,startPoint.y-_gridWidth*0.5f);
                bottomLeft = new Vector2(startPoint.x-_gridWidth*0.5f,startPoint.y-_gridWidth*0.5f);
                
                VectorField temp = new VectorField(topLeft,topRight,bottomRight,bottomLeft,i,j,
                                                    _lineResource,_textResource , _debugLineRoot);
                _vectorFields[i,j] = temp;
                startPoint.y -= _gridHeight;
            }

            startPoint.x += _gridWidth;
            startPoint.y = 0.5f * (_backgroundTransform.rect.height - _gridHeight);
        }
    }

    private void InitializeParticles()
    {
        GameObject resource = Resources.Load("Prefabs/Bubble")as GameObject;
        GameObject parent = GameObject.Find("Panel");
        for (int i = 0; i < 1000; i++)
        {
            VectorField temp = _vectorFields[Random.Range(0,3),Random.Range(0,_numberOfGridsInYDirection)];
            if (!temp.Active)
            {
                continue;
            }
            GameObject bubble = Instantiate(resource);
            bubble.transform.parent = parent.transform;
            Vector3 position = temp.Position;
            position.x += Random.Range(-_gridWidth * 0.4f, _gridWidth * 0.4f);
            position.y += Random.Range(-_gridHeight * 0.4f, _gridHeight * 0.4f);
            bubble.transform.localPosition = position;
            
            CustomParticle particle = new CustomParticle();
            particle.Instance = bubble;
            UpdateVectorFieldForParticle(particle);
            particle.Speed = Random.Range(1, 5);
            particle.Acceleration = Vector3.zero;
            particle.Velocity = Vector3.zero;
            _particles.Add(particle);
        }
    }
    
    private void SimulateObstacle()
    {
        _vectorFields[3,0].SetActive(false);
        _vectorFields[3,1].SetActive(false);
        _vectorFields[3,2].SetActive(false);
        _vectorFields[3,3].SetActive(false);
        _vectorFields[3,4].SetActive(false);
        
        _vectorFields[6,11].SetActive(false);
        _vectorFields[6,10].SetActive(false);
        _vectorFields[6,9].SetActive(false);
        _vectorFields[6,8].SetActive(false);
        _vectorFields[6,7].SetActive(false);
        _vectorFields[7,7].SetActive(false);
        _vectorFields[8,7].SetActive(false);
        _vectorFields[9,7].SetActive(false);
        _vectorFields[9,10].SetActive(false);
        _vectorFields[9,11].SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            mousePosition.x -= Screen.width * 0.5f;
            mousePosition.y -= Screen.height * 0.5f;

            if (_backgroundTransform.rect.left < mousePosition.x && mousePosition.x < _backgroundTransform.rect.right
                && _backgroundTransform.rect.top < mousePosition.y && mousePosition.y < _backgroundTransform.rect.bottom)
            {
                //Reset all previous calculation
                foreach (var grid in _vectorFields)
                {
                    if (!grid.Active)
                    {
                        continue;
                    }
                    grid.SetVisited(false);
                    grid.SetDistance(0);
                    grid.ActivateDirectionPointer(true);
                }
                //Calculate grid index
                float ratioX = mousePosition.x / _backgroundTransform.rect.width + 0.5f;
                int gridX = (int)Mathf.Lerp(0, _numberOfGridsInXDirection, ratioX);
                float ratioY = mousePosition.y / _backgroundTransform.rect.height + 0.5f ;
                int gridY = (int)Mathf.Lerp(_numberOfGridsInYDirection,0 , ratioY);

                if (_vectorFields[gridX, gridY].Active)
                {
                    UpdateVectorFields(_vectorFields[gridX, gridY]);   
                    
                    //Calculate vector direction
                    List<VectorField> neighbours = new List<VectorField>();
                    foreach (var grid in _vectorFields)
                    {
                        if (grid.IndexX > 0)
                        {
                            if (_vectorFields[grid.IndexX - 1, grid.IndexY].Active)
                            {
                                neighbours.Add(_vectorFields[grid.IndexX - 1, grid.IndexY]);
                            }
                        }
                        if (grid.IndexX < _numberOfGridsInXDirection-1)
                        {
                            if (_vectorFields[grid.IndexX + 1, grid.IndexY].Active)
                            {
                                neighbours.Add(_vectorFields[grid.IndexX + 1, grid.IndexY]);
                            }
                        }
                        if (grid.IndexY > 0)
                        {
                            if (_vectorFields[grid.IndexX, grid.IndexY - 1].Active)
                            {
                                neighbours.Add(_vectorFields[grid.IndexX , grid.IndexY-1]);   
                            }
                        }
                        if (grid.IndexY < _numberOfGridsInYDirection-1)
                        {
                            if (_vectorFields[grid.IndexX, grid.IndexY + 1].Active)
                            {
                                neighbours.Add(_vectorFields[grid.IndexX , grid.IndexY+1]);   
                            }
                        }
                        /*
                        if (grid.IndexX > 0 && grid.IndexY > 0)
                        {
                            if (_vectorFields[grid.IndexX - 1, grid.IndexY - 1].Active)
                            {
                                neighbours.Add(_vectorFields[grid.IndexX - 1, grid.IndexY - 1]);
                            }
                        }
                        if (grid.IndexX < _numberOfGridsInXDirection-1 && grid.IndexY > 0)
                        {
                            if (_vectorFields[grid.IndexX + 1, grid.IndexY - 1].Active)
                            {
                                neighbours.Add(_vectorFields[grid.IndexX + 1, grid.IndexY - 1]);
                            }
                        }
                        
                        if (grid.IndexX > 0 && grid.IndexY < _numberOfGridsInYDirection-1)
                        {
                            if (_vectorFields[grid.IndexX - 1, grid.IndexY + 1].Active)
                            {
                                neighbours.Add(_vectorFields[grid.IndexX - 1, grid.IndexY + 1]);
                            }
                        }
                        
                        if (grid.IndexX < _numberOfGridsInXDirection-1 && grid.IndexY < _numberOfGridsInYDirection-1)
                        {
                            if (_vectorFields[grid.IndexX + 1, grid.IndexY + 1].Active)
                            {
                                neighbours.Add(_vectorFields[grid.IndexX + 1, grid.IndexY + 1]);
                            }
                        }
                        */
                        VectorField smallestDistanceNode = null;
                        for (int i = 0; i < neighbours.Count; i++)
                        {
                            if (smallestDistanceNode == null)
                            {
                                smallestDistanceNode = neighbours[i];
                            }
                            else
                            {
                                if (smallestDistanceNode.Distance > neighbours[i].Distance)
                                {
                                    smallestDistanceNode = neighbours[i];
                                }
                            }
                        }

                        if (smallestDistanceNode != null)
                        {
                            grid.SetDirection(new Vector3(smallestDistanceNode.IndexX - grid.IndexX , 
                                grid.IndexY-smallestDistanceNode.IndexY));
                        }
                        neighbours.Clear();
                    }

                    _updateParticles = true;
                }
            }
        }

        if (_updateParticles)
        {
            UpdateParticles();
        }
    }

    private void UpdateVectorFields(VectorField goalNode)
    {
        _goalGrid = goalNode;
        _goalGrid.SetDistance(0);
        _goalGrid.SetVisited(true);
        _goalGrid.ActivateDirectionPointer(false);
        Queue<VectorField> queue = new Queue<VectorField>();
        queue.Enqueue(_goalGrid);

        while (queue.Count > 0 )
        {
            VectorField currentNode = queue.Dequeue();
            VectorField adjacentNode;
            if (currentNode.IndexX > 0)
            {
                adjacentNode = _vectorFields[currentNode.IndexX - 1, currentNode.IndexY];
                if (!adjacentNode.Visited && adjacentNode.Active)
                {
                    adjacentNode.SetDistance(currentNode.Distance + 1);
                    adjacentNode.SetVisited(true);
                    queue.Enqueue(adjacentNode);
                }
            }
            if (currentNode.IndexX < _numberOfGridsInXDirection-1)
            {
                adjacentNode = _vectorFields[currentNode.IndexX + 1, currentNode.IndexY];
                if (!adjacentNode.Visited && adjacentNode.Active )
                {
                    adjacentNode.SetDistance(currentNode.Distance + 1);
                    adjacentNode.SetVisited(true);
                    queue.Enqueue(adjacentNode);
                }
            }
            if (currentNode.IndexY > 0)
            {
                adjacentNode = _vectorFields[currentNode.IndexX , currentNode.IndexY-1];
                if (!adjacentNode.Visited && adjacentNode.Active)
                {
                    adjacentNode.SetDistance(currentNode.Distance + 1);
                    adjacentNode.SetVisited(true);
                    queue.Enqueue(adjacentNode);
                }
            }
            if (currentNode.IndexY < _numberOfGridsInYDirection-1)
            {
                adjacentNode = _vectorFields[currentNode.IndexX , currentNode.IndexY+1];
                if (!adjacentNode.Visited && adjacentNode.Active)
                {
                    adjacentNode.SetDistance(currentNode.Distance + 1);
                    adjacentNode.SetVisited(true);
                    queue.Enqueue(adjacentNode);
                }
            }
            /*
            if (currentNode.IndexX > 0 && currentNode.IndexY > 0)
            {
                adjacentNode = _vectorFields[currentNode.IndexX-1 , currentNode.IndexY-1];
                if (!adjacentNode.Visited && adjacentNode.Active)
                {
                    adjacentNode.SetDistance(currentNode.Distance + 1);
                    adjacentNode.SetVisited(true);
                    queue.Enqueue(adjacentNode);
                }
            }
            
            if (currentNode.IndexX < _numberOfGridsInXDirection-1 && currentNode.IndexY > 0 )
            {
                adjacentNode = _vectorFields[currentNode.IndexX + 1, currentNode.IndexY - 1];
                if (!adjacentNode.Visited && adjacentNode.Active )
                {
                    adjacentNode.SetDistance(currentNode.Distance + 1);
                    adjacentNode.SetVisited(true);
                    queue.Enqueue(adjacentNode);
                }
            }
            
            if (currentNode.IndexX > 0 && currentNode.IndexY < _numberOfGridsInYDirection-1)
            {
                adjacentNode = _vectorFields[currentNode.IndexX-1 , currentNode.IndexY+1];
                if (!adjacentNode.Visited && adjacentNode.Active)
                {
                    adjacentNode.SetDistance(currentNode.Distance + 1);
                    adjacentNode.SetVisited(true);
                    queue.Enqueue(adjacentNode);
                }
            }
            
            if (currentNode.IndexX < _numberOfGridsInXDirection-1 && currentNode.IndexY < _numberOfGridsInYDirection-1)
            {
                adjacentNode = _vectorFields[currentNode.IndexX+1 , currentNode.IndexY+1];
                if (!adjacentNode.Visited && adjacentNode.Active)
                {
                    adjacentNode.SetDistance(currentNode.Distance + 1);
                    adjacentNode.SetVisited(true);
                    queue.Enqueue(adjacentNode);
                }
            }
            */
        }
    }

    //Larger the offset , smaller is the rect to be checked
    void UpdateVectorFieldForParticle(CustomParticle particle)
    {
        Vector3 position = particle.Instance.transform.localPosition;
        foreach (var grid in _vectorFields)
        {
            if (grid.Active == false)
            {
                continue;
            }
            if( grid.TopLeft.x < position.x && position.x < grid.TopRight.x 
                && grid.TopLeft.y > position.y && position.y > grid.BottomRight.y )
            {
                particle.VectorField = grid;
                break;
            }
        }
    }

    private void UpdateParticles()
    {
        int shouldUpdateParticles = 0;
        for (int i = 0; i < _particles.Count; i++)
        {
            Vector3 position = _particles[i].Instance.transform.localPosition;
            UpdateVectorFieldForParticle(_particles[i]);
            
            if (_particles[i].VectorField != null)
            {
                CustomParticle particle = _particles[i];
                Vector3 desiredVelocity = new Vector3(particle.VectorField.Direction.x, particle.VectorField.Direction.y, 0) * particle.Speed ;
                Vector3 steeringForce = (desiredVelocity - particle.Velocity);
                steeringForce = Vector3.ClampMagnitude(steeringForce, 0.03f);//Higher the max force, quicker will be the turn
                particle.Acceleration += steeringForce;
                particle.Velocity += particle.Acceleration;
                particle.Velocity = Vector3.ClampMagnitude(particle.Velocity, 1.0f);//Higher the value , greater is curve
                position += particle.Velocity;
                particle.Instance.transform.localPosition = position;
                particle.Acceleration = Vector3.zero;

                if( _goalGrid.TopLeft.x  < position.x && position.x < _goalGrid.TopRight.x 
                    && _goalGrid.TopLeft.y  > position.y && position.y > _goalGrid.BottomRight.y )
                {
                    shouldUpdateParticles |= 0;
                }
                else
                {
                    shouldUpdateParticles |= 1;
                }
            }
        }

        _updateParticles = shouldUpdateParticles == 1;
    }
}
