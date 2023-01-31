using UnityEngine;

[CreateAssetMenu(fileName = "AttackCard.asset", menuName = "Data/Atack", order = 1)]
public class AttackCardData : ScriptableObject
{
    // unique card ID
    public int ID;
    
    // title
    public string CardName;

    // discription
    [Multiline]
    public string CardDescription;

    // prefab ref
    public GameObject CardPrefab;

    public DefenseCardData correctDefense;
}
