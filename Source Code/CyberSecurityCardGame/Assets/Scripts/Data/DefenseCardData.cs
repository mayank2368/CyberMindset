using UnityEngine;

[CreateAssetMenu(fileName = "DefenseCard.asset", menuName = "Data/Defense", order = 2)]
public class DefenseCardData : ScriptableObject
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
}
