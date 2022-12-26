using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundScore : MonoBehaviour
{
    public Sprite winRoundUISprite;
    public Sprite loseRoundUISprite;
    public Sprite drawRoundUISprite;
    public Sprite emptyRoundUISprite;

    public List<Image> gamerScoreUIImages;
    public List<Image> enemyScoreUIImages;

    internal void Update()
    {
        var roundResults = GameManager.Instance.CurrentRoundResult;
        for (int i = 0; i < 5; i++)
        {
            if (roundResults.Count > i)
            {
                switch (roundResults[i])
                {
                    case RoundResult.Win:
                        gamerScoreUIImages[i].sprite = winRoundUISprite;
                        enemyScoreUIImages[i].sprite = loseRoundUISprite;
                        break;
                    case RoundResult.Lose:
                        gamerScoreUIImages[i].sprite = loseRoundUISprite;
                        enemyScoreUIImages[i].sprite = winRoundUISprite;
                        break;
                    case RoundResult.Draw:
                        gamerScoreUIImages[i].sprite = drawRoundUISprite;
                        enemyScoreUIImages[i].sprite = drawRoundUISprite;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                gamerScoreUIImages[i].sprite = emptyRoundUISprite;
                enemyScoreUIImages[i].sprite = emptyRoundUISprite;
            }
        }
    }
}
