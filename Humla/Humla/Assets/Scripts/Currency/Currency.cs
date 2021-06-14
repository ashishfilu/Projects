
public enum CurrencyType
{
    Coins=0,
    Gold,
    Max,
}

public class Currency
{
    public CurrencyType Type { get; private set; }
    public long Amount { get; set; }

    public Currency(CurrencyType type, long amount)
    {
        Type = type;
        Amount = amount;
    }

    public void AddDelta(int delta)
    {
        Amount += delta;
    }
}
