using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class TouchHandler : MonoBehaviour
{
    public static TouchHandler instance;
    public bool TouchIsEnabled = true;
    protected Camera m_Cam;
    protected Vector2 m_Cursor;

    IDragDrop Target = null, HoverTarget = null;
    int layer_mask_card, layer_mask_deck, layer_mask_ui;

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

    void OnEnable() {
        m_Cam = Camera.main;

        LeanTouch.OnFingerUpdate += LeanFingerUpdate;
        LeanTouch.OnFingerDown += LeanFingerDown;
        LeanTouch.OnFingerUp += LeanFingerUp;
        LeanTouch.OnFingerTap += LeanFingerTap;

        layer_mask_card = LayerMask.GetMask("Card", "DefenseCard", "AttackCard");
        layer_mask_deck = LayerMask.GetMask("Deck");
        layer_mask_ui = LayerMask.GetMask("3DUI");
    }

    void OnDisable() {
        LeanTouch.OnFingerUpdate -= LeanFingerUpdate;
        LeanTouch.OnFingerDown -= LeanFingerDown;
        LeanTouch.OnFingerUp -= LeanFingerUp;
        LeanTouch.OnFingerTap -= LeanFingerTap;
    }



    void LeanFingerDown(LeanFinger touch)
    {
        if (!TouchIsEnabled || touch.IsOverGui)
        {
            return;
        }
        Vector3 pos = CheckCollision(touch.ScreenPosition);
    }

    void LeanFingerUpdate(LeanFinger touch)
    {
        if (!TouchIsEnabled || touch.IsOverGui)
        {
            return;
        }
        Vector3 pos = m_Cam.ScreenToWorldPoint(touch.ScreenPosition);
        // Debug.Log(pos);
        if (Target != null)
        {
            Target.OnDrag(pos);
        }
        else
        {
            CheckHover(touch.ScreenPosition);
        }
    }

    void LeanFingerUp(LeanFinger touch)
    {
        if (!TouchIsEnabled || touch.IsOverGui)
        {
            return;
        }
        Vector3 pos = m_Cam.ScreenToWorldPoint(touch.ScreenPosition);
        // Debug.Log(pos);
        if (Target != null)
        {
            Target.OnDrop(pos);
            Target = null;
        }
        
    }

    void LeanFingerTap(LeanFinger touch)
    {
        if (!TouchIsEnabled || touch.IsOverGui)
        {
            return;
        }
        if (HoverTarget != null)
        {
            HoverTarget.OnHover(false);
        }
        CheckIfDeck(touch.ScreenPosition);
    }

    public void ResetCard()
    {
        if (Target != null)
        {
            Target.CanBeDragged = false;
            Target = null;
        }
    }

    Vector3 CheckCollision(Vector2 _screenPoint)
    {
        Vector3 ray = m_Cam.ScreenToWorldPoint(_screenPoint);
        RaycastHit hit;
        if (Physics.Raycast(ray, Vector3.forward, out hit, 50f, layer_mask_card))
        {
            Debug.DrawRay(ray, Vector3.forward * hit.distance, Color.yellow);
            Target = hit.transform.GetComponent<IDragDrop>();
            if (Target != null && Target.CanBeDragged)
            {
                Target.OnGrab(hit.point);
            }
            else
            {
                // make null if Target.CannotBeDragged
                Target = null;
            }
            return hit.point;
        }
        else
        {
            Debug.DrawRay(ray, Vector3.forward * 50f, Color.white);
            return ray;
        }
    }

    // weridly complex logic but still fairly simple, do try to optimize, open challange :)
    void CheckHover(Vector2 _screenPoint)
    {
        Vector3 ray = m_Cam.ScreenToWorldPoint(_screenPoint);
        RaycastHit hit;
        IDragDrop _hoverTarget;
        if (Physics.Raycast(ray, Vector3.forward, out hit, 50f, layer_mask_card))
        {
            Debug.DrawRay(ray, Vector3.forward * hit.distance, Color.yellow);
            _hoverTarget = hit.transform.GetComponent<IDragDrop>();

            if (HoverTarget != null && HoverTarget != _hoverTarget)
            {
                HoverTarget.OnHover(false);
                HoverTarget = null;
                // return;
            }
            else if (_hoverTarget != null && Target == null)
            {
                HoverTarget = _hoverTarget; // cache
                HoverTarget.OnHover(true);
            }
            else if (HoverTarget != null && Target == null)
            {
                HoverTarget.OnHover(false);
                HoverTarget = null;
            }
        }
    }

    void CheckIfDeck(Vector2 _screenPoint)
    {
        Vector3 ray = m_Cam.ScreenToWorldPoint(_screenPoint);
        RaycastHit hit;
        Deck _deck;
        
        if (Physics.Raycast(ray, Vector3.forward, out hit, 60f, layer_mask_deck)) // if deck clicked
        {
            Debug.DrawRay(ray, Vector3.forward * hit.distance, Color.yellow);
            _deck = hit.transform.GetComponent<Deck>();

            // check if multiplayer or singleplayer
            if (_deck != null && CardManager.instance != null)
            {
                _deck.Tapped();
                // Debug.Log("CardManager");
            }
            else if (_deck != null && MultiplayerCardManager.instance != null)
            {
                _deck.MultiplayerTapped();
                // Debug.Log("MultiplayerCardManager");
            }
        } 
        else if (Physics.Raycast(ray, Vector3.forward, out hit, 60f, layer_mask_ui)) // if 3dUI clicked
        {
            if (GameUi.instance != null)
            {
                if (hit.transform.tag.Equals("Settings"))
                {
                    GameUi.instance.ShowSettingsPanel();
                }
                else if (hit.transform.tag.Equals("Hint"))
                {
                    GameUi.instance.ShowHintPanel();
                }
            }
            else if (MultiplayerUi.instance != null)
            {
                if (hit.transform.tag.Equals("Settings"))
                {
                    MultiplayerUi.instance.ShowSettingsPanel();
                }
                else if (hit.transform.tag.Equals("Chat"))
                {
                    MultiplayerUi.instance.ShowChatPanel();
                }
            }
        }
    }

    public void UpdateSingleplayerCardMask(DeckType _type)
    {
        if (_type == DeckType.AttackDeck)
        {
            Debug.Log("AttackDeck");

            layer_mask_card = LayerMask.GetMask("Card", "AttackCard");
        }
        else if (_type == DeckType.DefenseDeck)
        {
            Debug.Log("DefenseDeck");

            layer_mask_card = LayerMask.GetMask("Card", "DefenseCard");
        }
    }

    // to update what card a player can drag
    public void UpdateMultiplayerCardMask(DeckType _type)
    {
        layer_mask_deck = LayerMask.GetMask("Deck");

        if (_type == DeckType.AttackDeck)
        {
            // Debug.Log("AttackDeck");

            layer_mask_card = LayerMask.GetMask("Card", "AttackCard");
        }
        else if (_type == DeckType.DefenseDeck)
        {
            // Debug.Log("DefenseDeck");

            layer_mask_card = LayerMask.GetMask("Card", "DefenseCard");
        }
        else if (_type == DeckType.None)
        {
            // Debug.Log("None");
            layer_mask_card = LayerMask.GetMask("Card");
            layer_mask_deck = LayerMask.GetMask("null");
        }
    }
}
