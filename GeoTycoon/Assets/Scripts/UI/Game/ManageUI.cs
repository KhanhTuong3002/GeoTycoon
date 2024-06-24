using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class ManageUI : MonoBehaviour
{
    public static ManageUI instance;

    [SerializeField] GameObject managePanel; //TO SHOW AND HIDE 
    [SerializeField] Transform propertyGrid; //YO PARENT PROPERTY SETS TO IT
    [SerializeField] GameObject propertySetPrefab; //
    Player_Mono playerReference;
    List<GameObject> propertyPrefabs = new List<GameObject>();
    [SerializeField] TMP_Text yourMoneyText;
    [SerializeField] TMP_Text systemMessageText;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        managePanel.SetActive(false);
    }
    public void OpenManager() //CALL FROM BUTTON
    {
        playerReference = GameManager.instance.GetCurrentPlayer;
        CreateProperties();
        managePanel.SetActive(true);
        UpdateMoneyText();
        //COMOPARE IF OWNER IS PLAYER REF

        //FILL PROPERTY SETS AND CREATE AS MUCH AS NEEDED
    }

    public void CloseManager()
    {
        managePanel.SetActive(false);
        ClearProperties();
    }

    void ClearProperties()
    {
        for (int i = propertyPrefabs.Count-1; i >= 0; i--)
        {
            Destroy(propertyPrefabs[i]);

        }
        propertyPrefabs.Clear();
    }

    void CreateProperties()
    {
        //GET ALL NODES AS NODE SETS
        List<MonopolyNode> processedSet = null;

        foreach (var node in playerReference.GetMonopolyNodes)
        {
            var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(node);
            List<MonopolyNode> nodeSet = list;//new List<MonopolyNode>();
            //nodeSet.AddRange(list);

            if(nodeSet != null && nodeSet != processedSet)
            {
                //UPDATE PROCESSED FIRST
                processedSet = nodeSet;

                nodeSet.RemoveAll(n => n.Owner != playerReference);

                //CREATE PREFAB WITH ALL NODES OWNED BY THE PLAYER
                GameObject newPropertySet = Instantiate(propertySetPrefab,propertyGrid,false);
                newPropertySet.GetComponent<ManagePropertyUI>().SetProperty(nodeSet,playerReference);

                propertyPrefabs.Add(newPropertySet);
            }
        }
    }

    public void UpdateMoneyText()
    {
        string showMoney = (playerReference.ReadMoney >= 0)? "<color=green>$" +playerReference.ReadMoney: "<color=red>$" +playerReference.ReadMoney;
        yourMoneyText.text = "<color=black>Your Money:</color> " + showMoney;
    }

    public void UpdateSystemMessage(string message)
    {
        systemMessageText.text = message;
    }

    public void AutoHandleFunds()//CALL FROM BUTTON
    {
        if(playerReference.ReadMoney>0)
        {
            UpdateSystemMessage("You don't need to do that, you have enough money!");
            return;
        }
        playerReference.HandleInsufficientFunds(Mathf.Abs(playerReference.ReadMoney));
        //UPDATE THE UI
        ClearProperties();
        CreateProperties();
        //UPDATE SYSTEM MESSAGE
        UpdateMoneyText();
    }

}
