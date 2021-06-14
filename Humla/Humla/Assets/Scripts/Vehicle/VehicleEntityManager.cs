using System.Collections.Generic;

public class VehicleEntity
{
    public VehicleData Data { get; private set; }
    public bool Locked { get; set; }

    public VehicleEntity(VehicleData data)
    {
        Data = data;
        Locked = true;
    }
}

public delegate void OnVehicleEntitiesUpdatedDelegate(IReadOnlyList<VehicleEntity> runtimeEntities);

public class VehicleEntityManager
{
    public OnVehicleEntitiesUpdatedDelegate OnEntitiesUpdate;
    
    public List<VehicleEntity> AllVehicles { get; private set; }

    public VehicleEntityManager()
    {
        AllVehicles = new List<VehicleEntity>();
    }

    public void OnDataUpdated(IReadOnlyList<VehicleData> data)
    {
        AllVehicles.Clear();
        for (int i = 0; i < data.Count; i++)
        {
            VehicleEntity entity = new VehicleEntity(data[i]);
            AllVehicles.Add(entity);
        }

        AllVehicles[0].Locked = false;
    }
}
