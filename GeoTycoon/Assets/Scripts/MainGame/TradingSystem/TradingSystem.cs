using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.Rendering;
using JetBrains.Annotations;
using UnityEngine.UI;

public class TradingSystem : MonoBehaviour
{
    public static TradingSystem instance;

    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject tradePanel;
    [Header("LEFT SIDE")]
    [SerializeField] TMP_Text leftOffererNameText;
    [SerializeField] Transform leftCardGrid;
    [SerializeField] ToggleGroup leftToggleGroup;//TO TOGGLE THE CARD SELECTION
    [SerializeField] TMP_Text leftYourMoneyText;
    [SerializeField] TMP_Text leftOfferMoney;
    [SerializeField] Slider leftMoneySlider;
    List<GameObject> leftCardPrefabList = new List<GameObject>();
    int leftChoosenMoneyAmount;
    MonopolyNode leftSelectedNode;
    Player_Mono leftPlayerReference;
    [Header("MIDDLE")]
    [SerializeField] Transform buttonGrid;
    [SerializeField] GameObject playerButtonPrefab;
    List<GameObject> playerButtonList = new List<GameObject>();
    [Header("RIGHT SIDE")]
    [SerializeField] TMP_Text rightOffererNameText;
    [SerializeField] Transform rightCardGrid;
    [SerializeField] ToggleGroup rightToggleGroup;//TO TOGGLE THE CARD SELECTION
    [SerializeField] TMP_Text rightYourMoneyText;
    [SerializeField] TMP_Text rightOfferMoney;
    [SerializeField] Slider rightMoneySlider;
    List<GameObject> rightCardPrefabList = new List<GameObject>();
    int rightChoosenMoneyAmount;
    MonopolyNode rightSelectedNode;
    Player_Mono rightPlayerReference;

    //Message System
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        tradePanel.SetActive(false);
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
                    int diffreence = CaulateValueOfNode(requestedNode) - CaulateValueOfNode(node);
                    //dif = 600 -300 > 0
                    //valid trade posible
                    if(diffreence > 0)
                    {
                        MakeTradeOffer(currntPlayer, nodeOwner, requestedNode, node, diffreence, 0);
                    }
                    else
                    {
                        MakeTradeOffer(currntPlayer, nodeOwner, requestedNode, node, 0, Mathf.Abs(diffreence));
                    }
                  
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
        int valueOfTheTrade = (CaulateValueOfNode(requestedNode) + requestedMoney) - (CaulateValueOfNode(offeredNode) + offeredMoney);
        //300 - 600 (-300) + 0 - 300 = -600
        //(300 + request 300) - (600 + 0) 
        //(600 + req0) - (300 + offer 300)
        //Watn         //        give
        //200 + 200    > 200 + 100

        // sell a node for money only
        if(requestedNode == null && offeredNode != null && requestedMoney < nodeOwner.ReadMoney / 3)
        {
            Trade(currentPlayer, nodeOwner, requestedNode, offeredNode, offeredMoney, requestedMoney);
            return;
        }
        // just a nomal trade
        if(valueOfTheTrade <= 0) 
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

    //---------------------------- USER INTERFACE CONTENT ---------------------------- HUMAN
    //---------------------------- CURRENT PLAYER ------------------------------------ HUMAN
    void CreateLeftPanel()
    {
        leftOffererNameText.text = leftPlayerReference.name;

        List<MonopolyNode> referenceNodes = leftPlayerReference.GetMonopolyNodes;
        for (int i = 0; i < referenceNodes.Count; i++)
        {
            GameObject tradeCard = Instantiate(cardPrefab, leftCardGrid,false);
            //SET UP THE ACTUAL CARD CONTENT
            tradeCard.GetComponent<TradePropertyCard>().SetTradeCard(referenceNodes[i],leftToggleGroup);

            leftCardPrefabList.Add(tradeCard);
        }
        leftYourMoneyText.text = "Your money: " + leftPlayerReference.ReadMoney;
        //SET UP THE MONEY SLIDER AND TEXT
        leftMoneySlider.maxValue = leftPlayerReference.ReadMoney;
        leftMoneySlider.value = 0;
        UpdateLeftSlider(leftMoneySlider.value);
        //leftMoneySlider.onValueChanged.AddListener(UpdateLeftSlider);
        //RESET OLD CONTENT

        tradePanel.SetActive(true);
    }

    public void UpdateLeftSlider(float value)
    {
        leftOfferMoney.text = "Offer Money: $ " + leftMoneySlider.value;
    }

    public void CloseTradePanel()
    {
        tradePanel.SetActive(false);
        ClearAll();
    }

    public void OpenTradePanel()
    {
        leftPlayerReference = GameManager.instance.GetCurrentPlayer;
        rightOffererNameText.text = "Select a Player";

        CreateLeftPanel();

        CreateMiddleButton();
    }
    //---------------------------- SELECTED PLAYER ----------------------------------- HUMAN
    public void ShowRightPlayer(Player_Mono player)
    {
        rightPlayerReference = player;
        //RESET THE CURRENT CONTENT
        ClearRightPanel();

        //SHOW RIGHT PLAYER OR ABOVE PLAYER
        rightOffererNameText.text = rightPlayerReference.name;
        List<MonopolyNode> referenceNodes = rightPlayerReference.GetMonopolyNodes;

        for (int i = 0; i < referenceNodes.Count; i++)
        {
            GameObject tradeCard = Instantiate(cardPrefab, rightCardGrid, false);
            //SET UP THE ACTUAL CARD CONTENT
            tradeCard.GetComponent<TradePropertyCard>().SetTradeCard(referenceNodes[i], rightToggleGroup);

            rightCardPrefabList.Add(tradeCard);
        }
        rightYourMoneyText.text = "Your money: " + rightPlayerReference.ReadMoney;
        //SET UP THE MONEY SLIDER AND TEXT
        rightMoneySlider.maxValue = rightPlayerReference.ReadMoney;
        rightMoneySlider.value = 0;
        UpdateRightSlider(rightMoneySlider.value);

        //UPDATE THE MOBNEY AND THE SLIDER
    }

    //SET UP MIDDLE
    void CreateMiddleButton()
    {
        //CLEAR CONTENT
        for (int i = playerButtonList.Count - 1; i >= 0; i--)
        {
            Destroy(playerButtonList[i]);
        }
        playerButtonList.Clear();

        //LOOP THROUGHT ALL PLAYER 
        List<Player_Mono> allPlayers = new List<Player_Mono>();
        allPlayers.AddRange(GameManager.instance.GetPlayers);
        allPlayers.Remove(leftPlayerReference);

        //AND THE BUTTONS FOR THEM
        foreach (var player in allPlayers)
        {
            GameObject newPlayerButton = Instantiate(playerButtonPrefab,buttonGrid,false);
            newPlayerButton.GetComponent<TradePlayerButton>().SetPlayer(player);

            playerButtonList.Add(newPlayerButton);
        }
    }

    void ClearAll()//IF WE OPEN OR CLOSE TRADE SYSTEM
    {
        rightOffererNameText.text = "Select a Player";
        rightYourMoneyText.text = "Your Money: $ 0";
        rightMoneySlider.maxValue = 0;
        rightMoneySlider.value = 0;
        UpdateRightSlider(rightMoneySlider.value);

        //CLEAR MIDDLE BUTTONS
        for (int i = playerButtonList.Count - 1; i >= 0; i--)
        {
            Destroy(playerButtonList[i]);
        }
        playerButtonList.Clear();

        //CLEAR LEFT CARD CONTENT
        for (int i = leftCardPrefabList.Count - 1; i >= 0; i--)
        {
            Destroy(leftCardPrefabList[i]);
        }
        leftCardPrefabList.Clear();

        //CLEAR RIGHT CARD CONTENT
        for (int i = rightCardPrefabList.Count - 1; i >= 0; i--)
        {
            Destroy(rightCardPrefabList[i]);
        }
        rightCardPrefabList.Clear();
    }

    void ClearRightPanel()
    {
        //CLEAR RIGHT CARD CONTENT
        for (int i = rightCardPrefabList.Count - 1; i >= 0; i--)
        {
            Destroy(rightCardPrefabList[i]);
        }
        rightCardPrefabList.Clear();
        //RESET THE SLIDER
        //SET UP THE MONEY SLIDER AND TEXT
        rightMoneySlider.maxValue = 0;
        rightMoneySlider.value = 0;
        UpdateRightSlider(rightMoneySlider.value);
    }

    public void UpdateRightSlider(float value)
    {
        rightOfferMoney.text = "Requested Money: $ " + rightMoneySlider.value;
    }

    //-----------------------------MAKE OFFER--------------------------HUMAN
    public void MakeOfferButton()//HUMAN INPUT BUTTON
    {
        MonopolyNode requestedNode = null;
        MonopolyNode offeredNode = null;
        if (rightPlayerReference == null)
        {
            //ERROR MESSAGE HERE? - NO PLAYER TO TRADE WITH

            return;
        }

        //LEFT SELECTED NODE
        Toggle offeredToggle = leftToggleGroup.ActiveToggles().FirstOrDefault();
        if (offeredToggle != null)
        {
            offeredNode = offeredToggle.GetComponentInParent<TradePropertyCard>().Node();
        }

        //RIGHT SELECTED NODE
        Toggle requestedToggle = rightToggleGroup.ActiveToggles().FirstOrDefault();
        if (requestedToggle != null)
        {
            requestedNode = requestedToggle.GetComponentInParent<TradePropertyCard>().Node();
        }

        MakeTradeOffer(leftPlayerReference,rightPlayerReference,requestedNode, offeredNode,(int)leftMoneySlider.value,(int)rightMoneySlider.value);
    }
}
