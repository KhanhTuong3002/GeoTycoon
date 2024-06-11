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
    [SerializeField] int maxTurnsInJail = 3; // Setting for how long in jail
    [SerializeField] int startMoney = 2000;
    [SerializeField] int goMoney = 500;

    [SerializeField] GameObject playerInfoPrefab;
    [SerializeField] Transform playerPanel; // For the playerInfo Prefabs to become parented to

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (playerInfoPrefab == null)
        {
            Debug.LogError("PlayerInfoPrefab is not assigned in the Inspector.");
            return;
        }

        if (playerPanel == null)
        {
            Debug.LogError("PlayerPanel is not assigned in the Inspector.");
            return;
        }

        for (int i = 0; i < playerList.Count; i++)
        {
            GameObject infoObject = Instantiate(playerInfoPrefab, playerPanel, false);
            Player_MonoInfor info = infoObject.GetComponent<Player_MonoInfor>();

            if (info == null)
            {
                Debug.LogError("PlayerInfoPrefab does not have a Player_MonoInfor component.");
                continue;
            }

            playerList[i].Inititialize(gameBoard.route[0], startMoney, info);
        }
    }
}
