

using System.Collections.Generic;

public delegate void OnVehicleSelectedDelegate(VehicleEntity vehicle);

public class VehicleController : Singleton<VehicleController>
{
    public OnVehicleSelectedDelegate OnVehicleSelected;
    public VehicleEntity SelectedVehicle { get; private set; }

    private VehicleDataManager _dataManager;
    private VehicleEntityManager _entityManager;

    public VehicleController()
    {
        _dataManager = new VehicleDataManager();
        _entityManager = new VehicleEntityManager();

        _dataManager.OnDataUpdated += _entityManager.OnDataUpdated;
        _entityManager.OnEntitiesUpdate += OnEntitiesUpdate;
        
        SelectedVehicle = null;
    }

    private void OnEntitiesUpdate(IReadOnlyList<VehicleEntity> runtimeentities)
    {
        
    }

    ~VehicleController()
    {
        _dataManager = null;
        SelectedVehicle = null;
        _dataManager.OnDataUpdated -= _entityManager.OnDataUpdated;
        _entityManager.OnEntitiesUpdate -= OnEntitiesUpdate;
    }
    
    public void InitializeData()
    {
        _dataManager.Initialize();
    }

    public IReadOnlyList<VehicleEntity> GetVehicles(VehicleType type)
    {
        List<VehicleEntity> output = new List<VehicleEntity>();
        for (int i = 0; i < _entityManager.AllVehicles.Count; i++)
        {
            if (_entityManager.AllVehicles[i].Data.Type == type)
            {
                output.Add(_entityManager.AllVehicles[i]);
            }
        }

        return output;
    }
    
    public void SetSelectedVehicle(int index)
    {
        SelectedVehicle = index >= 0 && index < _entityManager.AllVehicles.Count
            ? _entityManager.AllVehicles[index]
            : null;
        OnVehicleSelected?.Invoke(SelectedVehicle);
    }

    public void SetSelectedVehicle(string id)
    {
        SelectedVehicle = GetVehicle(id);
        OnVehicleSelected?.Invoke(SelectedVehicle);
    }

    public VehicleEntity GetVehicle(string id)
    {
        for (int i = 0; i < _entityManager.AllVehicles.Count; i++)
        {
            if (_entityManager.AllVehicles[i].Data.Id == id)
            {
                return _entityManager.AllVehicles[i];
            }
        }

        return null;
    }
}
