using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public MonopolyNodeType monopolyNodeType;
    public Image propertyColorField;
    [Header("Property Name")]
    [SerializeField] internal new string name;
    [SerializeField] TMP_Text nameText;
    [Header("Property Price")]
    public int price;
    [SerializeField] TMP_Text priceText;
    [Header("Property Rent")]
    [SerializeField] bool calculateRentAuto;
    [SerializeField] int currentRent;
    [SerializeField] internal int baseRent;
    [SerializeField] internal List<int> rentWithHouse = new List<int>();
    int numberOfHouse;
    [Header("Property Mortgage")]
    [SerializeField] GameObject morgtgageImage;
    [SerializeField] GameObject propertyImage;
    [SerializeField] bool isMortgaged;
    [SerializeField] int mortgageValue;
    [Header("Preperty Owner")]
    [SerializeField] GameObject ownerBar;
    [SerializeField] TMP_Text ownerText;
    public Player_Mono owner;

    public Player_Mono Owner => owner;
    public void SetOwner(Player_Mono newOwner)
    {
        owner = newOwner;
    }


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
                    rentWithHouse.Clear();                  
                        rentWithHouse.Add(baseRent * 5);
                        rentWithHouse.Add(baseRent * 5 * 3);
                        rentWithHouse.Add(baseRent * 5 * 9);
                        rentWithHouse.Add(baseRent * 5 * 16);
                        rentWithHouse.Add(baseRent * 5 * 25);    
                }
                else if(baseRent <= 0) 
                {
                    price = 0;
                    baseRent = 0;
                    rentWithHouse.Clear();
                    mortgageValue = 0;
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

    public void UpdateColorField(Color color)
    {
        if(propertyColorField != null)
        {
            propertyColorField.color = color;
        }     
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
        bool continueTurn = true;

        //Check For node type and atc

        switch (monopolyNodeType)
        {
            case MonopolyNodeType.Property:
                if (!playerIsHuman)//Ai
                {
                    //If it owned && if we not are owner && is not mortgaged
                    if (owner.name != "" && owner != currentPlayer && !isMortgaged)
                    {
                        //pay rent to somebody

                        //caculate the  rent
                        Debug.Log("PLAYER MIGHT PAY RETN && OWNER SHIP IS : " + owner.name);
                        int renToPay = CalculatePropertyRent();
                        //pay the rent to the owner
                        currentPlayer.PayRent(renToPay, owner);


                        //show a message about what happend
                        Debug.Log(currentPlayer.name + "pay ren of: " + renToPay + " to " + owner.name);
                    }
                    else if (owner.name == "" && currentPlayer.CanAfford(price))
                        {
                        //buy the node
                        Debug.Log("PLAYER COULD BUY");
                        currentPlayer.BuyProperty(this);
                        OnOwnerUpdate();

                          //show a message about what happend 
                        }
                    else
                    {
                        //Is unowned and we cant afford it
                    }

        }               
                else //Human
                {
                    //If it owned && if we not are owner && is not mortgaged
                    if (owner.name != "" && owner != currentPlayer && !isMortgaged)
                    {
                        //pay rent to somebody

                        //caculate the  rent

                        //pay the rent to the owner

                        //show a message about what happend 
                    }
                    else if (owner.name == "")
                    {
                        //Show buy interface for the property


                    }
                    else
                    {
                        //Is unowned and we cant afford it
                    }
                }
                break;
            case MonopolyNodeType.Utility:
                if (!playerIsHuman)//Ai
                {
                    //If it owned && if we not are owner && is not mortgaged
                    if (owner.name != "" && owner != currentPlayer && !isMortgaged)
                    {
                        //pay rent to somebody

                        //caculate the  rent
                        int renToPay = CalculateUtilityRent();
                        //pay the rent to the owner
                        currentPlayer.PayRent(renToPay, owner);


                        //show a message about what happend
                        Debug.Log(currentPlayer.name + "pay ren of: " + renToPay + " to " + owner.name);
                    }
                    else if (owner.name == "" && currentPlayer.CanAfford(price))
                    {
                        //buy the node
                        Debug.Log("PLAYER COULD BUY");
                        currentPlayer.BuyProperty(this);
                        OnOwnerUpdate();

                        //show a message about what happend 
                    }
                    else
                    {
                        //Is unowned and we cant afford it
                    }

                }
                else //Human
                {
                    //If it owned && if we not are owner && is not mortgaged
                    if (owner.name != "" && owner != currentPlayer && !isMortgaged)
                    {
                        //pay rent to somebody

                        //caculate the  rent

                        //pay the rent to the owner

                        //show a message about what happend 
                    }
                    else if (owner.name == "")
                    {
                        //Show buy interface for the property


                    }
                    else
                    {
                        //Is unowned and we cant afford it
                    }
                }


                break;
            case MonopolyNodeType.Railroad:
                if (!playerIsHuman)//Ai
                {
                    //If it owned && if we not are owner && is not mortgaged
                    if (owner.name != "" && owner != currentPlayer && !isMortgaged)
                    {
                        //pay rent to somebody

                        //caculate the  rent
                        Debug.Log("PLAYER MIGHT PAY RETN && OWNER SHIP IS : " + owner.name);
                        int renToPay = CalculateRailroadRent();
                        currentRent = renToPay;
                        //pay the rent to the owner
                        currentPlayer.PayRent(renToPay, owner);


                        //show a message about what happend
                        Debug.Log(currentPlayer.name + "pay ren of: " + renToPay + " to " + owner.name);
                    }
                    else if (owner.name == "" && currentPlayer.CanAfford(price))
                    {
                        //buy the node
                        Debug.Log("PLAYER COULD BUY");
                        currentPlayer.BuyProperty(this);
                        OnOwnerUpdate();

                        //show a message about what happend 
                    }
                    else
                    {
                        //Is unowned and we cant afford it
                    }

                }
                else //Human
                {
                    //If it owned && if we not are owner && is not mortgaged
                    if (owner.name != "" && owner != currentPlayer && !isMortgaged)
                    {
                        //pay rent to somebody

                        //caculate the  rent

                        //pay the rent to the owner

                        //show a message about what happend 
                    }
                    else if (owner.name == "")
                    {
                        //Show buy interface for the property


                    }
                    else
                    {
                        //Is unowned and we cant afford it
                    }
                }


                break;
            case MonopolyNodeType.Tax:
                GameManager.instance.AddTaxToPool(price);
                currentPlayer.PayMoney(price);
                //show a message about what happend 
                break;

            case MonopolyNodeType.FreeParking:
                int tax = GameManager.instance.GetTaxPool();
                currentPlayer.CollectMoney(tax);
                //show a message about what happend 

                break;
            case MonopolyNodeType.Gotojail:
                System.Threading.Thread.Sleep(1000); // Delay for 2 seconds
                int indexOnBoard = MonopolyBoard.instance.route.IndexOf(currentPlayer.MyMonopolyNode);
                currentPlayer.GoToJail(indexOnBoard);
                continueTurn = false;
                break;
            case MonopolyNodeType.Chance:



                break;
            case MonopolyNodeType.Communitychest:



                break;
        }
        //stop here if needed
        if (!continueTurn)
        {
            return;
        }


        //Continue
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

    int CalculatePropertyRent()
    {
        switch (numberOfHouse)
        {
            case 0:
                // Check if owner has the full set of this nodes
                var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(this);

                if (allSame)
                {
                    currentRent = baseRent * 2;
                }
                else
                {
                    currentRent = baseRent;
                }

                break;

            case 1:
                currentRent = rentWithHouse[0];
                break;

            case 2:
                currentRent = rentWithHouse[1];
                break;

            case 3: //hotel
                currentRent = rentWithHouse[2];
                break;

            case 4:
                currentRent = rentWithHouse[3];
                break;

            case 5: //hotel
                currentRent = rentWithHouse[4];
                break;

        }

        return currentRent; 
    }

    int CalculateUtilityRent()
    {
        int[] lastRolledDice = GameManager.instance.LastRolledDice;

        int result = 0;
        var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(this);
        if(allSame)
        {
            result = (lastRolledDice[0] + lastRolledDice[1]) * 10;
        }
        else
        {
            result = (lastRolledDice[0] + lastRolledDice[1]) * 4;
        }
        return result;
    }
    int CalculateRailroadRent()
    {
        int[] lastRolledDice = GameManager.instance.LastRolledDice;

        int result = 0;
        var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(this);
        int amount = 0;
        foreach (var item in list)
        {
            amount += (item.owner == this.owner) ? 1 : 0;
        }
        Debug.Log(list.Count);
        result = baseRent * (int)Mathf.Pow(2,amount-1);
        
        return result;
    }
}
