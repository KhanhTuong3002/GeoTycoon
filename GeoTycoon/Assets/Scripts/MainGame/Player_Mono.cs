using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class Player_Mono
{
    public enum PlayerType
    {
        HUMAN,
        AI
    }//HUMAN
    public PlayerType playerType;
    public string name;
    int money;
    MonopolyNode currentnode;
    bool isInjail;
    int numTurnsInJail;
    [SerializeField] GameObject myTonken;
    [SerializeField] List<MonopolyNode> myMonopolyNodes = new List<MonopolyNode>();

    //PLAYERINFOR
    Player_MonoInfor myInfor;


    //AI
    int aiMoneySavity = 200;


    //RETURN SOME INFORS
    public bool IsInjail => isInjail;
    public GameObject MyTonken => myTonken;
    public MonopolyNode MyMonopolyNode => currentnode;

    public void Inititialize(MonopolyNode startNode, int startMoney, Player_MonoInfor info, GameObject token)
    {
        currentnode = startNode;
        money = startMoney;
        myInfor = info;
        myInfor.SetPlayerNameandCash(name, money);
        myTonken = token;
    }

    public void SetMyCurrentNode(MonopolyNode newNode)// turn is over
    {
        currentnode = newNode;
        //Player Landed on node so lets
        newNode.PlayerLandedOnNode(this);
        // if its ai player

        // check if can build houses

        //Check for unmortgage properties

        //Check if he could trde for missing properties
    }

    public void CollectMoney(int amount)
    {
        money += amount;
        myInfor.SetPlayerCash(money);
    }
}
