using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class CreateRoomMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text _roomName;

    public void OnClick_CreateRoom()
    {
        //create Rooms
        //Join OR create Room
        if(!PhotonNetwork.IsConnected) return;

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom(_roomName.text, options, TypedLobby.Default);

    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Create Room Successfuly.", this);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Creation Failed." +message, this);
    }
}
