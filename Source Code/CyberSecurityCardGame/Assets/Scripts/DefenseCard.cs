using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DefenseCard : MonoBehaviour, IDragDrop
{
    public TextMeshPro Title, Description;
    public bool CanBeDragged { get; set; }
    public float HoverScaleMultiplier = 1.5f; // 50% larger on hover by default
    public DefenseCardData data; //TODO: make private later and expose property
    public GameObject correctGfx, wrongGfx;

    BoxCollider cardCollider;
    Vector3 hoverScale, originalScale, originalPos, hoverPos; // cache for scale values

    void Start()
    {
        cardCollider = GetComponent<BoxCollider>();
        CanBeDragged = true;
        originalScale = transform.localScale;
        hoverScale = transform.localScale * HoverScaleMultiplier;
        correctGfx.SetActive(false);
        wrongGfx.SetActive(false);
    }

    public void SetData(DefenseCardData _data)
    {
        data = _data;
        Title.SetText(_data.CardName);
        Description.SetText(_data.CardDescription);
    }

    public void SyncMultiplayerPos()
    {
        if (MultiplayerManager.instance != null)
        {
            MultiplayerManager.instance.UpdateCardPos(data.ID, DeckType.DefenseDeck, transform.position);
        }
    }

    public void DisplayResultGfx(bool _result)
    {
        correctGfx.SetActive(_result);
        wrongGfx.SetActive(!_result);
    }

    // --------------------------------------- IDragDrop ---------------------------------------
    public void OnGrab(Vector3 _pos)
    {
        // Debug.Log("Grabbed");
        _pos.z = -1;
        transform.position = _pos;
        transform.localScale = hoverScale;
    }

    public void OnDrag(Vector3 _pos)
    {
        // Debug.Log("OnDrag");
        _pos.z = -10;
        transform.position = _pos;
        
        SyncMultiplayerPos();
    }

    public void OnDrop(Vector3 _pos)
    {
        // Debug.Log("OnDrop");
        transform.localScale = originalScale;

        if (CardManager.instance != null)
        {
            CardManager.instance.ReLayerCards(gameObject);
        }
        else if (MultiplayerCardManager.instance != null)
        {
            MultiplayerCardManager.instance.ReLayerCards(gameObject);
        }

        SyncMultiplayerPos();
    }

    public void OnHover(bool _value)
    {
        // Debug.Log("OnHover");
        // incase this object is destoried 
        if (this == null || !CanBeDragged)
        {
            return;
        }

        originalPos = transform.position;
        hoverPos = originalPos;

        if (_value)
        {
            transform.localScale = hoverScale;
            hoverPos.z = -10;
            transform.position = hoverPos;
        }
        else
        {
            transform.localScale = originalScale;
            transform.position = originalPos;
        }
    }

}
