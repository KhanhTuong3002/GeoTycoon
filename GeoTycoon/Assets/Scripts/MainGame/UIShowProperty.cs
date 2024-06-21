using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class UIShowProperty : MonoBehaviour
{
    MonopolyNode nodeReference;
    Player_Mono playerReference;

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

     void OnEnable()
    {
        MonopolyNode.OnShowPropertyBuyPanel += ShowBuyPropertyUI;
        
    }

     void OnDisable()
    {
        MonopolyNode.OnShowPropertyBuyPanel -= ShowBuyPropertyUI;
    }

    void Start()
    {
        propertyUIPanel.SetActive(false);
    }
    void ShowBuyPropertyUI(MonopolyNode node, Player_Mono currentPlayer)
    {
        nodeReference = node;
        playerReference = currentPlayer;
        //top Panel content
        propertyNameText.text = node.name;
        //coloField.color = node.propertyColorField.color;
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

    public void BuyPropertyButton() //this call from the button
    {
        //tell the player buy
        playerReference.BuyProperty(nodeReference);
        // maybe colse thr property card

        //make the button not interact anymore
        buyPropertyButton.interactable = false;
    }
    public void ClosePropertyButton() //this call from the button
    {
        //colse the panel
        propertyUIPanel.SetActive(false);
        //clear node reference
        nodeReference=null;
        playerReference=null;
    }
}
