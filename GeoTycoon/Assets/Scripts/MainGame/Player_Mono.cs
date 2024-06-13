using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    internal bool CanAfford (int price)
    {
        return price <= money;
    }

    public void BuyProperty(MonopolyNode node)
    {
        money -= node.price;
        node.SetOwner(this);
        //update UI
        myInfor.SetPlayerCash(money);
        //set ownership
        myMonopolyNodes.Add(node);
        //sort all nodes by price
        SortPropertyByPrice();
    }

    void SortPropertyByPrice()
    {
        myMonopolyNodes.OrderBy(_node => _node.price).ToList();
    }

    internal void PayRent(int rentAmount,Player_Mono owner)
    {
        //dont have enough money
        if(money < rentAmount) 
        {
          //handle insufficent funds > AI
        }
        money -= rentAmount;
        owner.CollectMoney(rentAmount);
        //Update Ui
        myInfor.SetPlayerCash(money);
    }

    internal void PayMoney(int amount)
    {
        //dont have enough money
        if (money < amount)
        {
            //handle insufficent funds > AI
        }
        money -= amount;
        //Update Ui
        myInfor.SetPlayerCash(money);
    }

    //--------------------------JAIL-------------------------------------

    public void GoToJail()
    {
        isInjail = true;
        //Reposition Player
        //myTonken.transform.position = MonopolyBoard.instance.route[8].transform.position;
        //currentnode = MonopolyBoard.instance.route[8];
        MonopolyBoard.instance.MovePlayertonken(-8, this);
    }

}
