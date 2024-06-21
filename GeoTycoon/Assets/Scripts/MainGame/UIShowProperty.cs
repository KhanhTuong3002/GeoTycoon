using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class UIShowProperty : MonoBehaviour
{
    MonopolyNode nodeReference;
    [Header("Buy Property UI")]
    [SerializeField] GameObject propertyUIPanel;
    [SerializeField] TMP_Text propertyNameText;
    [SerializeField] Image coloField;
    [Space]
    [SerializeField] TMP_Text rentPriceText;//Without a house
    [SerializeField] TMP_Text oneHouseRentText;
    [SerializeField] TMP_Text twoHouseRentText;
    [SerializeField] TMP_Text threeHouseRentText;
    [SerializeField] TMP_Text fourHouseRentText;
    [SerializeField] TMP_Text hotelRentText;
    [Space]
    [SerializeField] TMP_Text housePriceText;
    [SerializeField] TMP_Text hotelPriceText;
    [Space]
    [SerializeField] Button buyPropertyButton;
    [Space]
    [SerializeField] TMP_Text propertyPriceText;
    [SerializeField] TMP_Text playerMoneyText;

    void ShowBuyPropertyUI(MonopolyNode node, Player_Mono currentPlayer)
    {
        nodeReference = node;
        //top Panel content
        propertyNameText.text = node.name;
        coloField.color = node.propertyColorField.color;
        //center the card
        rentPriceText.text = "$ " + node.baseRent;
        oneHouseRentText.text = "$ " + node.rentWithHouse[0];
        twoHouseRentText.text = "$ " + node.rentWithHouse[1];
        threeHouseRentText.text = "$ " + node.rentWithHouse[2];
        fourHouseRentText.text = "$ " + node.rentWithHouse[3];
        hotelRentText.text = "$ " + node.rentWithHouse[4];
        //cost of building
        housePriceText.text = "$ " + node.houseCost;
        hotelPriceText.text = "$ " + node.houseCost;
        //bottom bar
        propertyPriceText.text = "Price: $ " + node.price;
        playerMoneyText.text = "You have" + currentPlayer.ReadMoney;
        //buy property button
        if(currentPlayer.CanAfford(node.price))
        {
            buyPropertyButton.interactable = true;
        }
        else
        {
            buyPropertyButton.interactable = false;
        }
        //show the panel
        propertyUIPanel.SetActive(true);
    }
}
