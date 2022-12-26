using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Rules : MonoBehaviour
{
    public TextMeshProUGUI gamePriceUIText;
    public TextMeshProUGUI gameScoreUIText;
    public TextMeshProUGUI playerNameUIText;

    public Button PlayButton;
    
    internal void Update()
    {
        gamePriceUIText.text = GameManager.Instance.GamePrice.ToString();
        gameScoreUIText.text = GameManager.Instance.Diamonds.ToString();
        playerNameUIText.text = $"Your name:\n{GameManager.Instance.PlayerName}";

        PlayButton.interactable = GameManager.Instance.Energy >= GameManager.Instance.GamePrice;
    }
}
