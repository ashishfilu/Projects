using UnityEngine;
using Object = System.Object;

public class Player
{
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    public TankScript Vehicle { get; private set; }

    public Player()
    {
        GameEventManager.Instance.SubscribeEventListener(Event.OnHitByRocket , OnHitByProjectile);
        GameEventManager.Instance.SubscribeEventListener(Event.OnHitByBullet , OnHitByProjectile);
        GameEventManager.Instance.SubscribeEventListener(Event.OnHItByAI , OnHitByProjectile);
        GameEventManager.Instance.SubscribeEventListener(Event.OnStarCollected , OnStarCollected);
    }

    public void Destroy()
    {
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnHitByRocket , OnHitByProjectile);
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnHitByBullet , OnHitByProjectile);
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnHItByAI , OnHitByProjectile);
        GameEventManager.Instance.UnsubscribeEventListener(Event.OnStarCollected , OnStarCollected);
    }
    
    public void Initialize(VehicleEntity selectedVehicle)
    {
        VehicleData vehicleData = selectedVehicle.Data;
        
        GameObject playerReferencePoint = GameObject.FindWithTag("PlayerReferencePosition");
        GameObject vehicleObject = GameObject.Instantiate(Resources.Load(vehicleData.PrefabPath) as GameObject);
        vehicleObject.transform.parent = playerReferencePoint.transform;
        vehicleObject.transform.localPosition = Vector3.zero;
        
        if (vehicleObject != null)
        {
            Vehicle = vehicleObject.GetComponent<TankScript>();
            Vehicle.InitWithData(vehicleData);
            CurrentHealth = MaxHealth = Vehicle.Data.MaxHealth;
        }
        
        GameEventManager.Instance.TriggerEvent(Event.OnPlayerInitialized,string.Empty);
    }

    private void OnHitByProjectile(Object data)
    {
        if (CurrentHealth < 0)
        {
            return;
        }
        
        int damage = int.Parse(data.ToString());
        CurrentHealth -= damage;

        GameEventManager.Instance.TriggerEvent(Event.OnPlayerHealthUpdated , (CurrentHealth/MaxHealth).ToString());
        if (CurrentHealth <= 0)
        {
            GameEventManager.Instance.TriggerEvent(Event.PausePressed,"true");
            GameEventManager.Instance.TriggerEvent(Event.OnPlayerDead , null);
        }
    }

    private void OnStarCollected(Object data)
    {
        CurrentHealth += 100;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        GameEventManager.Instance.TriggerEvent(Event.OnPlayerHealthUpdated , (CurrentHealth/MaxHealth).ToString());
    }
}

public partial class Event
{
    public static string OnPlayerInitialized = "OnInitialized";
    public static string OnPlayerDead = "OnDead";
    public static string OnPlayerHealthUpdated = "OnHealthUpdated";
}

