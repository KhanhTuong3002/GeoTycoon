using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;
using JetBrains.Annotations;

public class TradingSystem : MonoBehaviour
{
    public static TradingSystem instance;

    //Message System
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;

    private void Awake()
    {
        instance = this;
    }
    //--------------------------- FIND MISSING PROPOERTY IN SET ---------------------------AI
    public void findMissingProperty(Player_Mono currentPlayer)
    {
        List<MonopolyNode> processedSet = null;
        MonopolyNode requestNode = null;
        foreach (var node in currentPlayer.GetMonopolyNodes)
        {
                var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(node);
                List<MonopolyNode> nodeSet = new List<MonopolyNode>();
                nodeSet.AddRange(list);
            //check ìf all habe been purchased
            bool notAllPurchased = list.Any(n => n.Owner == null);
            //AI owns This full set Already;
            if(allSame || processedSet == list || notAllPurchased)
            {
                processedSet = list;
                continue;
            }
            //find the owner by other player
            //buy check if we have more than avegere
            if(list.Count == 2)
            {
                requestNode = list.Find(n => n.Owner != currentPlayer && n.Owner != null);
                if(requestNode != null)
                {
                    //make offer to the owner
                    MakeTradeDecision(currentPlayer, requestNode.Owner,requestNode);
                    break;
                }
            }
            if(list.Count >= 3)
            {
                int hasMostOfSet = list.Count(n => n.Owner == currentPlayer);
                if(hasMostOfSet >= 2)
                {
                    requestNode = list.Find(n => n.Owner != currentPlayer && n.Owner != null);
                    //make offer to the owner of the node
                    MakeTradeDecision(currentPlayer, requestNode.Owner, requestNode);
                    break;
                }
            }
        }
    }
    //----------------------------- Make Trade Decision --------------------------------------
      void MakeTradeDecision(Player_Mono currntPlayer, Player_Mono nodeOwner,MonopolyNode requestedNode)
    {
        //Trade With money if Posible
        if(currntPlayer.ReadMoney >= CaulateValueOfNode(requestedNode))
        {
            //trade with money only

            //make trade offer
            MakeTradeOffer(currntPlayer, nodeOwner, requestedNode,null,CaulateValueOfNode(requestedNode),0);
            return;
        }
        //find all incomplete set and exclude the set with the request node
        foreach(var node in currntPlayer.GetMonopolyNodes)
        {
            var checkedSet = MonopolyBoard.instance.PlayerHasAllNodesOfSet(node).list;
            if(checkedSet.Contains(requestedNode))
            {
                //stop checking here
                continue;
            }
            // Valid Node Check
            if (checkedSet.Count(n => n.Owner == currntPlayer) == 1)
            {
                if(CaulateValueOfNode(node) + currntPlayer.ReadMoney >= requestedNode.price)
                {
                    int offeredMoney = CaulateValueOfNode(requestedNode) - CaulateValueOfNode(node);
                    //valid trade posible
                    MakeTradeOffer(currntPlayer, nodeOwner, requestedNode, node, offeredMoney, 0);
                    //make a trade offer
                    break;
                }
            }
        }
        //find out if only one node of the found sey is owned

        //Caculate the value of that node and see if wirh enough money it could be affordanable

        // if so .. make trade offer

    }
    //-----------------------------Make a trade offer--------------------------------
    void MakeTradeOffer(Player_Mono currentPlayer,Player_Mono nodeOwner, MonopolyNode requestedNode, MonopolyNode offeredNode, int offeredMoney, int requestedMoney)
    {
        if(nodeOwner.playerType == Player_Mono.PlayerType.AI)
        {
            ConsiderTradeOffer(currentPlayer, nodeOwner, requestedNode, offeredNode, offeredMoney, requestedMoney);
        }
        else if(nodeOwner.playerType == Player_Mono.PlayerType.HUMAN) 
        {
            //show Ui for Human
        }
    }

    //---------------------------- Consider trade Offer ----------------------------------AI
    void ConsiderTradeOffer(Player_Mono currentPlayer, Player_Mono nodeOwner, MonopolyNode requestedNode, MonopolyNode offeredNode, int offeredMoney, int requestedMoney)
    {
        int valueOfTheTrade = CaulateValueOfNode(requestedNode) + offeredMoney - requestedMoney - CaulateValueOfNode(offeredNode);
        // sell a node for money only
        if(requestedNode == null && offeredNode != null && requestedMoney < nodeOwner.ReadMoney / 3)
        {
            Trade(currentPlayer, nodeOwner, requestedNode, offeredNode, offeredMoney, requestedMoney);
            return;
        }
        // just a nomal trade
        if(valueOfTheTrade >= 0) 
        {
            //Trade the node is valid
            Trade(currentPlayer, nodeOwner, requestedNode, offeredNode, offeredMoney, requestedMoney);

        }
        else
        {
            //debug line or tell the player thet rejected
            Debug.Log("AI rejected trade offer");
        }
    }
    //---------------------------- Caculator Value Of node -------------------------------AI
     int CaulateValueOfNode(MonopolyNode requestedNode)
    {
        int value = 0;
        if(requestedNode != null)
        {
            if (requestedNode.monopolyNodeType == MonopolyNodeType.Property)
            {
                value = requestedNode.price + requestedNode.NumberOfHouses * requestedNode.houseCost;
            }
            else
            {
                value = requestedNode.price;
            }
            return value;
        }
        return value;
    }
    //--------------------------- Trade the node ----------------------------------------
    void Trade(Player_Mono currentPlayer, Player_Mono nodeOwner, MonopolyNode requestedNode, MonopolyNode offeredNode, int offeredMoney, int requestedMoney)
    {
        //CurrentPlayer needs to
        if(requestedNode !=null)
        {
            currentPlayer.PayMoney(offeredMoney);
            requestedNode.ChangeOwner(currentPlayer);
            //node owner
            nodeOwner.CollectMoney(offeredMoney);
            nodeOwner.PayMoney(requestedMoney);

            if (offeredNode != null)
            {
                offeredNode.ChangeOwner(nodeOwner);
            }
            // show the message for the ui
            string offeredNodeName = (offeredNode != null) ?" & " + offeredNode.name : "";
            OnUpdateMessage.Invoke(currentPlayer.name + " traded " + requestedNode.name + " for " + offeredMoney + offeredNodeName + " to " + nodeOwner.name);

        }
        else if(offeredNode != null && requestedNode == null)
        {
            currentPlayer.CollectMoney(requestedMoney);
            nodeOwner.PayMoney(requestedMoney);
            offeredNode.ChangeOwner(nodeOwner);
           // show the message for the ui
            OnUpdateMessage.Invoke(currentPlayer.name + " sold " + offeredNode.name + " To " + nodeOwner.name + " for " +requestedMoney);
        }        
    }
}
