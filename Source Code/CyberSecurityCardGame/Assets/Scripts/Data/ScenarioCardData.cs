using UnityEngine;

[CreateAssetMenu(fileName = "ScenarioCard.asset", menuName = "Data/Scenario", order = 0)]
public class ScenarioCardData : ScriptableObject
{
    // unique card ID
    public int ID;
    
    // title
    public string CardName;

    // discription
    [Multiline]
    public string CardDescription;

    public AttackCardData CorrectAttack;

    public DefenseCardData CorrectDefense;

    // prefab ref
    public GameObject CardPrefab;

    // for level 1
    public string AttackHint, DefenseHint;

}
