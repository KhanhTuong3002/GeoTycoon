using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuOffl : MonoBehaviour
{
    [Serializable]
    public class PlayerSelect
    {
       
        public TMP_InputField nameInput;
        public TMP_Dropdown typeDropdown;
        public TMP_Dropdown colorDropdown;
        public Toggle toggle;
    }
    [SerializeField] PlayerSelect[] playerSelection;

    public TMP_InputField setIDInput; 
    public string supportUrl;
    public string webUrl;
    public void StartButton()
    {
        foreach (var player in playerSelection)
        {
            if (player.toggle.isOn)
            {
                Setting newSet = new Setting(player.nameInput.text, player.typeDropdown.value, player.colorDropdown.value);
                GameSettings.AddSetting(newSet);
            }
        }

        string setID = setIDInput.text;
        if (string.IsNullOrEmpty(setID))
        {
            setID = "defaultSetID";  // Replace with your actual default SetID
        }
        GameSettings.SetSetID(setID);
        Debug.Log("****your SetID is**** "+ setID);
        SceneManager.LoadScene("MainGame Offl");
    }

    public void SupportUs()
    {
        Application.OpenURL(supportUrl);
    }

    public void VisitUs()
    {
        Application.OpenURL(webUrl);
    }
}

