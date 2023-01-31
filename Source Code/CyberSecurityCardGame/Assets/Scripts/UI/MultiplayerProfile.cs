using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiplayerProfile : MonoBehaviour
{
    public MeshRenderer profile_img;
    public TextMeshPro currentCardTurn, score, username, timer_txt, streak_txt;
    public List<Transform> hpBars;
    public MeshRenderer mesh;

    public string myName;
    public int timeLeft;

    public void setHealth(float _scalePercentage)
    {
        // Debug.Log(_scalePercentage);
        foreach (Transform bar in hpBars)
        {
            bar.localScale = new Vector3(_scalePercentage, bar.localScale.y, bar.localScale.z);
        }
    }

    public void setCardTurnState(DeckType _type)
    {
        if (_type == DeckType.AttackDeck)
        {
            currentCardTurn.SetText(myName+"'s Attack ");
        }
        else if (_type == DeckType.DefenseDeck)
        {
            currentCardTurn.SetText(myName+"'s Defense ");
        }
    }

    public void setScore(int _score, int _streak)
    {
        score.SetText("Score: "+_score);
        streak_txt.SetText("x"+_streak);
    }

    public void setName(string _name, int _img)
    {
        username.SetText(_name);
        myName = _name;
        // Debug.Log(" imageeee "+ _img+ " namee "+ _name);
        currentCardTurn.SetText(_name+currentCardTurn.text);

        mesh.material.mainTexture = GameManager.instance.allImages.textures[_img];
    }

    public void updateTimeLeft(int _timeLeft)
    {
        timer_txt.SetText("Time: "+_timeLeft);
    }

    public void showTimer(bool _value)
    {
        timer_txt.gameObject.SetActive(_value);
        currentCardTurn.gameObject.SetActive(_value);
    }
}
