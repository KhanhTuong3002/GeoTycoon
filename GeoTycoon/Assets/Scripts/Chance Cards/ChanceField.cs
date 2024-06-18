
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChanceField : MonoBehaviour
{
    [SerializeField] List<SCR_ChanceCard> cards = new List<SCR_ChanceCard> ();
    [SerializeField] TMP_Text cardText;
    [SerializeField] GameObject cardHolderBackground;
    [SerializeField] float showTime = 3; //HIDE CARD AUTOMATIC AFTER 3 Seconds
    [SerializeField] Button closeCardButton;

    List<SCR_ChanceCard> cardPool = new List<SCR_ChanceCard>();
    List<SCR_ChanceCard> usedcardPool = new List<SCR_ChanceCard>();

    SCR_ChanceCard pickedCard;
    Player_Mono currentPlayer;

    void OnEnable()
    {
        MonopolyNode.OnDrawChanceCard += DrawCard;
    }
    void OnDisable()
    {
        MonopolyNode.OnDrawChanceCard -= DrawCard;
    }

    void Start()
    {
        cardHolderBackground.SetActive(false);
        //ADD ALL CARDS TO THE POOL
        cardPool.AddRange(cards);
        //SHUFFLE THE CARDS
        ShuffleCards();
    }

    void ShuffleCards()
    {
        for (int i = 0; i < cardPool.Count; i++)
        {
            int index = Random.Range(0, cardPool.Count);
            SCR_ChanceCard tempCard = cardPool[index];
            cardPool[index] = cardPool[i];
            cardPool[i] = tempCard;
        }
        Debug.Log("chance card number: " + cardPool.Count);
    }

    void DrawCard(Player_Mono cardTaker)
    {
        //DRAW AN ACTUAL CARD
        pickedCard = cardPool[0];
        cardPool.RemoveAt(0);
        usedcardPool.Add(pickedCard);
        if(cardPool.Count == 0)
        {
            //PUT BACK ALL CARDS
            cardPool.AddRange(usedcardPool);
            usedcardPool.Clear();
            //SHUFFLE ALL
            ShuffleCards();
        }
        //WHO IS CURRENT PLAYER
        currentPlayer = cardTaker;
        //SHOWCARD
        cardHolderBackground.SetActive(true);
        //FILL IN THE TEXT
        cardText.text = pickedCard.textOnCard;
        //DEACTIVATE BUTTON IF WE ARE AN AI PLAYER
        if(currentPlayer.playerType == Player_Mono.PlayerType.AI)
        {
            closeCardButton.interactable = false;
            Invoke("ApplyCardEffect", showTime);
        }
        else
        {
            closeCardButton.interactable = true;
        }
    }
    public void ApplyCardEffect() //CLOSE BUTTON OF THE CARD
    {
        bool isMoving = false;
        if(pickedCard.rewardMoney != 0)
        {
            currentPlayer.CollectMoney(pickedCard.rewardMoney);
        }
        else if(pickedCard.penalityMoney != 0 && !pickedCard.payToPlayer)
        {
            currentPlayer.PayMoney(pickedCard.penalityMoney); //HANDLE INSUFF FUNDS
        }
        else if(pickedCard.moveToBoardIndex != -1)
        {
            isMoving = true;
            //STEP TO GOAL
            int currentIndex = MonopolyBoard.instance.route.IndexOf(currentPlayer.MyMonopolyNode);
            int lengthOfBoard = MonopolyBoard.instance.route.Count;
            int stepsToMove = 0;
            if(currentIndex < pickedCard.moveToBoardIndex)
            {
                stepsToMove = pickedCard.moveToBoardIndex-currentIndex;
            }
            else if(currentIndex > pickedCard.moveToBoardIndex)
            {
                stepsToMove = lengthOfBoard-currentIndex+pickedCard.moveToBoardIndex;
            }
            //START THE MOVE
            MonopolyBoard.instance.MovePlayertonken(stepsToMove, currentPlayer);
        }
        else if(pickedCard.payToPlayer)
        {
            int totalCollected = 0;
            List<Player_Mono> allPlayers = GameManager.instance.GetPlayers;

            foreach (var player in allPlayers)
            {
                if(player != currentPlayer)
                {
                    //PREVENT BANKRUPCY
                    int amount = Mathf.Min(currentPlayer.ReadMoney, pickedCard.penalityMoney);
                    player.CollectMoney(amount);
                    totalCollected+=amount;
                }
            }
            currentPlayer.PayMoney(totalCollected);
        }
        else if(pickedCard.streetRepair)
        {
            int[] allBuildings = currentPlayer.CountHousesAndHotels();
            int totalCosts = pickedCard.streetRepairsHousePrice * allBuildings[0] + pickedCard.streetRepairsHotelPrice * allBuildings[1];
            currentPlayer.PayMoney(totalCosts);
        }
        else if(pickedCard.goToJail)
        {
            isMoving = true;
            currentPlayer.GoToJail(MonopolyBoard.instance.route.IndexOf(currentPlayer.MyMonopolyNode));
        }
        else if(pickedCard.jailFreeCard) //JAIL FREE CARD
        {

        }
        else if(pickedCard.moveStepsBackward != 0)
        {
            int steps = Mathf.Abs(pickedCard.moveStepsBackward);
            MonopolyBoard.instance.MovePlayertonken(-steps,currentPlayer);
        }
        else if(pickedCard.nextRailRoad){
            MonopolyBoard.instance.MovePlayertonken(MonopolyNodeType.Railroad,currentPlayer);
        }
        else if(!pickedCard.nextUtility){
            MonopolyBoard.instance.MovePlayertonken(MonopolyNodeType.Utility,currentPlayer);
        }
        cardHolderBackground.SetActive(false);
        ContinueGame(isMoving);
    }
    void ContinueGame(bool isMoving)
    {
        Debug.Log(isMoving);
        if(currentPlayer.playerType == Player_Mono.PlayerType.AI)
        {
            if(!isMoving && GameManager.instance.RolledADouble)
            {
                GameManager.instance.RollDice();
            }
            else if(!isMoving && !GameManager.instance.RolledADouble)
            {
               GameManager.instance.SwitchPlayer();
            }
        }
        else //HUMAN INPUTS
        {

        }
    }
}
