
public class DataManager : Singleton<DataManager>
{
    public void LoadAllDataManagers()
    {
        MissionController.Instance.InitializeData();
        VehicleController.Instance.InitializeData();
        ParticleDataManager.Instance.Initialize();
    }
}
