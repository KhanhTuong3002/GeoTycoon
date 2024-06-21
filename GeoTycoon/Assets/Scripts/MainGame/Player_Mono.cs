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
    int numTurnsInJail = 0;
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
    public int ReadMoney => money;
    
    //Message System
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;

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
        if (playerType== PlayerType.AI){
            // check if can build houses
            CheckIfPlayerHasASet();
            //Check for unmortgage properties
            
            //Check if he could trde for missing properties
        }
        
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

    public void GoToJail(int indexOnBoard)
    {
        isInjail = true;
        //Reposition Player
        //myTonken.transform.position = MonopolyBoard.instance.route[8].transform.position;
        //currentnode = MonopolyBoard.instance.route[8];
        MonopolyBoard.instance.MovePlayertonken(CalculateDistanceFromJail(indexOnBoard), this);
        GameManager.instance.ResetRolledADouble();
    }

    public void setOutOfJail()
    {
        isInjail = false;
        //reset turn in jail
        numTurnsInJail = 0;
    }

    int CalculateDistanceFromJail(int indexOnBoard)
    {
        int result = 0;
        int indexOfjail = 8;
        if(indexOnBoard > indexOfjail)
        {
            result = (indexOnBoard - indexOfjail) * -1;
        }
        else
        {
            result = (indexOfjail - indexOnBoard);
        }
        return result;
    }


    public int NumTurnInjail => numTurnsInJail;

    public void IcreaseNumTurnInJail()
    {
        numTurnsInJail++;
    }

    //-----------------------SREET REPAIR-----------------------------------

    public int[] CountHousesAndHotels()
    {
        int houses = 0; //GOES TO INDEX 0
        int hotels = 0; //GOES TO INDEX 1

        foreach (var node in myMonopolyNodes)
        {
            if(node.NumberOfHouses!=5)
            {
                houses+= node.NumberOfHouses;
            }
            else 
            {
                hotels+=1;
            }
        }

        int[] allBuildings = new int[]{houses, hotels};
        return allBuildings;
    }

    void HandleInsufficientFund(int amountToPay){
        int houseToSell = 0;
        int allHouses = 0; 
        int propertiesToMortgage = 0;
        int allPropertiesToMortgage = 0;

        foreach (var node in myMonopolyNodes)
        {
            allHouses+=node.NumberOfHouses;

        }
        while   (money < amountToPay && allHouses>0){
            foreach (var node in myMonopolyNodes){
                houseToSell=node.NumberOfHouses;
                if      (houseToSell>0){
                    CollectMoney(node.SellHousesOrHotel());
                    allHouses--;
                    if (money>amountToPay){
                        return;
                    }
                }
            }
        }
        foreach (var node in myMonopolyNodes){
            allPropertiesToMortgage+=(node.IsMortgaged?0:1);
        }
        while   (money < amountToPay && propertiesToMortgage>0){
            foreach(var node in myMonopolyNodes){
                propertiesToMortgage=(node.IsMortgaged?0:1);
                if (propertiesToMortgage>0){

                    allPropertiesToMortgage--;
                    if (money>amountToPay){
                        return;
                    }
                }
            }
        }
    }

    void CheckIfPlayerHasASet(){
        List<MonopolyNode> processedSet = null;
        foreach (var node in myMonopolyNodes) 
        {
            Debug.Log(node.monopolyNodeType.ToString());
            var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(node);
            
            // OPEN LATER WHEN FIX PREFAB, KEEP FOR DEBUGGING

            // if (!allSame){
            //     continue;
            // }
            
            List<MonopolyNode> nodeSet = list;
            if (nodeSet!=null && nodeSet != processedSet ){
                bool hasMortgagedNode = nodeSet.Any(node => node.IsMortgaged)?true:false; 
                if (!hasMortgagedNode){
                    if (nodeSet[0].monopolyNodeType == MonopolyNodeType.Property){
                        BuildHousesOrHotelEvenly(nodeSet);
                        processedSet = nodeSet;
                    }
                }
            }
        }
    }

    void BuildHousesOrHotelEvenly(List<MonopolyNode> nodesToBuildOn){
        int minHouses = int.MaxValue;
        int maxHouses = int.MinValue;

        foreach (var node in nodesToBuildOn){
            int numberOfHouses = node.NumberOfHouses;
            if (numberOfHouses < minHouses){
                minHouses = numberOfHouses;
            }
            if (numberOfHouses > maxHouses){
                maxHouses = numberOfHouses;
            }
        }

        foreach (var node in nodesToBuildOn){
            if (node.NumberOfHouses==minHouses && node.NumberOfHouses<5 && CanAffordHouse(node.houseCost)){
                node.BuyHousesOrHotel();
                PayMoney(node.houseCost);
                break;
            }
        }
    }

    bool CanAffordHouse(int price){
        if (playerType==PlayerType.AI){
            return (money-aiMoneySavity) >= price;
        }
        return money >= price;
    }
}
