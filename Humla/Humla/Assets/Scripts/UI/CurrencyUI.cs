using UnityEngine;
using UnityEngine.UI;

public class CurrencyUI : MonoBehaviour
{
   public RawImage _image;
   public Text _amount;
   public CurrencyType _type;

   private void Start()
   {
      CurrencyController.Instance.OnCurrencyUpdated += OnCurrencyUpdated;
      UpdateCurrency();
      switch (_type)
      {
         case CurrencyType.Coins:
            _image.texture = Resources.Load<Texture>("Textures/UI/Coin");   
            break;
         case CurrencyType.Gold:
            _image.texture = Resources.Load<Texture>("Textures/UI/Goldbar");
            break;
      }
   }

   private void OnDestroy()
   {
      CurrencyController.Instance.OnCurrencyUpdated -= OnCurrencyUpdated;
   }

   private void OnCurrencyUpdated(CurrencyType type, int delta, long amount)
   {
      UpdateCurrency();
   }

   private void UpdateCurrency()
   {
      Currency currency = CurrencyController.Instance.GetCurrency(_type);
      if (currency != null)
      {
         _amount.text = currency.Amount.ToString();
      }
   }
}
