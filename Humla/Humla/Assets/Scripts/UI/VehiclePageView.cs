using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehiclePageView : PageView
{
    public Button _buyButton;
    public Button _selectButton;
    public Text _selectButtonText;
    
    private GameObject _childObjectResource;

    void Start()
    {
        _childObjectResource = Resources.Load<GameObject>("Prefabs/UI/VehicleData");
        Populate();
        InitializePageView();
        
        _buyButton.onClick.AddListener(OnBuyButtonPressed);
        _selectButton.onClick.AddListener(OnSelectButtonPressed);
    }
    
    private void Populate()
    {
        IReadOnlyList<VehicleEntity> vehicles = VehicleController.Instance.GetVehicles(VehicleType.Tank);
        
        for (int i = 0; i < vehicles.Count; i++)
        {
            GameObject vehicleViewObject = Instantiate(_childObjectResource);
            VehicleView script = vehicleViewObject.GetComponent<VehicleView>();
            script.SetData(vehicles[i].Data);

            vehicleViewObject.transform.parent = _viewRoot.transform;
            vehicleViewObject.transform.localScale = Vector3.one;
        }
    }

    private void Update()
    {
        VehicleEntity vehicle = VehicleController.Instance.GetVehicles(VehicleType.Tank)[CurrentIndex];
        if (User.Instance.DoesInventoryExits(vehicle))
        {
            _selectButton.enabled = true;
            _selectButtonText.text = "Select";
            _selectButton.gameObject.SetActive(true);
            _buyButton.gameObject.SetActive(false);

            if (vehicle == VehicleController.Instance.SelectedVehicle)
            {
                _selectButtonText.text = "Equipped";
                _selectButton.enabled = false;
            }
        }
        else
        {
            _selectButton.gameObject.SetActive(false);
            _buyButton.gameObject.SetActive(true);
        }
    }
    
    private void OnBuyButtonPressed()
    {
        VehicleEntity vehicle = VehicleController.Instance.GetVehicles(VehicleType.Tank)[CurrentIndex];

        List<Currency> cost = vehicle.Data.Cost;
        for (int i = 0; i < cost.Count; i++)
        {
            bool success = User.Instance.Purchase(vehicle);
            if (success)
            {
                User.Instance.Save();
            }
        }
    }

    private void OnSelectButtonPressed()
    {
        VehicleController.Instance.SetSelectedVehicle(CurrentIndex);
    }
}
