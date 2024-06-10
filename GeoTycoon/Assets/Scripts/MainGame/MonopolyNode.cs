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

    private void OnValidate()
    {
        if(nameText != null)
        {
            nameText.text = name;
        }
    }
}
