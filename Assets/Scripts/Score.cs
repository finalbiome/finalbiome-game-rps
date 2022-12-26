using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI scoreEnergyUIText;
    public TextMeshProUGUI scoreDiamondsUIText;

    void Update()
    {
        scoreEnergyUIText.text = GameManager.Instance.Energy.ToString();
        scoreDiamondsUIText.text = GameManager.Instance.Diamonds.ToString();
    }
}
