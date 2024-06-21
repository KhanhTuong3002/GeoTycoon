using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
            manageCardUI.SetCard(nodesInSet[i],owner);
        }
        var (list, allsame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(nodesInSet[0]);
        buyHouseButton.interactable = allsame;
        sellHouseButton.interactable = allsame;
    }
    public void BuyHouseButton()
    {
        if (playerReference.CanAffordHouse(nodesInSet[0].houseCost))
        {
            playerReference.BuildHouseOrHotelEvenly(nodesInSet);
            
        }
        else
        {
            
        }
    }
    public void SellHouseButton()
    {
        playerReference.SellHouseEvenly(nodesInSet);

    }
}
