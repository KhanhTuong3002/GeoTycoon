using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;
public class UIShowUtility : MonoBehaviour
{
    MonopolyNode nodeReference;
    Player_Mono playerReference;

    [Header("Buy Utility UI")]
    [SerializeField] GameObject utilityUIPanel;
    [SerializeField] TMP_Text utilityNameText;
    [SerializeField] Image colorField;
    [Space]
    [SerializeField] TMP_Text mortgagePriceText;
    [Space]
    [SerializeField] Button buyUtilityButton;
    [Space]
    [SerializeField] TMP_Text utilityPriceText;
    [SerializeField] TMP_Text playerMoneyText;

    void OnEnable()
    {
        MonopolyNode.OnShowUtilityBuyPanel += ShowBuyUtilityUI;
        
    }

     void OnDisable()
    {
        MonopolyNode.OnShowUtilityBuyPanel -= ShowBuyUtilityUI;
    }

    void Start()
    {
        utilityUIPanel.SetActive(false);
    }

    void ShowBuyUtilityUI(MonopolyNode node, Player_Mono currentPlayer)
    {
        nodeReference = node;
        playerReference = currentPlayer;
        //top Panel content
        utilityNameText.text = node.name;
        //coloField.color = node.propertyColorField.color;
        //center the card
        //result = baseRent * (int)Mathf.Pow(2,amount-1);
        //cost of building
        mortgagePriceText.text = "$ " + node.MortgageValue;
        //bottom bar
        utilityPriceText.text = "Price: $ " + node.price;
        playerMoneyText.text = "You have" + currentPlayer.ReadMoney;
        //buy property button
        if(currentPlayer.CanAfford(node.price))
        {
            buyUtilityButton.interactable = true;
        }
        else
        {
            buyUtilityButton.interactable = false;
        }
        //show the panel
        utilityUIPanel.SetActive(true);
    }

    public void BuyUtilityButton() //this call from the button
    {
        //tell the player buy
        playerReference.BuyProperty(nodeReference);
        // maybe colse thr property card

        //make the button not interact anymore
        buyUtilityButton.interactable = false;
    }
    public void CloseUtilityButton() //this call from the button
    {
        //colse the panel
        utilityUIPanel.SetActive(false);
        //clear node reference
        nodeReference=null;
        playerReference=null;
    }
}
