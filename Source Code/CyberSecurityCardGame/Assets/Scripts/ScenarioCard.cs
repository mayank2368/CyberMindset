using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScenarioCard : MonoBehaviour, IDragDrop
{
    public TextMeshPro Title, Description;
    public ScenarioCardData data; //TODO: make private later and expose property
    public float HoverScaleMultiplier = 1.5f; // 50% larger on hover by default

    BoxCollider cardCollider;
    Vector3 hoverScale, originalScale; // cache for scale values

    public bool CanBeDragged { get; set; }

    void Start()
    {
        cardCollider = GetComponent<BoxCollider>();
        CanBeDragged = false;
        originalScale = transform.localScale;
        hoverScale = transform.localScale * HoverScaleMultiplier;
    }

    public void SetData(ScenarioCardData _data)
    {
        data = _data;
        Title.SetText(_data.CardName);
        Description.SetText(_data.CardDescription);
    }

    public void OnGrab(Vector3 _pos)
    {
        
    }

    public void OnDrag(Vector3 _pos)
    {
        
    }

    public void OnDrop(Vector3 _pos)
    {
        
    }

    public void OnHover(bool _value)
    {
        Debug.Log("OnHover");
        // incase this object is destoried 
        if (this == null)
        {
            return;
        }

        if (_value)
        {
            transform.localScale = hoverScale;
        }
        else
        {
            transform.localScale = originalScale;
        }
    }
}
