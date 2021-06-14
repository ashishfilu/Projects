
using System.Collections.Generic;
using UnityEngine;

 
public class Raycast
{
    private List<GameObject> _allGameObjects;
    private List<Ray> _rays;
    private GameObject _line;
    private GameObject _circle;
    private bool _mouseDrag = false;

    public Raycast()
    {
        _allGameObjects = new List<GameObject>();
        _rays = new List<Ray>();
    }

    public void Initialize()
    {
        _circle = GameObject.Find("Circle");
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Line");
        for (int i = 0; i < allObjects.Length; i++)
        {
            _allGameObjects.Add(allObjects[i]);
        }

        _line = Resources.Load<GameObject>("Prefabs/Line");

        for (int i = 0; i < 360; i = i+3)
        {
            Ray ray = new Ray(_line,_circle,new Vector3(0, 1, 0),i);
            ray.Update();
            _rays.Add(ray);
        }
    }

    public void Update()
    { 
        if (Input.GetMouseButtonDown(0))
        {
            _mouseDrag = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            _mouseDrag = false;
        }

        if (_mouseDrag)
        {
            UpdatePosition();
        }
        
        bool doesIntersect;
        Vector3 hitPoint;
        Vector3 startPoint;

        for (int j = 0; j < _rays.Count; j++)
        {
            Ray ray = _rays[j];
            startPoint = ray.GetWorldPosition();
            hitPoint = Vector3.positiveInfinity;
            
            for (int i = 0; i < _allGameObjects.Count; i++)
            {
                doesIntersect = false;
                Vector3 newHitPoint;
                GameObject gameObject = _allGameObjects[i];
                RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
                Vector3[] cornerPoints = new Vector3[4];
                rectTransform.GetWorldCorners(cornerPoints);
            
                Vector3 p1, p2;
                p1 = cornerPoints[0];
                p2 = cornerPoints[3];
            
                MathHelper.CheckLineIntersection2D(startPoint , startPoint+ray.Direction , p1 , p2 , 
                    out doesIntersect , out newHitPoint);

                if (doesIntersect && (startPoint-newHitPoint).sqrMagnitude < (startPoint-hitPoint).sqrMagnitude)
                {
                    hitPoint = newHitPoint;
                    ray.SetHitPoint(newHitPoint);
                }
            } 
        }
    }

    public void UpdatePosition()
    {
        RectTransform rectTransform = _circle.GetComponent<RectTransform>();
        rectTransform.position = Input.mousePosition;
    }
}

