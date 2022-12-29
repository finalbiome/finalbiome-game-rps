using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using FinalBiome.Sdk;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null; 
    public Screens CurrentScreen;
    /// <summary>
    /// Total diamonds the player has
    /// </summary>
    public BigInteger Diamonds;
    /// <summary>
    /// Total energy the player has
    /// </summary>
    public BigInteger Energy;
    /// <summary>
    /// Price of the one game
    /// </summary>
    public BigInteger GamePrice;
    /// <summary>
    /// List of the current round results
    /// </summary>
    public List<RoundResult> CurrentRoundResult;
    /// <summary>
    /// Final result of the game;
    /// </summary>
    public GameResult GameResult;
    /// <summary>
    /// Final result of the game in the energy
    /// </summary>
    public BigInteger GameResultEnergy;
    /// <summary>
    /// Final result of the game in the diamonds
    /// </summary>
    public BigInteger GameResultDiamonds;
    /// <summary>
    /// The name of the gamer
    /// </summary>
    public string PlayerName;

    private FinalBiomeManager fbManager;

#region UI Editor Properties
    public Canvas CanvasStartScreen;
    public Canvas CanvasScore;
    public Canvas CanvasRulesScreen;
    public Canvas CanvasChooseScreen;
    public Canvas CanvasRoundScore;
    public Canvas CanvasRound;
    public Canvas CanvasResult;
#endregion
    
    /// <summary>
    /// Address of the game
    /// </summary>
    public string GameAddress;
    /// <summary>
    /// Endpoint of FinalBiome Network
    /// </summary>
    public string Endpoint = "ws://127.0.0.1:9944";
    /// <summary>
    /// Id of the fungible asset Energy
    /// </summary>
    public uint FbFaEnergyId;
    /// <summary>
    /// Id of the fungible asset Diamonds
    /// </summary>
    public uint FbFaDiamondsId;
    /// <summary>
    /// Id of the non-fungible asset Bet
    /// </summary>
    public uint FbNfaBetClassId;

    public MxId? currentBetMechanic;
    public uint? faNfaBetInstanceId;

    public int ResultDelay;

    public bool IsLoggedIn;

    private bool hasFirstTurn;

    public Hand? LastSelectedHand;

    /// <summary>
    /// Temp hardcoded username
    /// </summary>
    public string UserEmail;
    /// <summary>
    /// Temp hardcoded password
    /// </summary>
    public string UserPassword;

    internal void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);    
        DontDestroyOnLoad(gameObject);

        InitGame();
    }

    internal async void Start()
    {
        // get instance of the FinalBiome manager
        ClientConfig config = new(GameAddress, Endpoint);
        
        fbManager = await FinalBiomeManager.GetInstance();
        await FinalBiomeManager.Initialize(config);

        Debug.Log($"fbManager.Client :{fbManager.Client is not null}");
        Debug.Log($"fbManager.Client.Auth :{fbManager.Client.Auth is not null}");

        // listen user state changes
        fbManager.Client.Auth.StateChanged += UserStateChangedHandler;
        // listen changes of Fa
        fbManager.Client.Fa.FaBalanceChanged += FaBalanceChangedHandler;
        // listen changes of Nfa
        fbManager.Client.Nfa.NfaInstanceChanged += NfaInstanceChangedHandler;
        // start listen changes of the mechanics
        fbManager.Client.Mx.MechanicsChanged += MxInstanceChangedHandler;

        // sign in with hardcoded credentials
        if (!await fbManager.Client.Auth.IsLoggedIn())
        {
            Debug.Log("Sign In");
            await fbManager.Client.Auth.SignInWithEmailAndPassword(UserEmail, UserPassword);
        }
    }

    // Update is called once per frame
    internal void Update()
    {
        SetScreen(CurrentScreen);
    }

    void InitGame()
    {
        CurrentScreen = Screens.Start;
        Diamonds = 0;
        Energy = 0;
        GamePrice = 20;
        CurrentRoundResult = new();
        GameResult = GameResult.Undefined;
        IsLoggedIn = false;
        LastSelectedHand = null;
    }

    /// <summary>
    /// Activates desired set of canvases
    /// </summary>
    /// <param name="screen"></param>
    public void SetScreen(Screens screen)
    {
        switch (screen)
        {
            case Screens.Start:
                CanvasStartScreen.gameObject.SetActive(true);
                CanvasScore.gameObject.SetActive(false);
                CanvasRulesScreen.gameObject.SetActive(false);
                CanvasChooseScreen.gameObject.SetActive(false);
                CanvasRoundScore.gameObject.SetActive(false);
                CanvasRound.gameObject.SetActive(false);
                CanvasResult.gameObject.SetActive(false);
                break;
            case Screens.Rules:
                CanvasStartScreen.gameObject.SetActive(false);
                CanvasScore.gameObject.SetActive(false);
                CanvasRulesScreen.gameObject.SetActive(true);
                CanvasChooseScreen.gameObject.SetActive(false);
                CanvasRoundScore.gameObject.SetActive(false);
                CanvasRound.gameObject.SetActive(false);
                CanvasResult.gameObject.SetActive(false);
                break;
            case Screens.Choose:
                CanvasStartScreen.gameObject.SetActive(false);
                CanvasScore.gameObject.SetActive(true);
                CanvasRulesScreen.gameObject.SetActive(false);
                CanvasChooseScreen.gameObject.SetActive(true);
                CanvasRoundScore.gameObject.SetActive(true);
                CanvasRound.gameObject.SetActive(false);
                CanvasResult.gameObject.SetActive(false);
                break;
            case Screens.Round:
                CanvasStartScreen.gameObject.SetActive(false);
                CanvasScore.gameObject.SetActive(true);
                CanvasRulesScreen.gameObject.SetActive(false);
                CanvasChooseScreen.gameObject.SetActive(false);
                CanvasRoundScore.gameObject.SetActive(true);
                CanvasRound.gameObject.SetActive(false);
                CanvasResult.gameObject.SetActive(false);
                break;
            case Screens.RoundResult:
                CanvasStartScreen.gameObject.SetActive(false);
                CanvasScore.gameObject.SetActive(true);
                CanvasRulesScreen.gameObject.SetActive(false);
                CanvasChooseScreen.gameObject.SetActive(false);
                CanvasRoundScore.gameObject.SetActive(true);
                CanvasRound.gameObject.SetActive(true);
                CanvasResult.gameObject.SetActive(false);
                break;
            case Screens.GameResult:
                CanvasStartScreen.gameObject.SetActive(false);
                CanvasScore.gameObject.SetActive(true);
                CanvasRulesScreen.gameObject.SetActive(false);
                CanvasChooseScreen.gameObject.SetActive(false);
                CanvasRoundScore.gameObject.SetActive(true);
                CanvasRound.gameObject.SetActive(false);
                CanvasResult.gameObject.SetActive(true);
                break;
            default:
                throw new System.Exception($"Unknown Screen: {screen}");
        }

    }

    public void OnClickBackFromGameResult()
    {
        // When we returns from game result screeg, we just going to stant screen
        CurrentScreen = Screens.Start;
        CurrentRoundResult.Clear();
        LastSelectedHand = null;
    }

    public void OnClickNextRound()
    {
        if (GameResult == GameResult.Undefined)
            CurrentScreen = Screens.Choose;
        else
            CurrentScreen = Screens.GameResult;
    }

    public void OnClickBackIntermediate()
    {
        // just go to start and ignore current state of the game
        CurrentScreen = Screens.Start;
    }

    /// <summary>
    /// When click the button at the spash screen
    /// </summary>
    public void OnClickStartPlay()
    {
        CurrentScreen = Screens.Rules;
        GameResult = GameResult.Undefined;
    }

    /// <summary>
    /// When click in the rules screen and run new game
    /// </summary>
    public async void OnClickStartGame()
    {
        await StartGame();
        // go to screen for choosing the hand
        CurrentScreen = Screens.Choose;
    }

    /// <summary>
    /// When was selected some hand for the next round
    /// </summary>
    /// <param name="hand"></param>
    async Task SetSelectedHand(Hand hand)
    {
        hasFirstTurn = true;

        LastSelectedHand = hand;
        // going to round screen
        CurrentScreen = Screens.Round;
        // send command to the FinalBiome
        var makeTurn = MakeTurn();
        // but delay showing results
        var delayResult = Task.Delay(ResultDelay);
        await Task.WhenAll(makeTurn, delayResult);
        var results = await makeTurn;

        CurrentRoundResult = results;
       
        // go to the round result screen or to the game result screen
        if (GameResult == GameResult.Undefined)
            CurrentScreen = Screens.RoundResult;
        else
            CurrentScreen = Screens.GameResult;
    }

    public async void OnClickSelectRockHand()
    {
        await SetSelectedHand(Hand.Rock);
    }
    public async void OnClickSelectPaperHand()
    {
        await SetSelectedHand(Hand.Paper);
    }
    public async void OnClickSelectScissonHand()
    {
        await SetSelectedHand(Hand.Scissor);
    }

    /// <summary>
    /// If FA changes, we update the energy and diamonds balances
    /// </summary>
    /// <param name="o"></param>
    /// <param name="e"></param>
    void FaBalanceChangedHandler(object o, FaBalanceChangedEventArgs e)
    {
        if (e.Id == FbFaEnergyId) Energy = e.Balance;
        else if (e.Id == FbFaDiamondsId) Diamonds = e.Balance;
    }

    /// <summary>
    /// If NFA Bet changed, update it
    /// </summary>
    /// <param name="o"></param>
    /// <param name="e"></param>
    void NfaInstanceChangedHandler(object o, NfaInstanceChangedEventArgs e)
    {
        // if we reseive event about nfa changes, this mean that gamer owns NFA
        // and we suppose if faNfaBetInstanceId is not set, set it. Because we know than only one type of NFA exists in this game.
        if (e.details is not null)
            faNfaBetInstanceId = e.instanceId;
        else
            faNfaBetInstanceId = null;
    }

    /// <summary>
    /// Listen user state
    /// </summary>
    /// <param name="loggedIn"></param>
    /// <returns></returns>
    async Task UserStateChangedHandler(bool loggedIn)
    {
        Debug.Log($"User logged in: {loggedIn}");

        if (loggedIn)
        {
            PlayerName = fbManager.Client.Auth.UserInfo.DisplayName ?? fbManager.Client.Auth.UserInfo.Email;
        Debug.Log($"PlayerName: {PlayerName}");

            await InitGamePrice();
            InitBalances();

            // we need to onboard of the gamer
            if (!(bool)fbManager.Client.Game.IsOnboarded)
            {
                await fbManager.Client.Mx.OnboardToGame();
            }
        }
        IsLoggedIn = loggedIn;
        await Task.Yield();
    }

    /// <summary>
    /// Get price of the Bet NFA and set in in the game manager
    /// </summary>
    /// <returns></returns>
    async Task InitGamePrice()
    {
        // we know that price of the bet nfa store in the Perchased characteristic of the Bet class.
        // so, get the class details
        var details = await fbManager.Client.Nfa.GetClassDetails(FbNfaBetClassId);
        // take the price from the first offer of the purchased chracteristic our bet class
        GamePrice = details.Purchased.Value.Offers.Value[0].Price.Value;
    }

    /// <summary>
    /// Set the balances of our Energy ad Diamonds to the game manager
    /// </summary>
    /// <returns></returns>
    void InitBalances()
    {
        Energy = fbManager.Client.Fa.Balances.GetValueOrDefault(FbFaEnergyId, 0);
        Diamonds = fbManager.Client.Fa.Balances.GetValueOrDefault(FbFaDiamondsId, 0);
    }

    /// <summary>
    /// Listen to mechanics changed and set current round intermediate results.
    /// It necessary if game starts and user already has not finished Mx.
    /// </summary>
    /// <param name="e"></param>
    void MxInstanceChangedHandler(object o, MechanicsChangedEventArgs e)
    {
        Debug.Log("MxInstanceChangedHandler");

        if (e.Details is not null)
        {
            // You can also listen changes of mechnics here, not only as a result executing specific mechanics.
            // look at Bet mechanics changes.
            if (e.Details.Data.Value == FinalBiome.Api.Types.PalletMechanics.Types.InnerMechanicData.Bet)
            {
                currentBetMechanic = e.Id;

                if (!hasFirstTurn)
                {
                    // we are here if we had an active mechanics and there was no turn.
                    // i.e. the game is just starting and the user has an active mechanic that will be used as the current one in this round
                    // in other cases, we do not use the result from here, because we do not need to show the result to the player in advance
                    var mechanicsData = e.Details.Data.Value2 as FinalBiome.Api.Types.PalletMechanics.Types.MechanicDataBet;
                    var outcomeIds = mechanicsData.Outcomes.Value;
                    CurrentRoundResult = OutcomesToRoundResult(outcomeIds.Select(o => (uint)o).ToList());
                }
            }
        }
        else
        {
            // mechanic was finished and dropped
            if (e.Id.nonce == currentBetMechanic.Value.nonce)
            {
                currentBetMechanic = null;
                faNfaBetInstanceId = null;
            }
        }

    }

    /// <summary>
    /// Convert outcome from Mx to the round result
    /// </summary>
    /// <param name="outcome"></param>
    /// <returns></returns>
    RoundResult MxBetOutcomeToRoundResult(uint outcome)
    {
        return outcome switch
        {
            0 => RoundResult.Win,
            1 => RoundResult.Lose,
            2 => RoundResult.Draw,
            _ => throw new System.Exception($"Cannot convert outcome {outcome} to the round result"),
        };
    }

    /// <summary>
    /// When we push the Play button, we should have a Bet NFA. If we don't have it, we should buy it.
    /// </summary>
    /// <returns></returns>
    async Task StartGame()
    {
        if (faNfaBetInstanceId is null)
        {
            // buys Bet nfa. We know what only one offer exists in our game spec, so hardcode it.
            Debug.Log("ExecBuyNfa");
            var res = await fbManager.Client.Mx.ExecBuyNfa(FbNfaBetClassId, 0);
            faNfaBetInstanceId = res.Result.InstanceId;
            CurrentRoundResult.Clear();
            LastSelectedHand = null;
        }
    }

    /// <summary>
    /// When we select the hand (make turn), we exec the Mx.
    /// There are two options here: it's the first turn (we have no already runned mechanics)
    /// and it's the next one, when we already have active Mx 
    /// </summary>
    /// <returns></returns>
    async Task<List<RoundResult>> MakeTurn()
    {
        if (faNfaBetInstanceId is null)
            throw new System.Exception("Gamer not owns the Bet instance");

        if (currentBetMechanic is null)
        {
            // start the Bet Mx
            Debug.Log("ExecBet");
            var mxResult = await fbManager.Client.Mx.ExecBet(FbNfaBetClassId, (uint)faNfaBetInstanceId);
            currentBetMechanic = mxResult.Id;
            // we know what there are 5 rounds
            // set round result
            return OutcomesToRoundResult(mxResult.Reason.Data.Outcomes);
        }
        else
        {
            // upgrade existed Mx
            Debug.Log("UpgradeBet");
            var mxResult = await fbManager.Client.Mx.UpgradeBet((MxId)currentBetMechanic);
            // Upgrade can has both results: Finished and stopped
            if (mxResult.Status == ResultStatus.Finished)
            {
                currentBetMechanic = null;
                // game is done. Set the game results.
                switch (mxResult.Result.BetResult)
                {
                    case BetResult.Won:
                        GameResult = GameResult.Win;
                        GameResultEnergy = GamePrice;
                        GameResultDiamonds = 10; // From game spec
                        break;
                    case BetResult.Lost:
                        GameResult = GameResult.Lose;
                        GameResultEnergy = -GamePrice;
                        GameResultDiamonds = 0;
                        break;
                    case BetResult.Draw:
                        GameResult = GameResult.Draw;
                        GameResultEnergy = 0;
                        GameResultDiamonds = 0;
                        break;
                }
                // set round result
                return OutcomesToRoundResult(mxResult.Result.Outcomes);
            }
            else // if (mxResult.Status == ResultStatus.Stopped)
            {
                // set round result
                return OutcomesToRoundResult(mxResult.Reason.Data.Outcomes);
            }
        }
    }

    /// <summary>
    /// Converts outcomes to round results
    /// </summary>
    /// <param name="outcomes"></param>
    List<RoundResult> OutcomesToRoundResult(List<uint> outcomes)
    {
        Debug.Log($"Outcomes: [{string.Join(", ", outcomes.Select(o => (RoundResult)o))}]");

        List<RoundResult> results = new();
        for (int i = 0; i < outcomes.Count; i++)
        {
            
            results.Add(MxBetOutcomeToRoundResult(outcomes[i]));
        }
        return results;
    }

}
