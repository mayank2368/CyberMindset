using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// used in attack and defense cards, scenario cards wont be using it for now
public interface IDragDrop
{
    // set this to false to disable drag, eg. when locked in placed after being played
    bool CanBeDragged { get; set; } 

    // when a touch input starts on a card indicating that this is to be dragged
    void OnGrab(Vector3 _pos);

    // when an object has been grabbed and is being dragged
    void OnDrag(Vector3 _pos);

    // on releasing a touch input for an object that has been grabbed
    void OnDrop(Vector3 _pos);

    // when the touch input did not start on a card but moved over a card (mostly to make the card bigger on hover)
    void OnHover(bool _value);
}
