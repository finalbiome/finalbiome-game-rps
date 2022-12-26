using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Round : MonoBehaviour
{
    public TextMeshProUGUI titleUIText;

    internal void Update()
    {
        if (GameManager.Instance.CurrentRoundResult.Count == 0) return;
        var lastRpundResult = GameManager.Instance.CurrentRoundResult.Last();
        switch (lastRpundResult)
        {
            case RoundResult.Win:
                titleUIText.text = "You win!";
                break;
            case RoundResult.Lose:
                titleUIText.text = "You lose!";
                break;
            case RoundResult.Draw:
                titleUIText.text = "Draw!";
                break;
            default:
                break;
        }
    }
}
