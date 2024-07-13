using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Linq;
using Unity.VisualScripting;
using Photon.Pun.UtilityScripts;
public class MainMenu : MonoBehaviourPunCallbacks
{
    public GameObject lobby;
    public GameObject loadingScreen;
    public GameObject multiPlayerMenu;

    public GameObject singlePlayerMenu;

    public Button startGameButton;


    public string[] playerNameList = new string[4];
    public int[] playerIdList = new int[4];

    [Serializable]
    public class PlayerSelect
    {
        public TMP_InputField nameInput;
        public TMP_Dropdown typeDropdown;
        public TMP_Dropdown colorDropdown;
        public Toggle toggle;
    }
    [Serializable]
    public class PlayerSelectMulti
    {
        public TMP_InputField nameInput;
        public TMP_Dropdown typeDropdown;
        public TMP_Dropdown colorDropdown;
        public Toggle toggle;
        public int playerId;
    }
    [SerializeField] PlayerSelect[] playerSelection;

    [SerializeField] public PlayerSelectMulti[] playerSelectionMulti;

    public string supportUrl;
    public string webUrl;

    

    [PunRPC]
    void SyncSetting(string[] currentPlayerNameList, int[] currentPlayerIdList)
    {
        
        playerNameList = currentPlayerNameList;
        playerIdList = currentPlayerIdList;
        
    }

    [PunRPC]
    void SyncStart()
    {
        foreach (var player in playerSelectionMulti)
        {
            if (player.toggle.isOn)
            {
                MultiSetting newSet = new MultiSetting(player.nameInput.text, player.typeDropdown.value, player.colorDropdown.value, player.playerId);
                GameSettings.AddMultiSetting(newSet);
            }
        }
    }
    public void Start()
    {
        lobby.SetActive(false);
        loadingScreen.SetActive(false);
        multiPlayerMenu.SetActive(false);
        singlePlayerMenu.SetActive(true);
        
    }

    
    public IEnumerator RoomListing(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (PhotonNetwork.IsConnected && PhotonNetwork.PlayerList.Count() > 0)
        {
            PhotonView PV = GetComponent<PhotonView>();
            PV.RPC("SyncSetting", RpcTarget.OthersBuffered, (object)playerNameList,(object)playerIdList);

            UpdateListing();
        }
    }

    public void UpdateListing()
    {
        int i = 0;
        
        foreach (PlayerSelectMulti playerSelectionMulti in playerSelectionMulti)
        {
            if (playerNameList[i] != "") playerSelectionMulti.toggle.isOn = true;
            playerSelectionMulti.nameInput.text = playerNameList[i];
            playerSelectionMulti.playerId = playerIdList[i];
            i++;
        }
    }

    public override void OnJoinedRoom()
    {
        foreach (PlayerSelectMulti playerSelectionMulti in playerSelectionMulti)
        {
            playerSelectionMulti.nameInput.text = "";
            playerSelectionMulti.typeDropdown.value = 0;
            playerSelectionMulti.toggle.isOn = false;

        }
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Master ID " + PhotonNetwork.MasterClient.ActorNumber);
            playerSelectionMulti[0].nameInput.text = PhotonNetwork.NickName;
            playerSelectionMulti[0].toggle.isOn = true;
            playerSelectionMulti[0].playerId = PhotonNetwork.MasterClient.ActorNumber;
            playerIdList[0] = PhotonNetwork.MasterClient.ActorNumber;
            playerNameList[0] = PhotonNetwork.NickName;
        }
        else
        {
            startGameButton.gameObject.SetActive(false);
            StartCoroutine(RoomListing(1f));
        }
        
    }

    public void StartButton()
    {
        if (PhotonNetwork.IsConnected)
        {
            foreach (var player in playerSelectionMulti)
            {
                if (player.toggle.isOn)
                {
                    MultiSetting newSet = new MultiSetting(player.nameInput.text, player.typeDropdown.value, player.colorDropdown.value, player.playerId);
                    GameSettings.AddMultiSetting(newSet);
                }
            }
            PhotonView PV = GetComponent<PhotonView>();
            PV.RPC("SyncStart", RpcTarget.OthersBuffered);
        }
        else
        {
            foreach (var player in playerSelection)
            {
                if (player.toggle.isOn)
                {
                    Setting newSet = new Setting(player.nameInput.text, player.typeDropdown.value, player.colorDropdown.value);
                    GameSettings.AddSetting(newSet);
                }
            }
        }

        

        PhotonNetwork.LoadLevel("MainGame");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        int i = 0;
        Debug.Log("current index on join: " + i);
        foreach (PlayerSelectMulti playerSelectionMulti in playerSelectionMulti)
        {
            Debug.Log("Slot: " + (i+1) + " | " + "Current name: " + playerSelectionMulti.nameInput.text + " | " + "Player nick name: " + newPlayer.NickName);
            if (playerSelectionMulti.nameInput.text == "" && playerSelectionMulti.toggle.isOn == false)
            {
                playerNameList[i] = newPlayer.NickName;
                playerIdList[i] = newPlayer.ActorNumber;
                playerSelectionMulti.nameInput.text = playerNameList[i];
                playerSelectionMulti.playerId = playerIdList[i];
                playerSelectionMulti.typeDropdown.value = 0;
                playerSelectionMulti.toggle.isOn = true;
                break;
            }
            i++;
        }
        StartCoroutine(RoomListing(0.3f));
        

    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int i = 0;
        Debug.Log("current index on left: " + i);
        foreach (PlayerSelectMulti playerSelectionMulti in playerSelectionMulti)
        {
             Debug.Log("Slot: " + (i+1) + " | " + "Current name: " + playerSelectionMulti.nameInput.text + " | " + "Player nick name: " + otherPlayer.NickName);
            if (playerSelectionMulti.nameInput.text == otherPlayer.NickName && playerSelectionMulti.toggle.isOn == true)
            {
                playerNameList[i] = "";
                playerIdList[i] = -1;
                playerSelectionMulti.nameInput.text = playerNameList[i];
                playerSelectionMulti.playerId = playerIdList[i];
                playerSelectionMulti.typeDropdown.value = 0;
                playerSelectionMulti.toggle.isOn = false;
                break;
            }
            i++;
        }
        
        StartCoroutine(RoomListing(0.3f));
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