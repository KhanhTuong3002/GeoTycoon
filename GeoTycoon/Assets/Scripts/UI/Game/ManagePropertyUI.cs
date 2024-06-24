using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ManagePropertyUI : MonoBehaviour
{
    [SerializeField] Transform cardHolder;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Button buyHouseButton, sellHouseButton;
    [SerializeField] TMP_Text buyHousePriceText, sellHousePriceText;

    Player_Mono playerReference;
    List<MonopolyNode> nodesInSet = new List<MonopolyNode>();
    List<GameObject> cardsInSet = new List<GameObject>();
    public void SetProperty(List<MonopolyNode> nodes, Player_Mono owner)
    {
        playerReference = owner;
        nodesInSet.AddRange(nodes);
        for (int i = 0; i < nodesInSet.Count; i++)
        {
            GameObject newCard = Instantiate(cardPrefab,cardHolder,false);
            ManageCardUI manageCardUI = newCard.GetComponent<ManageCardUI>();
            cardsInSet.Add(newCard);
            manageCardUI.SetCard(nodesInSet[i],owner,this);
        }
        var (list, allsame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(nodesInSet[0]);
        Debug.Log(allsame +"allsame");
        buyHouseButton.interactable = allsame && CheckIfBuyAllowed();
        sellHouseButton.interactable = CheckIfSellAllowed();

        buyHousePriceText.text ="- $" + nodesInSet[0].houseCost;
        sellHousePriceText.text = "+ $" + nodesInSet[0].houseCost;
    }
    public void BuyHouseButton()
    {
        if(!CheckIfBuyAllowed())
        {
            //ERROR MESSAGE
            string message = "One or moreProperties are mortgaged, you can't build a house";
            ManageUI.instance.UpdateSystemMessage(message);
            return;
        }
        if (playerReference.CanAffordHouse(nodesInSet[0].houseCost))
        {
            playerReference.BuildHouseOrHotelEvenly(nodesInSet);
            //UPDATE MONEY TEXT - IN MANAGE UI
            UpdateHouseVisulas();
            string message = "You build a house.";
            ManageUI.instance.UpdateSystemMessage(message);
        }
        else
        {
            string message = "You don't have enought money";
            ManageUI.instance.UpdateSystemMessage(message);
        }

        sellHouseButton.interactable = CheckIfSellAllowed();
        ManageUI.instance.UpdateMoneyText();
    }
    public void SellHouseButton()
    {
        playerReference.SellHouseEvenly(nodesInSet);
        //UPDATE MONEY TEXT - IN MANAGE UI
        UpdateHouseVisulas();
        string message = "You sell a house.";
            ManageUI.instance.UpdateSystemMessage(message);
        sellHouseButton.interactable = CheckIfSellAllowed();
        ManageUI.instance.UpdateMoneyText();
    }

    bool CheckIfSellAllowed()
    {
        if(nodesInSet.Any(n => n.NumberOfHouses > 0))
        {
            return true;
        }
        return false;
    }
    bool CheckIfBuyAllowed()
    {
        if(nodesInSet.Any(n => n.IsMortgaged == true))
        {
            return false;
        }
        return true;
    }
    public bool CheckIfMortgageAllowed()
    {
        if(nodesInSet.Any(n => n.NumberOfHouses > 0))
        {
            return false;
        }
        return true;
    }
    void UpdateHouseVisulas()
    {
        foreach (var card in cardsInSet)
        {
            card.GetComponent<ManageCardUI>().ShowBuildings();
        }
    }
}
