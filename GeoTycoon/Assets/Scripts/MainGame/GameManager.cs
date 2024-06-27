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
    public void ResetRolledADouble() => rolledADouble=false;
    int doubleRollCount;
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
    public delegate void ShowHumanPanel(bool activatePanel, bool activateRollDice, bool activateEndTurn);
    public static ShowHumanPanel OnShowHumanPanel;
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
            System.Threading.Thread.Sleep(2000); // Delay for 2 seconds
            RollDice();
        }
        else
        {
            //show ui for human input
        }
    }

    void Inititialize()
    {
        // Đảm bảo rằng playerTokenList không phải là null và có ít nhất một phần tử
        if (playerTokenList == null || playerTokenList.Count == 0)
        {
            Debug.LogError("Player token list is null or empty!");
            return;
        }

        // Tạo bản sao của danh sách token để tránh thao tác trực tiếp trên SerializedProperty
        List<GameObject> tempTokenList = new List<GameObject>(playerTokenList);

        // Đảm bảo rằng tempTokenList không phải là null và có ít nhất một phần tử
        if (tempTokenList == null || tempTokenList.Count == 0)
        {
            Debug.LogError("Temporary token list is null or empty after copying!");
            return;
        }

        // Khởi tạo tất cả các player
        for (int i = 0; i < playerList.Count; i++)
        {
            GameObject infoObject = Instantiate(playerInfoPrefab, playerPanel, false);
            Player_MonoInfor info = infoObject.GetComponent<Player_MonoInfor>();

            // Random token từ danh sách tạm thời
            int randomIndex = Random.Range(0, tempTokenList.Count);

            // Đảm bảo rằng randomIndex là hợp lệ
            if (randomIndex < 0 || randomIndex >= tempTokenList.Count)
            {
                Debug.LogError("Random index is out of range!");
                return;
            }

            // Instantiate the token
            GameObject newToken = Instantiate(tempTokenList[randomIndex], gameBoard.route[0].transform.position, Quaternion.identity);

            // Initialize player
            playerList[i].Inititialize(gameBoard.route[0], startMoney, info, newToken);

            // Remove the used token from the temporary list
            tempTokenList.RemoveAt(randomIndex);

            // Đảm bảo rằng tempTokenList không rỗng sau mỗi lần loại bỏ
            if (tempTokenList.Count == 0 && i < playerList.Count - 1)
            {
                Debug.LogError("Not enough tokens for all players!");
                return;
            }
        }
        playerList[currentPlayer].ActivateSelector(true);

        if (playerList[currentPlayer].playerType == Player_Mono.PlayerType.HUMAN)
        {
            OnShowHumanPanel.Invoke(true, true, false);
        }
        else
        {
            OnShowHumanPanel.Invoke(false, false, false);
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
            rolledDice[0] = 4;
            rolledDice[1] = 4;
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
        if (playerList[currentPlayer].playerType == Player_Mono.PlayerType.HUMAN)
        {
            OnShowHumanPanel.Invoke(true,false,false);
        }
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
        DeactivateArrows();
        playerList[currentPlayer].ActivateSelector(true);
        //check if in jail


        //is player Ai
        if (playerList[currentPlayer].playerType == Player_Mono.PlayerType.AI)
        {
            RollDice();
            OnShowHumanPanel.Invoke(false, false, false);
        }
        else  //if human - show ui
        {
            OnShowHumanPanel.Invoke(true, true, false);
        }
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
}
