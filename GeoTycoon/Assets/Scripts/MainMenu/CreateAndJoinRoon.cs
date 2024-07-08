using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Linq;

public class CreateAndJoinRoon : MonoBehaviourPunCallbacks
{
    public GameObject lobbyLayout;
    public GameObject multiPlayerMenu;
    public TMP_InputField createId;
    public TMP_InputField joinId;
    public TMP_InputField nickName;

    public void CreateRoom()
    {
        PhotonNetwork.NickName = nickName.text;
        PhotonNetwork.CreateRoom(createId.text);
    }
    public void JoinRoom()
    {
        PhotonNetwork.NickName = nickName.text;
        PhotonNetwork.JoinRoom(joinId.text);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        multiPlayerMenu.SetActive(false);
        lobbyLayout.SetActive(true);
    }
    public void LeaveLobby()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }

    public override void OnJoinedRoom()
    {
        multiPlayerMenu.SetActive(true);
        if(PhotonNetwork.IsMasterClient)
        {
            Debug.Log("You are room master.");
        }
        
        Debug.Log("Join Success");
        lobbyLayout.SetActive(false);

        // if(!PhotonNetwork.IsMasterClient)
        // {
            
        // }


    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join Failed");
        Debug.Log("" + returnCode);
        Debug.Log("" + message);
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    // public override void OnPlayerEnteredRoom(Player newPlayer)
    // {
    //     Debug.Log("Player " + newPlayer.NickName + " has joined.");
    //     int numberOfPlayer = PhotonNetwork.PlayerList.Count();
    //     Debug.Log("number of players: " + numberOfPlayer);
    // }

    // public override void OnPlayerLeftRoom(Player otherPlayer)
    // {
    //     Debug.Log("Player " + otherPlayer.NickName + " has left.");
    //     int numberOfPlayer = PhotonNetwork.PlayerList.Count();
    //     Debug.Log("number of players: " + numberOfPlayer);
    // }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LeaveRoom(true);
        PhotonNetwork.NickName = null;
        Debug.Log("Leave room");
    }
}
