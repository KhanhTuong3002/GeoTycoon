using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class ManageCardUI : MonoBehaviour
{
    [SerializeField] Image colorField;
    [SerializeField] TMP_Text propertyNameText;
    [SerializeField] GameObject[] buildings;
    [SerializeField] GameObject mortgageImage;
    [SerializeField] TMP_Text mortgageValueText;
    [SerializeField] Button mortgageButton, unMortgageButton;

    [SerializeField] Image iconImage;
    [SerializeField] Sprite houseSprite, railroadSprite, utilitySprite;


    Player_Mono playerReference;
    MonopolyNode nodeReference;
    ManagePropertyUI propertyReference;
// Color setColor, int  numberOfBuildings, bool isMortgaged, int mortgageValue
    public void SetCard(MonopolyNode node, Player_Mono owner, ManagePropertyUI SetProperty)
    {
        nodeReference = node;
        playerReference = owner;
        propertyReference = SetProperty;
        if (node.propertyColorField != null)
        {
            colorField.color = node.propertyColorField.color;
        }
        else
        {
            colorField.color = Color.black;
        }
        //SHOW HOUSES
        ShowBuildings();
        mortgageImage.SetActive(node.IsMortgaged);
        mortgageValueText.text = "Mortgage Value <br><b>$ "+node.MortgageValue;
        mortgageButton.interactable = !node.IsMortgaged;
        unMortgageButton.interactable = node.IsMortgaged;
        //SET ICON
        switch(nodeReference.monopolyNodeType)
        {
            case MonopolyNodeType.Property:
            iconImage.sprite = houseSprite;
            iconImage.color = Color.blue;
            break;
            case MonopolyNodeType.Railroad:
            iconImage.sprite = railroadSprite;
            iconImage.color = Color.white;
            break;
            case MonopolyNodeType.Utility:
            iconImage.sprite = utilitySprite;
            iconImage.color = Color.black;
            break;
        }
        //SET NAME OF PROPERTY
        propertyNameText.text = nodeReference.name;

    }

    public void MortgageButton()
    {
        if(!propertyReference.CheckIfMortgageAllowed())
        {
            //ERROR MESSAGE
            string message = "You have houses on one or more properties, you can't mortgage!";
            ManageUI.instance.UpdateSystemMessage(message);
            return;
        }
        if (nodeReference.IsMortgaged)
        {
            string message = "It's mortgaged already!";
            ManageUI.instance.UpdateSystemMessage(message);
            return;
        }
        playerReference.CollectMoney(nodeReference.MortagageProperty());
        mortgageImage.SetActive(true);
        mortgageButton.interactable = false;
        unMortgageButton.interactable = true;
        ManageUI.instance.UpdateMoneyText();
    }
    public void UnMortgageButton()
    {
        if (!nodeReference.IsMortgaged)
        {
            //ERROR MESSAGE OR SUCH
            string message = "It's unmortgaged already!";
            ManageUI.instance.UpdateSystemMessage(message);
            return;
        }
        if(playerReference.ReadMoney < nodeReference.MortgageValue)
        {
            //ERROR MESSAGE OR SUCH
            string message = "You don't have enough money!";
            ManageUI.instance.UpdateSystemMessage(message);
            return;
        }
        playerReference.PayMoney(nodeReference.MortgageValue);
        nodeReference.UnMortgageProperty();
        mortgageImage.SetActive(false);
        unMortgageButton.interactable = false;
        mortgageButton.interactable = true;
        ManageUI.instance.UpdateMoneyText();
    }

    public void ShowBuildings()
    {
        //HIDE ALL BUILDINGS FIRST
        foreach (var icon in buildings)
        {
            icon.SetActive(false);
        }
        //SHOW BUILDINGS
        if(nodeReference.NumberOfHouses < 5)
        {
            for (int i = 0; i < nodeReference.NumberOfHouses; i++)
            {
                buildings[i].SetActive(true);
            }
        }
        else
        {
            buildings[4].SetActive(true);
        }
    }
}
