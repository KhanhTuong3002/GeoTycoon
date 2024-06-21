using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class ManageCardUI : MonoBehaviour
{
    [SerializeField] Image colorField;
    [SerializeField] GameObject[] buildings;
    [SerializeField] GameObject mortgageImage;
    [SerializeField] TMP_Text mortgageValueText;
    [SerializeField] Button mortgageButton, unMortgageButton;

    Player_Mono playerReference;
    MonopolyNode nodeReference;
// Color setColor, int  numberOfBuildings, bool isMortgaged, int mortgageValue
    public void SetCard(MonopolyNode node, Player_Mono owner)
    {
        nodeReference = node;
        playerReference = owner;
        if (node.propertyColorField != null)
        {
            colorField.color = node.propertyColorField.color;
        }
        else
        {
            colorField.color = Color.black;
        }
        if(node.NumberOfHouses<4)
        {
            for (int i = 0; i < node.NumberOfHouses; i++)
            {
                buildings[i].SetActive(true);
            }
        }
        else
        {
            buildings[4].SetActive(true);
        }
        mortgageImage.SetActive(node.IsMortgaged);
        mortgageValueText.text = "Mortgage Value = $ "+node.MortgageValue;
        mortgageButton.interactable = !node.IsMortgaged;
        unMortgageButton.interactable = node.IsMortgaged;
    }

    public void MortgageButton()
    {
        if (nodeReference.IsMortgaged)
        {
            return;
        }
        playerReference.CollectMoney(nodeReference.MortagageProperty());
        mortgageImage.SetActive(true);
        mortgageButton.interactable = false;
        unMortgageButton.interactable = true;
    }
    public void UnMortgageButton()
    {
        if (!nodeReference.IsMortgaged)
        {
            return;
        }
        playerReference.PayMoney(nodeReference.MortgageValue);
        nodeReference.UnMortgageProperty();
        mortgageImage.SetActive(false);
        unMortgageButton.interactable = false;
        mortgageButton.interactable = true;
    }
}
