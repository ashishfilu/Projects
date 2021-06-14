using UnityEngine;
using UnityEngine.UI;

public class CostUI : MonoBehaviour
{
    public RawImage _image;
    public Text _amount;

    private Currency _currency;
    private bool _dirty = false;
    
    public void SetData(Currency currencyData)
    {
        _currency = currencyData;
        _dirty = true;
    }

    private void Update()
    {
        if (_dirty)
        {
            switch (_currency.Type)
            {
                case CurrencyType.Coins:
                    _image.texture = Resources.Load<Texture>("Textures/UI/Coin");   
                    break;
                case CurrencyType.Gold:
                    _image.texture = Resources.Load<Texture>("Textures/UI/Goldbar");
                    break;
            }

            _amount.text = _currency.Amount.ToString();
            _dirty = false;
        }
    }
}
