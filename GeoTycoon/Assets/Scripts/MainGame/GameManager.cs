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
    [SerializeField] float secondsBetweenTurns = 3;
    [Header("Player Info")]
    [SerializeField] GameObject playerInfoPrefab;
    [SerializeField] Transform playerPanel; // For the playerInfo Prefabs to become parented to
    [SerializeField] List<GameObject> playerTokenList = new List<GameObject>();

    //about the rolling dice
    int[] rolledDice;
    bool rolledADouble;
    public bool RolledADouble => rolledADouble;
    int doubleRollCount;
    //tax ppol
    int taxPool = 0;
    // pass over go to get money
    public int GetGoMoney => goMoney;
    public float SecondsBetweenTurns => secondsBetweenTurns;
    //Message System
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;
    //debug
    public bool AllwaysDoubleRoll = false;

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
        bool allowedToMove = true;
        //reset last roll
        rolledDice = new int[2];

        //any roll dice and store them
        rolledDice[0] = Random.Range(1, 7);
        rolledDice[1] = Random.Range(1, 7);
        Debug.Log("rolled dice are:" + rolledDice[0] + " & " + rolledDice[1]);


        //Debug
        if (AllwaysDoubleRoll)
        {
            rolledDice[0] = 5;
            rolledDice[1] = 5;
        }


        //check for double
        rolledADouble = rolledDice[0] == rolledDice[1];
        //throw 3 times in a row -> jail anyhow -> end turn

        //is in jail already
        if (playerList[currentPlayer].IsInjail)
        {
            playerList[currentPlayer].IcreaseNumTurnInJail();

            if(rolledADouble)
            {
                playerList[currentPlayer].setOutOfJail();
                OnUpdateMessage.Invoke(playerList[currentPlayer].name + " <color=green>can leave jail</color>, because a double was rolled");
                doubleRollCount++;
                //Move the player
            }
            else if (playerList[currentPlayer].NumTurnInjail >= maxTurnsInJail)
            {
                // we have been long enough here
                playerList[currentPlayer].setOutOfJail();
                OnUpdateMessage.Invoke(playerList[currentPlayer].name + " <color=green>can leave jail from now</color>");
                //allowed to leave
            }
            else
            {
                allowedToMove = false;
            }
        }
        else//Not in jail
        {
            // reset double roll
            if (!rolledADouble)
            {
                doubleRollCount = 0;
            }
            else
            {
                doubleRollCount++;
                if(doubleRollCount >= 3)
                {
                    //move to jail
                    int indexOnBoard = MonopolyBoard.instance.route.IndexOf(playerList[currentPlayer].MyMonopolyNode);
                    playerList[currentPlayer].GoToJail(indexOnBoard);
                    OnUpdateMessage.Invoke(playerList[currentPlayer].name + " has rolled <b>3 times a double</b>, and has to <b><color=red>go to jail!</color></b>");
                    rolledADouble = false; //reset
                    return;
                }
            }

        }

        //can we leave jail

        //move anyhow if allowed

        if(allowedToMove)
        {
            OnUpdateMessage.Invoke(playerList[currentPlayer].name + " has rolled: " +rolledDice[0] + " & "+ rolledDice[1]);
            StartCoroutine(DelayBeforeMove(rolledDice[0] + rolledDice[1]));

        }
        else
        {
            //Switch Player
            OnUpdateMessage.Invoke(playerList[currentPlayer].name + " <b><color=red>has to stay in Jail</color></b>");
            Debug.Log("WE CAN NOT MOVE BECAUSE NOT ALLOWED");
            StartCoroutine(DelayBetweenSwitchPlayer());
        }
        //show or hide ui
    }
    IEnumerator DelayBeforeMove(int rolledDice)
    {
        yield return new WaitForSeconds(secondsBetweenTurns);
        //if we are allowed to move we do so
        gameBoard.MovePlayertonken(rolledDice, playerList[currentPlayer]);
            //else we switch
    }
    IEnumerator DelayBetweenSwitchPlayer()
    {
        yield return new WaitForSeconds(secondsBetweenTurns);
        SwitchPlayer();
    }

    public void SwitchPlayer()
    {
        currentPlayer++;
        //rolledouble?
        doubleRollCount = 0;


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

    public int[] LastRolledDice => rolledDice;

    public void AddTaxToPool(int amount)
    {
        taxPool += amount;
    }

    public int GetTaxPool()
    {
        //temp store taxpool
        int durrentTaxCollected = taxPool;
        //reset the taxpool
        taxPool = 0;
        // send temp tax
        return durrentTaxCollected;
    }
}
