using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    public TextMeshProUGUI titleUIText;
    public TextMeshProUGUI scoreEnergyUIText;
    public TextMeshProUGUI scoreDiamondsUIText;
    public Image resultUIImage;
    public Sprite winResultUISprite;
    public Sprite loseResultUISprite;
    public Sprite drawResultUISprite;

    // Update is called once per frame
    internal void Update()
    {
        // if (gameResult is not null)
        var gameResult = GameManager.Instance.GameResult;
        {
            switch (gameResult)
            {
                case GameResult.Win:
                    titleUIText.text = "Great! You win";
                    resultUIImage.sprite = winResultUISprite;
                    break;
                case GameResult.Lose:
                    titleUIText.text = "Oops! You lose";
                    resultUIImage.sprite = loseResultUISprite;

                    break;
                case GameResult.Draw:
                    titleUIText.text = "Wow! Draw";
                    resultUIImage.sprite = drawResultUISprite;

                    break;
                default:
                    break;
            }

            scoreEnergyUIText.text = GameManager.Instance.GameResultEnergy.ToString("+#;-#;0");
            scoreDiamondsUIText.text = GameManager.Instance.GameResultDiamonds.ToString("+#;-#;0");
        }
    }
}
