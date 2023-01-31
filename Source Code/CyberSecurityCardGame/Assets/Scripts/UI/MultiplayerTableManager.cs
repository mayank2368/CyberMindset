using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerTableManager : MonoBehaviour
{
    public static MultiplayerTableManager instance;
    public GameObject scenarioCardHolder, attackDeckHolder, defenseDeckHolder;
    public DefensePlacement defensePlacement;
    public AttackPlacement attackPlacement;
    public MultiplayerProfile profile1, profile2;
    // public Transform attackDeck, defenseDeck;
    public GameObject newMsgNotification;

    void Awake()
    {
        if (instance != null) 
        {
            Destroy(gameObject);
        } 
        else 
        {
            instance = this;
        }
        
    }

    private void Start() 
    {
        newMsgNotification.SetActive(false);
    }
}
