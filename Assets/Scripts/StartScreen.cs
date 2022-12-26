using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    public Button PlayButton;

    void Update()
    {
        PlayButton.interactable = GameManager.Instance.IsLoggedIn;
    }
}
