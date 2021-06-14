using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public class Raycast 
{
    private GameObject[]        m_AllGameObjects;
    private Ray[]               m_Rays;
    private GameObject          m_Line;
    private GameObject          m_Circle;
    private bool                m_MouseDrag = false;
    private int                 m_StepAngle;
    private bool                m_UseJobSystem;

    private NativeArray<Vector3> m_RayStartPositions, m_RayDirections, m_HitPoints;
    private NativeArray<CornerPoints> m_CornerPoints;

    public Raycast(int stepAngle , bool useJob)
    {
        int numberOfRays = 360 / stepAngle;
        m_Rays = new Ray[numberOfRays];
        m_StepAngle = stepAngle;
        m_UseJobSystem = useJob; 
    }

    public void Initialize()
    {
        m_Circle = GameObject.Find("Circle");

        List<GameObject> tempList = new List<GameObject>();
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Line");
        for (int i = 0; i < allObjects.Length; i++)
        {
            tempList.Add(allObjects[i]);
        }
        m_AllGameObjects = tempList.ToArray();

        m_Line = Resources.Load<GameObject>("Prefabs/Line");

        int rayIndex = 0;
        for (int i = 0; i < 360; i = i + m_StepAngle)
        {
            Ray ray = new Ray(m_Line, m_Circle, new Vector3(0, 1, 0), i);
            ray.UpdateRayLength();
            m_Rays[rayIndex] = ray;
            rayIndex++;
        }

        m_RayStartPositions = new NativeArray<Vector3>(m_Rays.Length, Allocator.Persistent);
        m_RayDirections = new NativeArray<Vector3>(m_Rays.Length, Allocator.Persistent);
        m_HitPoints = new NativeArray<Vector3>(m_Rays.Length, Allocator.Persistent);
        m_CornerPoints = new NativeArray<CornerPoints>(m_AllGameObjects.Length, Allocator.Persistent);
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_MouseDrag = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            m_MouseDrag = false;
        }

        if (m_MouseDrag)
        {
            UpdatePosition();
        }

        float starTime = Time.realtimeSinceStartup;

        if (!m_UseJobSystem)
        {
            PerformRayCast();
        }
        else
        {
            PerformRaycastUsingParallelJob();
        }
        Debug.Log($"DeltaTime : {(Time.realtimeSinceStartup - starTime) * 1000f} ms");
    }

    public void UpdatePosition()
    {
        RectTransform rectTransform = m_Circle.GetComponent<RectTransform>();
        rectTransform.position = Input.mousePosition;
    }

    private void PerformRayCast()
    {
        bool doesIntersect;
        Vector3 hitPoint;
        Vector3 startPoint;

        for (int j = 0; j < m_Rays.Length; j++)
        {
            Ray ray = m_Rays[j];
            startPoint = ray.GetWorldPosition();
            hitPoint = Vector3.positiveInfinity;

            for (int i = 0; i < m_AllGameObjects.Length; i++)
            {
                doesIntersect = false;
                Vector3 newHitPoint;
                GameObject gameObject = m_AllGameObjects[i];
                RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
                Vector3[] cornerPoints = new Vector3[4];
                rectTransform.GetWorldCorners(cornerPoints);

                Vector3 p1, p2;
                p1 = cornerPoints[0];
                p2 = cornerPoints[3];

                MathHelper.CheckLineIntersection2D(startPoint, startPoint + ray.Direction, p1, p2,
                    out doesIntersect, out newHitPoint);

                if (doesIntersect && (startPoint - newHitPoint).sqrMagnitude < (startPoint - hitPoint).sqrMagnitude)
                {
                    hitPoint = newHitPoint;
                    ray.SetHitPoint(newHitPoint);
                }
            }
        }
    }

    private void PerformRaycastUsingParallelJob()
    {
        for( int i = 0; i < m_Rays.Length; i++)
        {
            m_RayStartPositions[i] = m_Rays[i].GetWorldPosition();
            m_RayDirections[i] = m_Rays[i].Direction;
            m_HitPoints[i] = Vector3.positiveInfinity;
        }

        for (int j = 0; j < m_AllGameObjects.Length; j++)
        {
            GameObject gameObject = m_AllGameObjects[j];
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

            Vector3[] points = new Vector3[4];
            rectTransform.GetWorldCorners(points);

            CornerPoints temp = new CornerPoints();
            temp.P1 = points[0];
            temp.P2 = points[1];
            temp.P3 = points[2];
            temp.P4 = points[3];
            m_CornerPoints[j] = temp;
        }

        RayCastParallelJob rayCastParallelJob = new RayCastParallelJob
        {
            StartPoint = m_RayStartPositions,
            Direction = m_RayDirections,
            HitPoint = m_HitPoints,
            AllCornerPoints = m_CornerPoints
        };

        JobHandle jobHandle = rayCastParallelJob.Schedule(m_Rays.Length, 10);
        jobHandle.Complete();

        for( int i = 0; i < m_Rays.Length; i++ )
        {
            m_Rays[i].SetHitPoint(m_HitPoints[i]);
        }
    }

    public void DisposeNativeArrays()
    {
        m_RayStartPositions.Dispose();
        m_RayDirections.Dispose();
        m_HitPoints.Dispose();
        m_CornerPoints.Dispose();
    }
}

public struct CornerPoints
{
    public Vector3 P1,P2,P3,P4;
}

[BurstCompile]
public struct RayCastParallelJob:IJobParallelFor
{
    public NativeArray<Vector3> StartPoint;
    public NativeArray<Vector3> Direction;
    public NativeArray<Vector3> HitPoint;
    [ReadOnly]
    public NativeArray<CornerPoints> AllCornerPoints;    

    public void Execute(int index)
    {
        bool doesIntersect;
        for (int i = 0; i < AllCornerPoints.Length; i++)
        {
            doesIntersect = false;
            Vector3 newHitPoint;
           
            Vector3 p1, p2;
            p1 = AllCornerPoints[i].P1;
            p2 = AllCornerPoints[i].P3;

            MathHelper.CheckLineIntersection2D(StartPoint[index], StartPoint[index]+ Direction[index], p1, p2,
                out doesIntersect, out newHitPoint);

            if (doesIntersect && (StartPoint[index] - newHitPoint).sqrMagnitude < (StartPoint[index] - HitPoint[index]).sqrMagnitude)
            {
                HitPoint[index] = newHitPoint;
            }
        }
    }
}
