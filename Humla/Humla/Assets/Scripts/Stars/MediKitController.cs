using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class MediKitController : MonoBehaviour
{
    public float _amplitude;
    public float _speedAlongYAxis,_speedAlongXAxis;
    public List<GameObject> _mediKits;
    
    private float _animationTime,_delay;
    private Vector3 _leftMargin;
    private int _index;
    private int _starsUpdated;
    private bool _gamePaused = false;

    private void Start()
    {
        _animationTime = 0.0f;
        _delay = 0.0f;
        _index = 0;
        _starsUpdated = 0;
        
        for (int i = 1; i < _mediKits.Count; i++)
        {
            _mediKits[i].SetActive(false);
        }  
        TerrainLayoutController terrainsLayoutController = FindObjectOfType<TerrainLayoutController>();
        _leftMargin = terrainsLayoutController.GetStartPoint();
        
        GameEventManager.Instance.SubscribeEventListener(Event.OnStarCollected ,OnStarCollected );
        GameEventManager.Instance.SubscribeEventListener(Event.PausePressed , OnGamePaused);
    }

    private void OnDestroy()
    {
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnStarCollected ,OnStarCollected );
        GameEventManager.Instance.UnsubscribeEventListener(Event.PausePressed , OnGamePaused);
    }

    void Update()
    {
        if (_starsUpdated >= _mediKits.Count || _gamePaused)
        {
            return;
        }
        
        _delay += Time.deltaTime;
        //Activate a star after 10 seconds
        if (_delay >= 10.0f)
        {
            _index++;
            if (_index > _mediKits.Count - 1)
            {
                _index = _mediKits.Count - 1;
            }
            _mediKits[_index].SetActive(true);
            _delay = 0.0f;
            _speedAlongXAxis += 20.0f;
        }
        
        //Animate star motion
        _animationTime += Time.deltaTime * _speedAlongYAxis;
        
        for (int i = 0; i <= _index; i++)
        {   
            if (!_mediKits[i].activeSelf)
            {
                continue;
            }

            float height = _amplitude * Mathf.Sin(_animationTime);
            Vector3 position = _mediKits[i].transform.position;
            position.y = height;
            position.x -= Time.deltaTime * _speedAlongXAxis;
            _mediKits[i].transform.position = position;
            
            if (position.x < _leftMargin.x)
            {
                _mediKits[i].SetActive(false);
                _starsUpdated++;
            }
        }
    }

    private void OnStarCollected(Object data)
    {
        _starsUpdated++;
    }

    private void OnGamePaused(Object data)
    {
        _gamePaused = Boolean.Parse(data.ToString());
    }
}
