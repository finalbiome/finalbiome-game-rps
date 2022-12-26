using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null; 
    public Screens CurrentScreen;
    /// <summary>
    /// Total diamonds the player has
    /// </summary>
    public int Diamonds;
    /// <summary>
    /// Total energy the player has
    /// </summary>
    public int Energy;
    /// <summary>
    /// Price of the one game
    /// </summary>
    public int GamePrice;
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
    public int GameResultEnergy;
    /// <summary>
    /// Final result of the game in the diamonds
    /// </summary>
    public int GameResultDiamonds;
    /// <summary>
    /// The name of the gamer
    /// </summary>
    public string PlayerName;

    public Canvas CanvasStartScreen;
    public Canvas CanvasScore;
    public Canvas CanvasRulesScreen;
    public Canvas CanvasChooseScreen;
    public Canvas CanvasRoundScore;
    public Canvas CanvasRound;
    public Canvas CanvasResult;

    internal void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);    
        DontDestroyOnLoad(gameObject);

        InitGame();
    }

    // Update is called once per frame
    void Update()
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
    }

    /// <summary>
    /// Activates desired set of canvases
    /// </summary>
    /// <param name="screen"></param>
    public void SetScreen(Screens screen)
    {
        // if (screen == CurrentScreen) return;

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
                Debug.Log("Not implemented");
                break;
        }

        // if (screen == Screens.Rules && CurrentScreen != Screens.Start)
        // {
        //     throw new System.Exception($"Cannot swith to {screen} screen from {CurrentScreen} screen");
        // }
        // else
        // {

        // }
    }

    public void OnClickBackFromGameResult()
    {
        // When we returns from game result screeg, we just going to stant screen
        CurrentScreen = Screens.Start;
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
    }

    /// <summary>
    /// When click in the rules screen and run new game
    /// </summary>
    public void OnClickStartGame()
    {
        // go to screen for choosing the hand
        CurrentScreen = Screens.Choose;
    }

    /// <summary>
    /// When was selected some hand for the next round
    /// </summary>
    /// <param name="hand"></param>
    async Task SetSelectedHand(Hand hand)
    {
        // 
        Debug.Log($"Selected {hand}");
        // going to round screen
        CurrentScreen = Screens.Round;
        // start calculating result
        await Task.Delay(3_000);
        // set result
        
        // todo.................
        var result = (RoundResult)Random.Range(0, 3);
        CurrentRoundResult.Add(result);

        if (CurrentRoundResult.Count == 5)
        {
            GameResult = GameResult.Win;
            GameResultDiamonds = 10;
            GameResultEnergy = 20;
        }
        // ...................
        
        // go to round result screen
        CurrentScreen = Screens.RoundResult;
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


}
