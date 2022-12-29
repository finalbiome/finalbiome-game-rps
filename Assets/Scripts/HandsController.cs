using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandsController : MonoBehaviour
{

    public Sprite HandRock;
    public Sprite HandPaper;
    public Sprite HandScissor;

    public GameObject Turn;
    public GameObject ResultPlayer;
    public GameObject ResultEnemy;

    public RoundResult roundResult;
    public Screens currentScreen;

    private SpriteRenderer resultPlayerHandSprite;
    private SpriteRenderer resultEnemyHandSprite;

    // Start is called before the first frame update
    internal void Start()
    {
        resultPlayerHandSprite = ResultPlayer.GetComponent<SpriteRenderer>();
        resultEnemyHandSprite = ResultEnemy.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    internal void Update()
    {
        var gameManager = GameManager.Instance;
        Screens currentScreen = gameManager.CurrentScreen;

        // if screen is Strart, show hands in the special position
        if (currentScreen == Screens.Start)
        {
            resultPlayerHandSprite.sprite = HandToSprite(Hand.Scissor);
            resultEnemyHandSprite.sprite = HandToSprite(Hand.Paper);

            resultPlayerHandSprite.transform.position = new Vector3(-10.2f, -1f, 0f);
            resultPlayerHandSprite.transform.eulerAngles = new Vector3(0f, 0f, 15f);
            resultEnemyHandSprite.transform.position = new Vector3(10.5f, -0.4f, 0f);
            resultEnemyHandSprite.transform.eulerAngles = new Vector3(0f, 0f, -4f);

            ResultPlayer.SetActive(true);
            ResultEnemy.SetActive(true);
            Turn.SetActive(false);
            return;
        }
        else
        {
            // set default position
            resultPlayerHandSprite.transform.position = new Vector3(-9.5f, 0f, 0f);
            resultPlayerHandSprite.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            resultEnemyHandSprite.transform.position = new Vector3(9.5f, 0f, 0f);
            resultEnemyHandSprite.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }

        // show hands only on Round and RoundResult screens
        if (currentScreen != Screens.Round &&
            currentScreen != Screens.RoundResult &&
            currentScreen != Screens.GameResult)
        {
            ResultPlayer.SetActive(false);
            ResultEnemy.SetActive(false);
            Turn.SetActive(false);
            return;
        }

        bool turnIsMade = currentScreen == Screens.Round;

        // change hands if needed
        if (currentScreen == Screens.RoundResult || currentScreen == Screens.GameResult)
        {
            RoundResult roundResult = gameManager.CurrentRoundResult.Last();
            Hand selectedHand = (Hand)gameManager.LastSelectedHand;

            SetHandSprites(roundResult, selectedHand);
        }
        // set visibility based on mading turn
        ResultPlayer.SetActive(!turnIsMade);
        ResultEnemy.SetActive(!turnIsMade);
        Turn.SetActive(turnIsMade);
    }

    void SetHandSprites(RoundResult roundResult, Hand selectedHand)
    {
        resultPlayerHandSprite.sprite = HandToSprite(selectedHand);
        resultEnemyHandSprite.sprite = roundResult switch
        {
            RoundResult.Win => HandToSprite(GetWeaker(selectedHand)),
            RoundResult.Lose => HandToSprite(GetStronger(selectedHand)),
            RoundResult.Draw => HandToSprite(selectedHand),
            _ => throw new System.Exception("Unknown Round Result"),
        };
    }

    Sprite HandToSprite(Hand hand)
    {
        return hand switch
        {
            Hand.Rock => HandRock,
            Hand.Paper => HandPaper,
            Hand.Scissor => HandScissor,
            _ => throw new System.Exception("Unknown hand"),
        };
    }
    Hand GetStronger(Hand hand)
    {
        return hand switch
        {
            Hand.Rock => Hand.Paper,
            Hand.Paper => Hand.Scissor,
            Hand.Scissor => Hand.Rock,
            _ => throw new System.Exception("Unknown hand"),
        };
    }
    Hand GetWeaker(Hand hand)
    {
        return hand switch
        {
            Hand.Rock => Hand.Scissor,
            Hand.Paper => Hand.Rock,
            Hand.Scissor => Hand.Paper,
            _ => throw new System.Exception("Unknown hand"),
        };
    }
}
