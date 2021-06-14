using System;
using System.Collections.Generic;

public delegate void EventAction(Object data);

public class GameEvent
{
    public string EventKey;
    public List<EventAction> ActionQueue;

    public GameEvent(string eventKey , EventAction action)
    {
        EventKey = eventKey;
        ActionQueue = new List<EventAction>();
        ActionQueue.Add(action);
    }
}

public class GameEventManager : Singleton<GameEventManager>
{    
    private Dictionary<string, GameEvent> _allEvents;

    public GameEventManager()
    {
        _allEvents = new Dictionary<string, GameEvent>();
    }

    public void SubscribeEventListener(string type, EventAction action)
    {
        if (_allEvents.ContainsKey(type) == false)
        {
            _allEvents.Add(type,new GameEvent(type,action));
        }
        else
        {
            _allEvents[type].ActionQueue.Add(action);
        }
    }

    public void UnsubscribeEventListener(string type, EventAction action)
    {
        if (_allEvents.ContainsKey(type))
        {
            _allEvents[type].ActionQueue.Remove(action);
        }
    }

    public void TriggerEvent(string type,Object data = null)
    {
        if (_allEvents.ContainsKey(type))
        {
            for (int i = 0; i < _allEvents[type].ActionQueue.Count; i++)
            {
                _allEvents[type].ActionQueue[i]?.Invoke(data);
            }
        }
    }
}
