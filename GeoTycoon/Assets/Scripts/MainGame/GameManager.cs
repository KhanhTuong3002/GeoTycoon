using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] MonopolyBoard gameBoard;
    [SerializeField] List<Player_Mono> playerList = new List<Player_Mono>();
    [SerializeField] int currentPlayer;
    [Header("Global Game Settings")]
    [SerializeField] int maxTurnsInJail = 3; // Setting for how long in jail
    [SerializeField] int startMoney = 2000;
    [SerializeField] int goMoney = 500;
    [Header("Player Info")]
    [SerializeField] GameObject playerInfoPrefab;
    [SerializeField] Transform playerPanel; // For the playerInfo Prefabs to become parented to
    [SerializeField] List<GameObject> playerTokenList = new List<GameObject>();

    //about the rolling dice
    int[] rolledDice;
    bool rolledADouble;
    int doubleRollCount;
    // pass over go to get money
    public int GetGoMoney => goMoney;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Inititialize();
        if (playerList[currentPlayer].playerType == Player_Mono.PlayerType.AI)
        {
            RollDice();
        }
        else
        {
            //show ui for human input
        }
    }

    void Inititialize()
    {
        //create all player
        for (int i = 0; i < playerList.Count; i++)
        {
            GameObject infoObject = Instantiate(playerInfoPrefab, playerPanel, false);
            Player_MonoInfor info = infoObject.GetComponent<Player_MonoInfor>();

            //Random token
            int randomIndex = Random.Range(0, playerTokenList.Count);
            //Instatiate
            GameObject newToken = Instantiate(playerTokenList[randomIndex], gameBoard.route[0].transform.position, Quaternion.identity);
            playerList[i].Inititialize(gameBoard.route[0], startMoney, info, newToken);
        }
    }

    public void RollDice() //press button form human or auto from ai
    {
        //reset last roll
        rolledDice = new int[2];

        //any roll dice and store them
        rolledDice[0] = Random.Range(1, 7);
        rolledDice[1] = Random.Range(1, 7);
        Debug.Log("rolled dice are:" + rolledDice[0] + " & " + rolledDice[1]);
        //check for double
        rolledADouble = rolledDice[0] == rolledDice[1];
        //throw 3 times in a row -> jail anyhow -> end turn

        //is in jail already

        //can we leave jail

        //move anyhow if allowed
        StartCoroutine(DelayBeforeMove(rolledDice[0] + rolledDice[1]));
        //show or hide ui
    }
    IEnumerator DelayBeforeMove(int rolledDice)
    {
        yield return new WaitForSeconds(2f);
        //if we are allowed to move we do so
        gameBoard.MovePlayertonken(rolledDice, playerList[currentPlayer]);
            //else we switch
    }

    public void SwitchPlayer()
    {
        currentPlayer++;
        //rolledouble?

        //overflow check
        if(currentPlayer >= playerList.Count)
        {
            currentPlayer = 0;
        }

        //check if in jail


        //is player Ai
        if (playerList[currentPlayer].playerType == Player_Mono.PlayerType.AI)
        {
            RollDice();
        }

        //if human - show ui
    }
}
