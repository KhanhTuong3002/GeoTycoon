using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] MonopolyBoard gameBoard;
    [SerializeField] List<Player_Mono> playerList = new List<Player_Mono>();
    [SerializeField] int currentPlayer;
    [SerializeField] int maxTurnsInJail = 3; //Setting for how long in jail
    [SerializeField] int startMoney = 2000;
    [SerializeField] int goMoney = 500;

    [SerializeField] GameObject playerInfoPrefab;
    [SerializeField] Transform playerPanel; //for the playerInfo Prefabs to become parented to

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            GameObject infoObject = Instantiate(playerInfoPrefab, playerPanel, false);
            Player_MonoInfor info = infoObject.GetComponent<Player_MonoInfor>();
            playerList[i].Inititialize(gameBoard.route[0], startMoney, info);
        }
    }
}
