using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUi : MonoBehaviour
{
    public TextMeshProUGUI goldText;

    //instance
    public static GameUi instance;

    void Awake()
    {
        instance = this;
    }

    public void UpdateGoldText(int amount)
    {
        goldText.text = "<b>Gold:</b> " + amount;
    }
}
