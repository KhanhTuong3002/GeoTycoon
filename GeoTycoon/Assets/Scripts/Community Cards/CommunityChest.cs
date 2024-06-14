using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CommunityChest : MonoBehaviour
{
    [SerializeField] List<SCR_CommunityCard> cards = new List<SCR_CommunityCard> ();
    [SerializeField] TMP_Text cardText;
    [SerializeField] GameObject cardHolderBackground;
    [SerializeField] float showTime = 3; //HIDE CARD AUTOMATIC AFTER 3 Seconds
    [SerializeField] float moveDelay = 0.5f; //MAYBE NOT USED LATER

    List<SCR_CommunityCard> cardPool = new List<SCR_CommunityCard>();

    //CURRENT CARD AND CURRENT PLAYER
    SCR_CommunityCard pickedCard;
    Player_Mono currentPlayer;
    void OnEnable()
    {
        MonopolyNode.OnDrawCommunityCard += DrawCard;
    }
    void OnDisable()
    {
        MonopolyNode.OnDrawCommunityCard -= DrawCard;
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
            SCR_CommunityCard tempCard = cardPool[index];
            cardPool[index] = cardPool[i];
            cardPool[i] = tempCard;
        }
    }

    void DrawCard(Player_Mono cardTaker)
    {
        //DRAW AN ACTUAL CARD
        pickedCard = cardPool[0];
        //WHO IS CURRENT PLAYER
        currentPlayer = cardTaker;
        //SHOWCARD
        cardHolderBackground.SetActive(true);
        //FILL IN THE TEXT
        cardText.text = pickedCard.textOnCard;
        //APPLY THE EFFECTS BASED ON THE CARD WE DRAW 
    }

    public void ApplyCardEffect() //CLOSE BUTTON OF THE CARD
    {
        bool isMoving = false;
        if(pickedCard.rewardMoney != 0 && !pickedCard.collectFromPlayer)
        {
            currentPlayer.CollectMoney(pickedCard.rewardMoney);
        }
        else if(pickedCard.penalityMoney != 0)
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
        else if(pickedCard.collectFromPlayer)
        {
            int totalCollected = 0;
            List<Player_Mono> allPlayers = GameManager.instance.GetPlayers;

            foreach (var player in allPlayers)
            {
                if(player != currentPlayer)
                {
                    //PREVENT BANKRUPCY
                    int amount = Mathf.Min(player.ReadMoney, pickedCard.rewardMoney);
                    player.PayMoney(amount);
                    totalCollected+=amount;
                }
            }
            currentPlayer.CollectMoney(totalCollected);
        }
        else if(pickedCard.streetRepair)
        {

        }
        else if(pickedCard.goToJail)
        {
            isMoving = true;
            currentPlayer.GoToJail(MonopolyBoard.instance.route.IndexOf(currentPlayer.MyMonopolyNode));
        }
        else if(pickedCard.jailFreeCard)
        {

        }
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
        else
        {

        }
    }
}
