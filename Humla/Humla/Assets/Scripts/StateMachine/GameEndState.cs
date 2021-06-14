using UnityEngine;

public class GameEndState : IState
{
    private GameEndUI _popUpScript;
    private AppStateType _type;
    private int _rewardCoins;
    private bool _playerWon;
    
    public AppStateType StateType
    {
        get { return _type; }
        set { _type = value; }
    }
    
    public GameEndState()
    {
        _type = AppStateType.GameEnd;
    }
    
    public void OnEnter(IState previousState)
    {
        ShowGameEndPopup();
    }

    public void Update()
    {
        
    }

    public void OnExit()
    {
        ObjectPool.Instance.ClearPool();
        ParticlePool.Instance.ClearPool();
        PopUpManager.Instance.Clear();
    }

    private void ShowGameEndPopup()
    {
        _playerWon = false;
        float rewardPercentage = 0.0f;
        DetermineWInLossCondition(ref _playerWon, ref rewardPercentage);

        //Calculate reward 50% of score * rewardPercentage
        _rewardCoins = (int) (GameStats.Instance.Score * rewardPercentage / 200.0f);
        
        string msg = "Game Ended!\n";
        msg += "You " + (_playerWon ? "Won" : "Lose");
        
        GameObject gameEndPopUp = PopUpManager.Instance.PushToStack("Prefabs/UI/GameEndPopUp");

        _popUpScript = gameEndPopUp.GetComponent<GameEndUI>();
        _popUpScript.SetTitle("Info!");
        _popUpScript.SetMessage(msg);
        _popUpScript.SetButtonText("Ok",0);
        _popUpScript.AddListenerToButton(ClosePopUp,0);
        _popUpScript.SetCoinData(_rewardCoins);
        _popUpScript.SetRewardPercentage(rewardPercentage);
    }

    private void ClosePopUp()
    {
        _popUpScript.RemoveListenerFromButton(ClosePopUp,0);
        PopUpManager.Instance.PopFromStack();
        
        CurrencyController.Instance.AddDelta(CurrencyType.Coins,_rewardCoins );
        if (_playerWon)
        {
            MissionController.Instance.UnlockNextMission();   
        }
        User.Instance.Save();
        GameStats.Instance.RemoveListeners();
        
        GameEventManager.Instance.TriggerEvent(Event.OnMissionFinished , MissionController.Instance.Mission);
        AppStateMachine.Instance.SetCurrentState(AppStateType.Menu);
    }

    private void DetermineWInLossCondition(ref bool win , ref float rewardPercentage)
    {
        if(!GameStats.Instance.PlayerDied)
        {
            //Currently I have 3 stars in each level . It should be read that value from some where
            rewardPercentage = (float) GameStats.Instance.StarsCollected / 3.0f;
            foreach (var botSpawned in GameStats.Instance.BotsSpawned)
            {
                float spawned = botSpawned.Value;
                float killed = GameStats.Instance.BotsKilled.ContainsKey(botSpawned.Key)
                    ? (float)GameStats.Instance.BotsKilled[botSpawned.Key]
                    : 0.0f;

                rewardPercentage += killed / spawned;
            }

            rewardPercentage = rewardPercentage / (GameStats.Instance.BotsSpawned.Count + 1);
            rewardPercentage *= 100.0f;

            if (rewardPercentage > 50.0f && GameStats.Instance.PlayerReachedTarget)
            {
                win = true;
            }
        }
    }
}