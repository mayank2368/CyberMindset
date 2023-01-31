using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AllCards.asset", menuName = "Data/AllCardsData", order = 3)]
public class AllCards : ScriptableObject
{
    public List<ScenarioCardData> ScenarioCards;
    public List<AttackCardData> AttackCards;
    public List<DefenseCardData> DefenseCards;

    // ----------------------------------- level2 data -----------------------------------

    public List<ScenarioCardData> Level2ScenarioCards;

    // wifi
    public List<AttackCardData> WifiAttackCards;
    public List<DefenseCardData> WifiDefenseCards;

    // phone
    public List<AttackCardData> PhoneAttackCards;
    public List<DefenseCardData> PhoneDefenseCards;

    //computer
    public List<AttackCardData> ComputerAttackCards;
    public List<DefenseCardData> ComputerDefenseCards;

}
