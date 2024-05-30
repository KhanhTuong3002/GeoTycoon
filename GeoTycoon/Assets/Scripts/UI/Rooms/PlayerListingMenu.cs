using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform _content;
    [SerializeField]
    private PlayerListing _playerlisting;
    [SerializeField]
    private Text _readyUpText;

    private List<PlayerListing> _listings = new List<PlayerListing>();
    private RoomsCanvases _roomsCanvases;
    private bool _ready = false;


    public override void OnEnable()
    {
        base.OnEnable();
        SetReadyUp(false);
        GetCurrentRoomPlayers();
    }

    public override void OnDisable() 
    { 
        base.OnDisable();
        for (int i = 0; i < _listings.Count; i++)
            Destroy(_listings[i].gameObject);
        _listings.Clear();
    }
    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
    }

    private void SetReadyUp(bool state)
    {
        _ready = state;
        if (state)
        {
            if (_ready)
            {
                _readyUpText.text = "R";
            }
            else
            {
                _readyUpText.text = "N";
            }
        }

    }

    private void GetCurrentRoomPlayers()
    {
        if (!PhotonNetwork.IsConnected)
            return;
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;

        foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            AddPlayerListing(playerInfo.Value);
        }

    }

    private void AddPlayerListing(Player player)
    {
        int index = _listings.FindIndex(x => x.Player == player);
        if (index!= -1)
        {
            _listings[index].SetPlayerInfo(player);
        }
        else
        {
            PlayerListing listing = Instantiate(_playerlisting, _content);
            if (listing != null)
            {
                listing.SetPlayerInfo(player);
                _listings.Add(listing);
            }
        }
        
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        _roomsCanvases.CurrentRoomCanva.LeaveRoomMenu.OnClick_LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerListing(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = _listings.FindIndex(x => x.Player == otherPlayer);
        if (index != -1)
        {
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
    }

    
}
