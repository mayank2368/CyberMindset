using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensePlacement : MonoBehaviour
{
    public ParticleSystem confettiFx;
    BoxCollider BoxCollider;
    DefenseCard PlacedCard = null;

    void OnTriggerEnter(Collider other) {
        // Debug.Log(other.gameObject);
        // PlacedCard = other.GetComponent<DefenseCard>();
        if (other.GetComponent<DefenseCard>() != null && PlacedCard==null && other.GetComponent<DefenseCard>().CanBeDragged)
        {
            PlacedCard = other.GetComponent<DefenseCard>();
            SnapCard();
        }
    }

    // temporary, will add tweens later
    void SnapCard()
    {
        // Debug.Log("SnapCard");
        Vector3 _pos = transform.position;
        _pos.z = transform.position.z - 0.005f;
        PlacedCard.transform.position = _pos;
        PlacedCard.CanBeDragged = false;
        
        if (CardManager.instance != null)
        {
            CheckCard();
        }
        else if (MultiplayerCardManager.instance != null)
        {
            MultiplayerUpdateCardPlaced();
        }
    }

    void CheckCard()
    {
        if (PlacedCard.data == CardManager.instance.correctDefense)
        {
            confettiFx.Play();
            PlacedCard.DisplayResultGfx(true);
            CardManager.instance.CorrectCardFound(DeckType.DefenseDeck, PlacedCard.gameObject);
            // TableManager.instance.setHintBtnState(false);
            GameUi.instance.ShowHint(DeckType.AttackDeck);
            GameUi.instance.ToastMsg("Correct Defense");
        }
        else
        {
            PlacedCard.DisplayResultGfx(false);
            CardManager.instance.RemoveFromDecks(PlacedCard.gameObject);
            StartCoroutine(killWrongCard());
            GameUi.instance.ShowHint(DeckType.DefenseDeck);
            GameUi.instance.ToastMsg("Wrong Defense");
        }
    }

    // void MultiplayerCheckCard()
    // {
    //     if (PlacedCard.data == MultiplayerCardManager.instance.correctDefense)
    //     {
    //         confettiFx.Play();
    //         MultiplayerCardManager.instance.CorrectCardFound(DeckType.DefenseDeck, PlacedCard.gameObject);
    //         MultiplayerManager.instance.addScore(DeckType.DefenseDeck, 10);
    //     }
    //     else
    //     {
    //         MultiplayerCardManager.instance.RemoveFromDecks(PlacedCard.gameObject);
    //         MultiplayerManager.instance.subtractHealth(DeckType.DefenseDeck, -5);
    //         StartCoroutine(multiplayerKillWrongCard());
    //     }
    // }

    void MultiplayerUpdateCardPlaced()
    {
        MultiplayerCardManager.instance.placedDefense = PlacedCard.data;
        MultiplayerCardManager.instance.UpdateCardPlaced(DeckType.DefenseDeck, PlacedCard.gameObject);
    }

    IEnumerator killWrongCard()
    {
        AudioManager.instance.PlaySfx("error");
        LevelManager.instance.useAttempt(DeckType.DefenseDeck);
        LevelManager.instance.subtractScore(-2);
        yield return new WaitForSeconds(2);
        CardManager.instance.DestoryCard(PlacedCard.gameObject);
        PlacedCard = null;
    }

    //unused
    IEnumerator multiplayerKillWrongCard()
    {
        AudioManager.instance.PlaySfx("error");
        yield return new WaitForSeconds(2);
        MultiplayerCardManager.instance.DestoryCard(PlacedCard.gameObject);
        PlacedCard = null;
    }
}
