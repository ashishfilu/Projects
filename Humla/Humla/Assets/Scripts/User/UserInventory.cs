using System.Collections.Generic;

public class UserInventory
{
    public List<VehicleEntity> Vehicles { get; private set; }

    public UserInventory()
    {
        Vehicles = new List<VehicleEntity>();
    }

    public bool Purchase(VehicleEntity vehicleEntity)
    {
        if (DoesExists(vehicleEntity))
        {
            return false;
        }
        
        List<Currency> cost = vehicleEntity.Data.Cost;
        for (int i = 0; i < cost.Count; i++)
        {
            bool status = CurrencyController.Instance.AddDelta(cost[i].Type, -(int)cost[i].Amount);
            if (status == false)
            {
                return false;
            }
            Vehicles.Add(vehicleEntity);
        }

        return true;
    }

    public bool DoesExists(VehicleEntity vehicle)
    {
        return Vehicles.Contains(vehicle);
    }

    public void AddToInventory(string id)
    {
        VehicleEntity vehicleEntity = VehicleController.Instance.GetVehicle(id);
        if (vehicleEntity != null)
        {
            Vehicles.Add(vehicleEntity);
        }
    }
    
    public void AddToInventory(VehicleEntity vehicleEntity)
    {
        if (vehicleEntity != null)
        {
            Vehicles.Add(vehicleEntity);
        }
    }
}
