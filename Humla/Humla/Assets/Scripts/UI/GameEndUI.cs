
public class GameEndUI : PopUp
{
    public CostUI _costUi;
    public StarRootUI _starRoot;

    public void SetCoinData(int amount)
    {
        _costUi.SetData(new Currency(CurrencyType.Coins,amount));
    }

    public void SetRewardPercentage(float percentage)
    {
        _starRoot.SetPercentage(percentage);
    }
}
