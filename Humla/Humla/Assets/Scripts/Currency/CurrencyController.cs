using System.Collections.Generic;

public delegate void OnCurrencyUpdatedDelegate(CurrencyType type, int delta, long amount);

public class CurrencyController : Singleton<CurrencyController>
{
    private List<Currency> _allCurrencies;

    public OnCurrencyUpdatedDelegate OnCurrencyUpdated;

    public CurrencyController()
    {
        _allCurrencies = new List<Currency>();
        for (int i = 0; i < (int) CurrencyType.Max; i++)
        {
            _allCurrencies.Add(new Currency((CurrencyType)i,0));
        }
    }

    public void SetAmount(CurrencyType type, long amount)
    {
        Currency currency = GetCurrency(type);
        currency.Amount = amount;
        OnCurrencyUpdated?.Invoke(type,0,amount);
    }
    
    public bool AddDelta(CurrencyType type, int delta)
    {
        Currency currency = GetCurrency(type);
        if (delta < 0 && currency.Amount < delta )
        {
            return false;
        }
        currency.AddDelta(delta);
        OnCurrencyUpdated?.Invoke(type,0,currency.Amount);
        return true;
    }

    public Currency GetCurrency(CurrencyType type)
    {
        for (int i = 0; i < _allCurrencies.Count; i++)
        {
            if (_allCurrencies[i].Type == type)
            {
                return _allCurrencies[i];
            }
        }
        return null;
    }
}
