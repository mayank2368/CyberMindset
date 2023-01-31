using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public DeckType Type;

    public void Tapped()
    {
        if (Type == DeckType.AttackDeck && !CardManager.instance.attackCardFound)
        {
            AudioManager.instance.PlaySfx("draw");
            CardManager.instance.AttackDeckTapped();
        }
        else if (Type == DeckType.DefenseDeck && CardManager.instance.attackCardFound)
        {
            AudioManager.instance.PlaySfx("draw");
            CardManager.instance.DefenseDeckTapped();
        }
    }

    // seperate multiplayer code implementaion
    public void MultiplayerTapped()
    {
        if (Type == DeckType.AttackDeck && MultiplayerManager.instance.playerCanMove == DeckType.AttackDeck)
        {
            MultiplayerCardManager.instance.AttackDeckTapped();
            MultiplayerManager.instance.NetworkDeckClicked(DeckType.AttackDeck);
            AudioManager.instance.PlaySfx("draw");
        }
        else if (Type == DeckType.DefenseDeck && MultiplayerManager.instance.playerCanMove == DeckType.DefenseDeck)
        {
            MultiplayerCardManager.instance.DefenseDeckTapped();
            MultiplayerManager.instance.NetworkDeckClicked(DeckType.DefenseDeck);
            AudioManager.instance.PlaySfx("draw");
        }
    }
}

public enum DeckType
{
    AttackDeck,
    DefenseDeck,
    None,
    All
}