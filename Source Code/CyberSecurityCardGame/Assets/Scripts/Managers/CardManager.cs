using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    public List<GameObject> AllActiveCards; // list of all active cards, used for layering too

    public List<AttackCardData> AttackCardOptions;
    public List<DefenseCardData> DefenseCardOptions;
    public List<ScenarioCardData> ScenarioCardOptions;

    // make these private via property later
    public ScenarioCardData currentScenario;
    public AttackCardData correctAttack;
    public DefenseCardData correctDefense;

    [SerializeField]
    AllCards AllCards;
    TableManager table; // cached reference
    public bool attackCardFound = false, defenseCardFound = false; // flags to check if correct cards found
    List<GameObject> AttackDeckCards, DefenseDeckCards;

    int scenarioIndex = 0;

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

    void Start() {
        table = TableManager.instance;
        AllActiveCards = new List<GameObject>();
        AttackDeckCards = new List<GameObject>();
        DefenseDeckCards = new List<GameObject>();
        AttackCardOptions = AllCards.AttackCards;
        DefenseCardOptions = AllCards.DefenseCards;
        ScenarioCardOptions = AllCards.ScenarioCards;
        ShuffleDecks();
        ShuffleScenarios();
        // CreateRandomScenario();
        CreateFixedScenario();
        AudioManager.instance.PlayBGM("kuia");
    }

    void ResetScenario()
    {
        attackCardFound = false;
        defenseCardFound = false;
        foreach (GameObject _card in AllActiveCards)
        {
            Destroy(_card);
        }
        AllActiveCards.Clear();
        AttackDeckCards.Clear();
        DefenseDeckCards.Clear();
        currentScenario = null;
        correctAttack = null;
        correctDefense = null;
        attackDeckIndex = 0;
        defenseDeckIndex = 0;
        ShuffleDecks();
    }

    // Fisher-Yates Shuffle implementation to shuffel the card decks;
    void ShuffleDecks()
    {
        int _rand;
        for (int x = 0; x < AttackCardOptions.Count; x++)
        {
            _rand = Random.Range(0, x);
            AttackCardData _tempAttackData = AttackCardOptions[x];
            AttackCardOptions[x] = AttackCardOptions[_rand];
            AttackCardOptions[_rand] = _tempAttackData;
        }
        for (int y = 0; y < DefenseCardOptions.Count; y++)
        {
            _rand = Random.Range(0, y);
            DefenseCardData _tempDefData = DefenseCardOptions[y];
            DefenseCardOptions[y] = DefenseCardOptions[_rand];
            DefenseCardOptions[_rand] = _tempDefData;
        }
    }

    void ShuffleScenarios()
    {
        int _rand;
        for (int y = 0; y < ScenarioCardOptions.Count; y++)
        {
            _rand = Random.Range(0, y);
            ScenarioCardData _tempSceData = ScenarioCardOptions[y];
            ScenarioCardOptions[y] = ScenarioCardOptions[_rand];
            ScenarioCardOptions[_rand] = _tempSceData;
        }
    }

    public void CreateRandomScenario()
    {
        LevelManager.instance.resetAttempts();
        
        currentScenario = AllCards.ScenarioCards[Random.Range(0, AllCards.ScenarioCards.Count)];
        correctAttack = currentScenario.CorrectAttack;
        correctDefense = currentScenario.CorrectDefense;

        SpawnCards();
    }

    public void CreateFixedScenario()
    {
        LevelManager.instance.resetAttempts();
        if (scenarioIndex >= AllCards.ScenarioCards.Count)
        {
            scenarioIndex = 0;
        }

        currentScenario = AllCards.ScenarioCards[scenarioIndex];
        correctAttack = currentScenario.CorrectAttack;
        correctDefense = currentScenario.CorrectDefense;
        scenarioIndex++;
        SpawnCards();
    }

    void SpawnCards()
    {
        // first attack card, then defense upon attack being played
        TouchHandler.instance.UpdateSingleplayerCardMask(DeckType.AttackDeck);

        // spawn and display scenario card
        GameObject scenarioCard = Instantiate(currentScenario.CardPrefab);
        AllActiveCards.Add(scenarioCard);
        scenarioCard.GetComponent<ScenarioCard>().SetData(currentScenario);
        scenarioCard.transform.SetParent(table.scenarioCardHolder.transform, false);

        // spawn 1 attack and 1 defense card
        DefenseDeckTapped();
        AttackDeckTapped();
        AudioManager.instance.PlaySfx("shuffel");
        // spawn the 4 attack and defense cards
        // for (int i = 0; i < 4; i++)
        // {
        //     GameObject attackCard = Instantiate(AttackCardOptions[i].CardPrefab);
        //     AllActiveCards.Add(attackCard);
        //     attackCard.transform.SetParent(table.attackCardsHolder[i].transform, false);
        //     attackCard.GetComponent<AttackCard>().SetData(AttackCardOptions[i]);

        //     GameObject defenseCard = Instantiate(DefenseCardOptions[i].CardPrefab);
        //     AllActiveCards.Add(defenseCard);
        //     defenseCard.transform.SetParent(table.defenseCardsHolder[i].transform, false);
        //     defenseCard.GetComponent<DefenseCard>().SetData(DefenseCardOptions[i]);
        // }

        ReLayerCards(AllActiveCards[AllActiveCards.Count-1]);
    }

    // sets the flags for the respective correct card, attack or defense
    public void CorrectCardFound(DeckType _type, GameObject _card)
    {
        AudioManager.instance.PlaySfx("correct");

        if (DeckType.AttackDeck == _type)
        {
            attackCardFound = true;
            AttackDeckCards.Remove(_card);
            LevelManager.instance.addScore(5);
            LevelManager.instance.increaseScoreStreak();
            // first attack card, then defense upon attack being played
            TouchHandler.instance.UpdateSingleplayerCardMask(DeckType.DefenseDeck);
        }
        else if (DeckType.DefenseDeck == _type)
        {
            defenseCardFound = true;
            DefenseDeckCards.Remove(_card);
            LevelManager.instance.addScore(5);
            LevelManager.instance.increaseScoreStreak();
        }

        if (attackCardFound && defenseCardFound)
        {
            Debug.Log("attack and defense card found! ");
            StartCoroutine(LoadNextScenario());
        }
    }

    public void RemoveFromDecks(GameObject _card)
    {
        AttackDeckCards.Remove(_card);
        DefenseDeckCards.Remove(_card);
    }
    public void DestoryCard(GameObject _card)
    {
        if (_card != null)
        {
            AllActiveCards.Remove(_card);
            Destroy(_card);
        }
    }

    int attackDeckIndex = 0; // starts at 1 since 0 is already spawnned
    public void AttackDeckTapped()
    {
        if (attackCardFound)
        {
            return;
        }

        // if all cards spawnned, remove existing cards on the table and reset the index
        if (attackDeckIndex >= AttackCardOptions.Count)
        {
            attackDeckIndex = 0;
        }
        foreach (GameObject _attackCard in AttackDeckCards)
        {
            AllActiveCards.Remove(_attackCard);
            Destroy(_attackCard);
        }
        AttackDeckCards.Clear();

        GameObject attackCard = Instantiate(AttackCardOptions[attackDeckIndex].CardPrefab);
        attackCard.transform.SetParent(table.attackDeckHolder.transform, false);
        attackCard.GetComponent<AttackCard>().SetData(AllCards.AttackCards[attackDeckIndex]);
        AllActiveCards.Add(attackCard);
        AttackDeckCards.Add(attackCard);
        attackDeckIndex++;
        ReLayerCards(attackCard);
    }

    int defenseDeckIndex = 0;
    public void DefenseDeckTapped()
    {
        if (defenseCardFound)
        {
            return;
        }

        // if all cards spawnned, remove existing cards on the table and reset the index
        if (defenseDeckIndex >= DefenseCardOptions.Count)
        {
            defenseDeckIndex = 0;
        }
        foreach (GameObject _defenseCard in DefenseDeckCards)
        {
            AllActiveCards.Remove(_defenseCard);
            Destroy(_defenseCard);
        }
        DefenseDeckCards.Clear();

        GameObject defenseCard = Instantiate(AllCards.DefenseCards[defenseDeckIndex].CardPrefab);
        defenseCard.transform.SetParent(table.defenseDeckHolder.transform, false);
        defenseCard.GetComponent<DefenseCard>().SetData(AllCards.DefenseCards[defenseDeckIndex]);
        AllActiveCards.Add(defenseCard);
        DefenseDeckCards.Add(defenseCard);
        defenseDeckIndex++;
        ReLayerCards(defenseCard);
    }

    public void displayCorrectCards()
    {
        StartCoroutine(showCorrectCards());
    }
    
    
    // the latest grabbed card is moved to the top most while the rest are layered in the z index according to their index in array.
    public void ReLayerCards(GameObject _card)
    {
        const float zMultiplier = -0.01f;
        Vector3 _cardPos;
        // card must exist in array before you try to re layer active card to the top
        if (AllActiveCards.Remove(_card))
        {
            AllActiveCards.Add(_card);
        }

        // after moving the card to the last index, now apply z elevation
        for (int i = 0; i < AllActiveCards.Count; i++)
        {
            _cardPos = AllActiveCards[i].transform.position;
            _cardPos.z = zMultiplier * (i+1);
            AllActiveCards[i].transform.position = _cardPos;
        }
    }

    // clears/resets data and creates a new scenario with a delay for user to visually perceive
    IEnumerator LoadNextScenario()
    {
        TouchHandler.instance.TouchIsEnabled = false; // disable touch while resetting scenario
        yield return new WaitForSeconds(1);
        ResetScenario();
        yield return new WaitForSeconds(1);
        // CreateRandomScenario();
        CreateFixedScenario();
        TouchHandler.instance.TouchIsEnabled = true; // enable touch after resetting scenario
    }

    IEnumerator showCorrectCards()
    {
        yield return new WaitForSeconds(2);
        if (!attackCardFound)
        {
            GameObject attackCard = Instantiate(correctAttack.CardPrefab);
            AllActiveCards.Add(attackCard);
            attackCard.GetComponent<AttackCard>().SetData(correctAttack);
            Vector3 _pos = table.attackPlacement.transform.position;
            _pos.z = -10;
            attackCard.transform.position = _pos;
        }
        if (!defenseCardFound)
        {
            GameObject defenseCard = Instantiate(correctDefense.CardPrefab);
            AllActiveCards.Add(defenseCard);
            defenseCard.GetComponent<DefenseCard>().SetData(correctDefense);
            Vector3 _pos = table.defensePlacement.transform.position;
            _pos.z = -10;
            defenseCard.transform.position = _pos;
        }
        yield return new WaitForSeconds(2);
        StartCoroutine(LoadNextScenario());
    }
}
