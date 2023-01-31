using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlacement : MonoBehaviour
{
    public ParticleSystem confettiFx;
    BoxCollider BoxCollider;
    AttackCard PlacedCard = null;
    
    void OnTriggerEnter(Collider other) {
        // Debug.Log(other.gameObject);
        // PlacedCard = other.GetComponent<AttackCard>();
        if (other.GetComponent<AttackCard>() != null && PlacedCard==null && other.GetComponent<AttackCard>().CanBeDragged)
        {
            PlacedCard = other.GetComponent<AttackCard>();
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
        if (PlacedCard.data == CardManager.instance.correctAttack)
        {
            confettiFx.Play();
            PlacedCard.DisplayResultGfx(true);
            CardManager.instance.CorrectCardFound(DeckType.AttackDeck, PlacedCard.gameObject);
            // TableManager.instance.setHintBtnState(false);
            GameUi.instance.ShowHint(DeckType.DefenseDeck);
            GameUi.instance.ToastMsg("Correct Attack");
        }
        else
        {
            PlacedCard.DisplayResultGfx(false);
            CardManager.instance.RemoveFromDecks(PlacedCard.gameObject);
            GameUi.instance.ShowHint(DeckType.AttackDeck);
            GameUi.instance.ToastMsg("Wrong Attack");
            StartCoroutine(killWrongCard());
        }
    }

    // void MultiplayerCheckCard()
    // {
    //     if (PlacedCard.data == MultiplayerCardManager.instance.correctAttack)
    //     {
    //         confettiFx.Play();
    //         MultiplayerCardManager.instance.CorrectCardFound(DeckType.AttackDeck, PlacedCard.gameObject);
    //         MultiplayerManager.instance.addScore(DeckType.AttackDeck, 10);
    //     }
    //     else
    //     {
    //         MultiplayerCardManager.instance.RemoveFromDecks(PlacedCard.gameObject);
    //         MultiplayerManager.instance.subtractHealth(DeckType.AttackDeck, -5);
    //         StartCoroutine(multiplayerKillWrongCard());
    //     }
    // }

    // new funct for level 2
    void MultiplayerUpdateCardPlaced()
    {
        MultiplayerCardManager.instance.placedAttack = PlacedCard.data;
        MultiplayerCardManager.instance.UpdateCardPlaced(DeckType.AttackDeck, PlacedCard.gameObject);
    }

    IEnumerator killWrongCard()
    {
        AudioManager.instance.PlaySfx("error");
        LevelManager.instance.useAttempt(DeckType.AttackDeck);
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
