
using UnityEngine;
using System.Collections.Generic;

public class PlayerState
{
    public PlayerBehavior Behavior;
    public Vector3 Source;
    public Vector3 Target;

    public PlayerState( PlayerBehavior behavior , Vector3 source , Vector3 target)
    {
        Behavior = behavior;
        Source = source;
        Target = target;
    }
}

public class Player : MonoBehaviour
{
    
    public Grid GameBoard { get; set; }
    public Vector2 GridIndex { get; set; }

    public float Speed = 1.0f;

    private float mTime;
    private List<PlayerState> m_AllStates;
    private PlayerState m_CurrentState;

    void Start()
    {
        mTime = 0.0f;
        
        m_CurrentState = null;

        gameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
        gameObject.transform.localPosition = GameBoard.GetPositionFromGridIndex((int)GridIndex.x, (int)GridIndex.y);
        gameObject.transform.localScale = Vector3.one * GameBoard.Scale;
    }

    public void InitializePlayerState()
    {
        m_AllStates = new List<PlayerState>();
        m_AllStates.Add(new PlayerState(PlayerBehavior.IDLE, Vector3.zero, Vector3.zero));
    }

    private void Update()
    {
        if( GameBoard == null )
        {
            return;
        }
        if( m_CurrentState == null && m_AllStates.Count > 0 )
        {
            m_CurrentState = m_AllStates[0];
        }

        if( m_CurrentState == null )
        {
            return;
        }

        switch(m_CurrentState.Behavior)
        {
            case PlayerBehavior.IDLE:
                {
                    if (GameBoard.IsIndexInBound((int)GridIndex.x, (int)GridIndex.y))
                    {
                        UpdateStateAsPerSorroundingGrid();
                    }
                    else
                    {
                        m_AllStates.Add(new PlayerState(PlayerBehavior.NONE, Vector3.zero, Vector3.zero));
                    }
                }
                break;
            case PlayerBehavior.MOVE:
                {
                    if( m_CurrentState.Source != m_CurrentState.Target)
                    {
                        mTime += Speed * Time.deltaTime;
                        if( mTime > 1.0f)
                        {
                            mTime = 1.0f;
                        }
                        Vector3 position = Vector3.Lerp(m_CurrentState.Source, m_CurrentState.Target, mTime);
                        gameObject.transform.position = position;
                        if( mTime == 1.0f )
                        {
                            mTime = 0.0f;
                            GridIndex = GameBoard.GetGridIndexFromPosition(position);
                            m_AllStates.RemoveAt(0);
                            m_CurrentState = null;
                            m_AllStates.Add(new PlayerState(PlayerBehavior.IDLE, Vector3.zero, Vector3.zero));
                        }
                    }
                }
                break;
            case PlayerBehavior.ROTATE:
                mTime += Speed * Time.deltaTime;
                if (mTime > 1.0f)
                {
                    mTime = 1.0f;
                }
                gameObject.transform.right = Vector3.Lerp(m_CurrentState.Source, m_CurrentState.Target, mTime);
                if (mTime == 1.0f)
                {
                    mTime = 0.0f;
                    GridIndex = GameBoard.GetGridIndexFromPosition(gameObject.transform.position);
                    m_CurrentState = null;
                    m_AllStates.RemoveAt(0);
                }

                break;
        }
   
    }

    void UpdateStateAsPerSorroundingGrid()
    {
        Square startSquare = GameBoard.AllSquares[(int)GridIndex.x, (int)GridIndex.y];

        Square topSquare = null;
        Square leftSquare = null;
        Square rightSquare = null;
        Square bottomSquare = null;

        AdjacencyInfo temp = startSquare.AdjacenctSquares[1];
        if( GameBoard.IsIndexInBound(temp.Row,temp.Column))
        {
            topSquare = GameBoard.AllSquares[temp.Row, temp.Column];
        }
        temp = startSquare.AdjacenctSquares[3];
        if (GameBoard.IsIndexInBound(temp.Row, temp.Column))
        {
            leftSquare = GameBoard.AllSquares[temp.Row, temp.Column];
        }
        temp = startSquare.AdjacenctSquares[4];
        if (GameBoard.IsIndexInBound(temp.Row, temp.Column))
        {
            rightSquare = GameBoard.AllSquares[temp.Row, temp.Column];
        }
        temp = startSquare.AdjacenctSquares[6];
        if (GameBoard.IsIndexInBound(temp.Row, temp.Column))
        {
            bottomSquare = GameBoard.AllSquares[temp.Row, temp.Column];
        }

        Vector3 resultantVector = gameObject.transform.right;
        EntityType controlType = EntityType.NONE;

        //Check for Rotation state
        if (leftSquare != null)
        {
            controlType = leftSquare.OccupierType;
            if (controlType == EntityType.ROTATE_CW || controlType == EntityType.ROTATE_CCW)
            {
                Vector3 start = gameObject.transform.right;
                Vector3 target = Quaternion.Euler(0, 0, controlType == EntityType.ROTATE_CW ? 90 : -90) * start;
                m_AllStates.Add(new PlayerState(PlayerBehavior.ROTATE, start, target));
                resultantVector = Quaternion.Euler(0, 0, controlType == EntityType.ROTATE_CW ? 90 : -90) * resultantVector;
            }
        }
        if (rightSquare != null)
        {
            controlType = rightSquare.OccupierType;
            if (controlType == EntityType.ROTATE_CW || controlType == EntityType.ROTATE_CCW)
            {
                Vector3 target = Quaternion.Euler(0, 0, controlType == EntityType.ROTATE_CW ? 90 : -90) * resultantVector;
                m_AllStates.Add(new PlayerState(PlayerBehavior.ROTATE, resultantVector, target));
                resultantVector = target;
            }
        }

        if (topSquare != null)
        {
            controlType = topSquare.OccupierType;
            if (controlType == EntityType.ROTATE_CW || controlType == EntityType.ROTATE_CCW)
            {
                Vector3 target = Quaternion.Euler(0, 0, controlType == EntityType.ROTATE_CW ? 90 : -90) * resultantVector;
                m_AllStates.Add(new PlayerState(PlayerBehavior.ROTATE, resultantVector, target));
                resultantVector = target;
            }
        }

        if (bottomSquare != null)
        {
            controlType = bottomSquare.OccupierType;
            if (controlType == EntityType.ROTATE_CW || controlType == EntityType.ROTATE_CCW)
            {
                Vector3 target = Quaternion.Euler(0, 0, controlType == EntityType.ROTATE_CW ? 90 : -90) * resultantVector;
                m_AllStates.Add(new PlayerState(PlayerBehavior.ROTATE, resultantVector, target));
                resultantVector = target;
            }
        }
        
        //Check for Move state 
        {
            Square nextSquare = null;
            if ( resultantVector == Vector3.right)
            {
                nextSquare = rightSquare;
            }
            else if( resultantVector == Vector3.right * (-1.0f))
            {
                nextSquare = leftSquare;
            }
            else if( resultantVector == Vector3.up)
            {
                nextSquare = topSquare;
            }
            else if( resultantVector == Vector3.up * (-1.0f))
            {
                nextSquare = bottomSquare;
            }

            if( nextSquare != null && nextSquare.OccupierType == EntityType.NONE && nextSquare.Type == SquareType.Floor)
            {
                Vector3 start = gameObject.transform.position;
                Vector3 target = GameBoard.GetPositionFromGridIndex(nextSquare.RowIndex, nextSquare.ColumnIndex);
                m_AllStates.Add(new PlayerState(PlayerBehavior.MOVE, start, target));
            }
            else
            {
                m_AllStates.Add(new PlayerState(PlayerBehavior.NONE, Vector3.zero, Vector3.zero));
            }
        }

        m_AllStates.RemoveAt(0);
        m_CurrentState = m_AllStates[0];
    }
}

