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
    [Header("Player Info")]
    [SerializeField] GameObject playerInfoPrefab;
    [SerializeField] Transform playerPanel; //for the playerInfo Prefabs to become parented to

    //about the rolling dice
    int[] rolledDice;
    bool rolledADouble;
    int doubleRollCount;
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

    public void RollDice() //press button form human or auto from ai
    {
        //reset last roll
        rolledDice = new int[2];

        //any roll dice and store them
        rolledDice[0] = Random.Range(1, 7);
        rolledDice[1] = Random.Range(1, 7);

        //check for double
        rolledADouble = rolledDice[0] == rolledDice[1];
        //throw 3 times in a row -> jail anyhow -> end turn

        //is in jail already

        //can we leave jail

        //move anyhow if allowed

        //show or hide ui
    }
    IEnumerator DelayBeforeMove()
    {
        yield return new WaitForSeconds(2f);
    }
}
