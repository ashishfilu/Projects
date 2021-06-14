
using System;
using UnityEngine;
using Object = System.Object;

public class Rocket : Ammunition
{
    public Vector3 RightVector { get; set; }
    
    private float time = 0.0f;

    private void Start()
    {
        GameEventManager.Instance.SubscribeEventListener(Event.PausePressed , OnGamePaused);
    }

    private void OnDestroy()
    {
        GameEventManager.Instance.UnsubscribeEventListener(Event.PausePressed , OnGamePaused);
    }
    
    private void OnEnable()
    {
        time = 0.0f;
    }

    void Update()
    {
        if (!IsVisible())
        {
            gameObject.SetActive(false);
            return;
        }

        if (_gamePaused)
        {
            return;
        }
        
        time += Time.deltaTime * 0.4f;
        Vector3 position = gameObject.transform.position;
        Vector3 direction = Vector3.Lerp(RightVector, -Vector3.up, time);
        position += direction * 400.0f * Time.deltaTime;
        gameObject.transform.position = position;
        gameObject.transform.right = direction;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Terrain"))
        {
            gameObject.SetActive(false);
        }
    }

    private bool IsVisible()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (GeometryUtility.TestPlanesAABB(planes, gameObject.GetComponent<Collider2D>().bounds))
        {
            return true;   
        }
        return false;
    }
    
    private void OnGamePaused(Object data)
    {
        _gamePaused = Boolean.Parse(data.ToString());
    }
}
