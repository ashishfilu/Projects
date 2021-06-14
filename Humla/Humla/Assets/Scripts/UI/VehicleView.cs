using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleView : MonoBehaviour
{
    public Text _health;
    public Text _speed;
    public Text _linearFireRate;
    public Text _radialFireRate;
    public Text _baseDamage;
    public List<CostUI> _currencyCost;
    
    public RawImage _Image;

    private VehicleData _data = null;
    private bool _isDirty = false;

    public void SetData(VehicleData data)
    {
        _data = data;
        _isDirty = true;
    }

    void Update()
    {
        if (_isDirty && _data != null)
        {
            _health.text = _data.MaxHealth.ToString();
            _speed.text = _data.Speed.ToString();
            FiringData ammoFiringData = _data.GetFiringData(ArmorType.Bullet);
            _linearFireRate.text = ammoFiringData.LinearFiringRate.ToString();
            _radialFireRate.text = ammoFiringData.RadialFiringRate.ToString();
            _baseDamage.text = ammoFiringData.BaseDamageUponImpact.ToString();
            _Image.texture = Resources.Load(_data.IconPath)as Texture;
            
            for (int i = 0; i < _currencyCost.Count; i++)
            {
                _currencyCost[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < _data.Cost.Count; i++)
            {
                if (i < _currencyCost.Count)
                {
                    _currencyCost[i].gameObject.SetActive(true);
                    CostUI script = _currencyCost[i].gameObject.GetComponent<CostUI>();
                    script.SetData(_data.Cost[i]);
                }
            }
            _isDirty = false;
        }
    }
}
