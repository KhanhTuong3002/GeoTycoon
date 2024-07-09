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
    [SerializeField] GameObject quizPanel;
    [SerializeField] TMP_Text propertyNameText;
    [SerializeField] Image colorField;
    [Space]
    [SerializeField] TMP_Text rentPriceText;//Without a house
    [SerializeField] TMP_Text oneHouseRentText;
    [SerializeField] TMP_Text twoHouseRentText;
    [SerializeField] TMP_Text threeHouseRentText;
    [SerializeField] TMP_Text fourHouseRentText;
    [SerializeField] TMP_Text hotelRentText;
    [Space]
    [SerializeField] TMP_Text housePriceText;
    [SerializeField] TMP_Text mortgagePriceText;
    [Space]
    [SerializeField] Button buyPropertyButton;
    [Space]
    [SerializeField] TMP_Text propertyPriceText;
    [SerializeField] TMP_Text playerMoneyText;
    //Message System
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;

    private bool quizAnsweredCorrectly = false;

    void OnEnable()
    {
        MonopolyNode.OnShowPropertyBuyPanel += ShowBuyPropertyUI;
        QuestionGetter.OnQuestionAnswered += HandleQuestionAnswered;
    }

    void OnDisable()
    {
        MonopolyNode.OnShowPropertyBuyPanel -= ShowBuyPropertyUI;
        QuestionGetter.OnQuestionAnswered -= HandleQuestionAnswered;
    }

    void Start()
    {
        propertyUIPanel.SetActive(false);
        quizPanel.SetActive(false);
    }

    void ShowBuyPropertyUI(MonopolyNode node, Player_Mono currentPlayer)
    {
        nodeReference = node;
        playerReference = currentPlayer;
        // Display the quiz panel
        quizPanel.SetActive(true);
        propertyUIPanel.SetActive(false);
    }

    void HandleQuestionAnswered(bool isCorrect)
    {
        quizAnsweredCorrectly = isCorrect;
        quizPanel.SetActive(false);

        if (isCorrect)
        {
            //top Panel content
            propertyNameText.text = nodeReference.name;
            //coloField.color = node.propertyColorField.color;
            //center the card
            rentPriceText.text = "$ " + nodeReference.baseRent;
            oneHouseRentText.text = "$ " + nodeReference.rentWithHouse[0];
            twoHouseRentText.text = "$ " + nodeReference.rentWithHouse[1];
            threeHouseRentText.text = "$ " + nodeReference.rentWithHouse[2];
            fourHouseRentText.text = "$ " + nodeReference.rentWithHouse[3];
            hotelRentText.text = "$ " + nodeReference.rentWithHouse[4];
            //cost of building
            housePriceText.text = "$ " + nodeReference.houseCost;
            mortgagePriceText.text = "$ " + nodeReference.MortgageValue;
            //bottom bar
            propertyPriceText.text = "Price: $ " + nodeReference.price;
            playerMoneyText.text = "You have " + playerReference.ReadMoney;
            //buy property button
            if (playerReference.CanAfford(nodeReference.price))
            {
                buyPropertyButton.interactable = true;
            }
            else
            {
                buyPropertyButton.interactable = false;
            }
            //show the panel
            propertyUIPanel.SetActive(true);
            OnUpdateMessage.Invoke("Correct answer, you now can buy current property.");
            Debug.Log("Correct answer.");
        }
        else if (!isCorrect)
        {
            // Notify incorrect answer
            OnUpdateMessage.Invoke("Incorrect answer, you cannot buy the property. End your turn.");
            Debug.Log("Incorrect answer, you cannot buy the property. End your turn.");
        }
    }

    public void BuyPropertyButton() //this call from the button
    {
        if (quizAnsweredCorrectly)
        {
            //tell the player buy
            playerReference.BuyProperty(nodeReference);
            //maybe close the property card
            //make the button not interact anymore
            buyPropertyButton.interactable = false;
        }
    }

    public void ClosePropertyButton() //this call from the button
    {
        //close the panel
        propertyUIPanel.SetActive(false);
        //clear node reference
        nodeReference = null;
        playerReference = null;
    }
}
