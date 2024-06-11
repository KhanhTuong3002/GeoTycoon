using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum MonopolyNodeType
{
    Property,
    Utility,
    Railroad,
    Tax,
    Chance,
    Communitychest,
    Go,
    Jail,
    FreeParking,
    Gotojail
}
public class MonopolyNode : MonoBehaviour
{
    [Header("Name")]
    [SerializeField] internal new string name;
    public MonopolyNodeType monopolyNodeType;
    [SerializeField] TMP_Text nameText;
    [Header("Property Price")]
    public int price;
    [SerializeField] TMP_Text priceText;
    [Header("Property Rent")]
    [SerializeField] bool calculateRentAuto;
    [SerializeField] int currentRent;
    [SerializeField] internal int baseRent;
    [SerializeField] internal int[] rentWithHouse;
    [Header("Property Mortgage")]
    [SerializeField] GameObject morgtgageImage;
    [SerializeField] GameObject propertyImage;
    [SerializeField] bool isMortgaged;
    [SerializeField] int mortgageValue;
    [Header("Preperty Owner")]
    public Player_Mono owner;
    [SerializeField] GameObject ownerBar;
    [SerializeField] TMP_Text ownerText;

    private void OnValidate()
    {
        if(nameText != null)
        {
            nameText.text = name;
        }
        //CALCULATION
        if(calculateRentAuto)
        {
            if(monopolyNodeType == MonopolyNodeType.Property)
            {
                if(baseRent > 0) 
                {
                    price = 3 * (baseRent * 10);
                    //MORTGAGE PRICE
                    mortgageValue = price / 2;
                    rentWithHouse = new int[]
                    {
                        baseRent * 5,
                        baseRent * 5 * 3,
                        baseRent * 5 * 9,
                        baseRent * 5 * 16,
                        baseRent * 5 * 25,
                    };
                }
            }
            if (monopolyNodeType == MonopolyNodeType.Railroad)
            {
                mortgageValue = price / 2;
            }
            if (monopolyNodeType == MonopolyNodeType.Utility)
            {
                mortgageValue = price / 2;
            }
        }
        if (priceText != null)
        {
            priceText.text = "$ " + price;
        }
        //UPDATE THE OWNER
        OnOwnerUpdate();
        UnMortgageProperty();
        //isMortgaged = false;
    }
    // MONRTGAGE CONTENT
    public int MortagageProperty()
    {
        isMortgaged = true;
        if (morgtgageImage != null)
        {
            morgtgageImage.SetActive(true);
        }
        if (propertyImage != null)
        {
            propertyImage.SetActive(false);
        }
        return mortgageValue;
    }

    public void UnMortgageProperty()
    {
        isMortgaged = false;
        if(morgtgageImage != null)
        {
            morgtgageImage.SetActive(false);
        }
       if(propertyImage != null)
        {
            propertyImage.SetActive(true);
        }
    }

    public bool IsMortgaged => isMortgaged;
    public int MortgageValue => mortgageValue;

    //UPADTE OWNER
    public void OnOwnerUpdate () 
    {
        if (ownerBar != null)
        {
            if(owner.name != "")
            {
                ownerBar.SetActive(true);
                ownerText.text = owner.name;
            }
            else
            {
                ownerBar.SetActive(false);
                ownerText.text = "";
            }
        }
    }
      
    public void PlayerLandedOnNode(Player_Mono currentPlayer)
    {
        bool playerIsHuman = currentPlayer.playerType == Player_Mono.PlayerType.HUMAN;


        if(!playerIsHuman)
        {
            Invoke("ContinueGame", 2f);
        }
        else
        {
            //show UI
        }
    }

    void ContinueGame()
    {
        //if the last roll was a double
        //roll again

        //not a double
        //switch player
        GameManager.instance.SwitchPlayer();
    }
}
