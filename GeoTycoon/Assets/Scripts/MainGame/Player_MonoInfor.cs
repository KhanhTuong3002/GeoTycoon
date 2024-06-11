using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player_MonoInfor : MonoBehaviour
{
    [SerializeField] TMP_Text PlayerNameText; 
    [SerializeField] TMP_Text PlayerCashText;
    
    public void SetPlayerName(string newName)
    {
        PlayerNameText.text = newName;
    }
    public void SetPlayerCash(int currentCash)
    {
        PlayerCashText.text = "$ " + currentCash.ToString();
    }

    public void SetPlayerNameandCash(int currentCash, string newName)
    {
        SetPlayerName(newName);
        SetPlayerCash(currentCash);
    }
}
