using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public int Score, AttemptsLeft = 3;
    int StreakMultiplier = 1;
    int CorrectCardReward, AdditionalStreakScore; // settings to tweak for later

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

    void Start() 
    {
        Score = 0;
    }

    public void increaseScoreStreak()
    {
        StreakMultiplier++;
        TableManager.instance.updateScore(Score, StreakMultiplier);
    }

    public void addScore(int _value)
    {
        Score += _value * StreakMultiplier;
        TableManager.instance.updateScore(Score, StreakMultiplier);
        GameManager.instance.updateLevel1Highscore(Score);
    }

    // made a seperate function to implement streak multiplier properly in the previous addScore
    public void subtractScore(int _value)
    {
        Score += _value;
        if (Score < 0)
        {
            Score = 0;
        }
        TableManager.instance.updateScore(Score, StreakMultiplier);
        GameManager.instance.updateLevel1Highscore(Score);
    }
    
    public void resetAttempts()
    {
        AttemptsLeft = 3;
        TableManager.instance.setAttemptsLeftUI(AttemptsLeft);
        TableManager.instance.setHintBtnState(false);
        GameUi.instance.HideAllPanels();
    }

    public void useAttempt(DeckType _type)
    {
        AttemptsLeft--;
        StreakMultiplier = 1;
        Debug.Log("attempts "+AttemptsLeft);
        TableManager.instance.setAttemptsLeftUI(AttemptsLeft);
        TableManager.instance.setHintBtnState(false);

        if (AttemptsLeft <= 0)
        {
            Debug.Log("lost round");
            TouchHandler.instance.TouchIsEnabled = false; // disable touch to show the correct card and reset the scenario.
            CardManager.instance.displayCorrectCards();
            return;
        } 
        else if (AttemptsLeft == 1)
        {
            GameUi.instance.ShowHint(_type);
            TableManager.instance.setHintBtnState(true);
        }
    }
}
