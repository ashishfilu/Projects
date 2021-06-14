using System;
using UnityEngine;
using Object = System.Object;

public class Bullet : Ammunition
{
    private void Start()
    {
        GameEventManager.Instance.SubscribeEventListener(Event.PausePressed , OnGamePaused);
    }

    private void OnDestroy()
    {
        GameEventManager.Instance.UnsubscribeEventListener(Event.PausePressed , OnGamePaused);
    }

    void Update()
    {
        if (!IsVisible())
        {
            gameObject.SetActive(false);
        }

        if (_gamePaused)
        {
            return;
        }

        Vector3 position = gameObject.transform.position;
        position += gameObject.transform.right * 500.0f * Time.deltaTime;
        gameObject.transform.position = position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        gameObject.SetActive(false);
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
