﻿using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
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
    [Header("Game Over/ Win Info")]
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TMP_Text winnerNameText;
    [Header("Dice")]
    [SerializeField] Dice _dice1;
    [SerializeField] Dice _dice2;
    //about the rolling dice
    List <int> rolledDice = new List<int>();
    bool rolledADouble;
    public bool RolledADouble => rolledADouble;
    public void ResetRolledADouble() => rolledADouble=false;
    int doubleRollCount;
    bool hasRolledDice;
    public bool HasRolledDice => hasRolledDice;
    //tax ppol
    int taxPool = 0;
    // pass over go to get money
    public int GetGoMoney => goMoney;
    public List<Player_Mono> GetPlayers => playerList;
    public float SecondsBetweenTurns => secondsBetweenTurns;
    public Player_Mono GetCurrentPlayer => playerList[currentPlayer];
    //Message System
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;
    //Human input panel
    public delegate void ShowHumanPanel(bool activatePanel, bool activateRollDice, bool activateEndTurn, bool hasChanceJailCard, bool hasCommunityJailCard);
    public static ShowHumanPanel OnShowHumanPanel;
    //debug
    // [SerializeField] int dice1;
    // [SerializeField] int dice2;
    // public bool AllwaysDoubleRoll = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentPlayer = Random.Range(0, playerList.Count);
        gameOverPanel.SetActive(false);
        Inititialize();
        CameraSwitcher.instance.SwitchToTopDown();

        StartCoroutine(StartGame());
        OnUpdateMessage.Invoke("Welcome to <b><color=black>GeoTycoon");
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3f);
        if (playerList[currentPlayer].playerType == Player_Mono.PlayerType.AI)
        {
            //System.Threading.Thread.Sleep(2000); // Delay for 2 seconds
            //RollDice();
            RollPhysicalDice();
        }
        else
        {
            //show ui for human input
            OnShowHumanPanel.Invoke(true,true,false,false,false);
        }
    }

    void Inititialize()
    { 
        if (GameSettings.settingsList.Count == 0)
        {
            Debug.LogError("Start the game from the Main Menu!");
            return;
        }
        foreach (var setting in GameSettings.settingsList)
        {
            Player_Mono p1 = new Player_Mono();
            p1.name = setting.playerName;
            p1.playerType = (Player_Mono.PlayerType)setting.selectedType;

            playerList.Add(p1);

            GameObject infoObject = Instantiate(playerInfoPrefab, playerPanel, false);
            Player_MonoInfor info = infoObject.GetComponent<Player_MonoInfor>();
            //Debug.Log("color number" +Setting.selectColor);
            GameObject newToken = Instantiate(playerTokenList[setting.selectColor], gameBoard.route[0].transform.position, Quaternion.identity);
            p1.Inititialize(gameBoard.route[0], startMoney, info, newToken);
        }
        playerList[currentPlayer].ActivateSelector(true);

        if (playerList[currentPlayer].playerType == Player_Mono.PlayerType.HUMAN)
        {
                bool jail1 = playerList[currentPlayer].HasChanceJailFreeCard;
                bool jail2 = playerList[currentPlayer].HasCommunityJailFreeCard;
            OnShowHumanPanel.Invoke(true, true, false,jail1,jail2);
        }
        else
        {
                bool jail1 = playerList[currentPlayer].HasChanceJailFreeCard;
                bool jail2 = playerList[currentPlayer].HasCommunityJailFreeCard;
            OnShowHumanPanel.Invoke(false, false, false,jail1,jail2);
        }
    }

    public void RollPhysicalDice()
    {
        CheckForJailFree();
        rolledDice.Clear();
        _dice1.RollDice();
        _dice2.RollDice();
        CameraSwitcher.instance.SwitchToDice();

        //show or hide ui
        if (playerList[currentPlayer].playerType == Player_Mono.PlayerType.HUMAN)
        {
            bool jail1 = playerList[currentPlayer].HasChanceJailFreeCard;
            bool jail2 = playerList[currentPlayer].HasCommunityJailFreeCard;
            OnShowHumanPanel.Invoke(true,false,false,jail1,jail2);
        }
    }

    void CheckForJailFree()
    {
        //Jail free card
        if (playerList[currentPlayer].IsInjail && playerList[currentPlayer].playerType == Player_Mono.PlayerType.AI)
        {
            if(playerList[currentPlayer].HasChanceJailFreeCard)
            {
                playerList[currentPlayer].UseChanceJailFreeCard();
            }
            else if (playerList[currentPlayer].HasCommunityJailFreeCard)
            {
                playerList[currentPlayer].UseCommunityJailFreeCard();
            }
        }
    }

    public void ReportDiceRolled(int diceValue)
    {
        rolledDice.Add(diceValue);
        if(rolledDice.Count==2)
        {
            RollDice();
        }
    }

    void RollDice() //press button form human or auto from ai
    {
        bool allowedToMove = true;
        hasRolledDice = true;
        
        
        // //reset last roll
        // rolledDice = new int[2];
        // //any roll dice and store them
        // rolledDice[0] = Random.Range(1, 7);
        // rolledDice[1] = Random.Range(1, 7);
        Debug.Log("rolled dice are:" + rolledDice[0] + " & " + rolledDice[1]);


        //Debug
        // if (AllwaysDoubleRoll)
        // {
        //     rolledDice[0] = 1;
        //     rolledDice[1] = 2;
        // }
        // if (forceDiceRolls)
        // {
        //     rolledDice[0] = dice1;
        //     rolledDice[1] = dice2;
        // }
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
        
    }
    IEnumerator DelayBeforeMove(int rolledDice)
    {
        CameraSwitcher.instance.SwitchToPlayer(playerList[currentPlayer].MyTonken.transform);
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
        CameraSwitcher.instance.SwitchToTopDown();
        currentPlayer++;
        //RESET DICE HAS ROLLED
        hasRolledDice = false;
        //rolledouble?
        doubleRollCount = 0;


        //overflow check
        if(currentPlayer >= playerList.Count)
        {
            currentPlayer = 0;
        }
        DeactivateArrows();
        playerList[currentPlayer].ActivateSelector(true);
        //check if in jail


        //is player Ai
        if (playerList[currentPlayer].playerType == Player_Mono.PlayerType.AI)
        {
            //RollDice();
            RollPhysicalDice();
            OnShowHumanPanel.Invoke(false, false, false,false, false);
        }
        else  //if human - show ui
        {
            bool jail1 = playerList[currentPlayer].HasChanceJailFreeCard;
            bool jail2 = playerList[currentPlayer].HasCommunityJailFreeCard;
            OnShowHumanPanel.Invoke(true, true, false,jail1 , jail2);
        }
    }

    public List<int> LastRolledDice => rolledDice;

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

    //--------------------------GAME OVER----------------------
    public void RemovePlayer(Player_Mono player)
    {
        playerList.Remove(player);
        //CHECK FOR GAME OVER
        CheckForGameOver();
    }

    void CheckForGameOver()
    {
        if (playerList.Count == 1)
        {
            //WE HAVE A WINNER
            Debug.Log(playerList[0].name + "IS THE WINNER");
            OnUpdateMessage.Invoke(playerList[0].name + "IS THE WINNER");
            //STOP THE GAME LOOP ANYHOW

            //SHOW UI
            gameOverPanel.SetActive(true);
            winnerNameText.text = playerList[0].name;
        }
    }

    //---------------------------------UI STUFF-----------------------------------
    void DeactivateArrows()
    {
        foreach (var player in playerList)
        {
            player.ActivateSelector(false);
        }
    }
    //-------------------------------Continue Game stuff--------------------------
    public void Continue()
    {
        if(playerList.Count > 1)
        {
            Invoke("ContinueGame",SecondsBetweenTurns);
        }
        
    }
    void ContinueGame()
    {
        //if the last roll was a double
        if (RolledADouble)
        {
            //roll again
            //RollDice();
            RollPhysicalDice();
        }
        else
        {
            //Switch player
          SwitchPlayer();
            //not a double
           
        }
    }
    // Human Bankrupt
    // Declare bankrupt button
    // Sure button?
    // Game Over Screen
    public void HumanBankrupt()
    {
        playerList[currentPlayer].Bankrupt();
    }
    //-----------------JAIL FREE CARD---------------------------------
    //BUTTONS
    public void UseJail1Card()// CHANCE CARD
    {
        playerList[currentPlayer].UseChanceJailFreeCard();
    }
    public void UseJail2Card()// COMMUNITY CARD
    {
        playerList[currentPlayer].UseCommunityJailFreeCard();
    }
}
