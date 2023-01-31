using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TableManager : MonoBehaviour
{
    public static TableManager instance;
    public GameObject scenarioCardHolder, attackDeckHolder, defenseDeckHolder, hintBtn;
    public DefensePlacement defensePlacement;
    public AttackPlacement attackPlacement;
    // public Transform attackDeck, defenseDeck;
    public TextMeshPro score, playerName_txt, streak_txt; 
    public MeshRenderer mesh;
    public List<GameObject> attempts;
    public Animator hintToast;

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
        playerName_txt.SetText(GameManager.instance.Username);
        mesh.material.mainTexture = GameManager.instance.allImages.textures[GameManager.instance.data.userImg];
    }

    public void updateScore(int _score, int _streak)
    {
        if (_score < 0)
        {
            _score = 0;
        }
        score.SetText("Score: "+_score);
        streak_txt.SetText("x"+_streak);
    }

    public void setAttemptsLeftUI(int _attemptsLeft)
    {
        foreach (GameObject life in attempts)
        {
            life.SetActive(false);
        }
        for (int i = 0; i < _attemptsLeft; i++)
        {
            attempts[i].SetActive(true);
        }
    }

    public void setHintBtnState(bool _val)
    {
        hintBtn.SetActive(_val);
        hintToast.gameObject.SetActive(_val);
        
        if (_val)
        {
            hintToast.Play("hint_toast", 0, 0);
        }
    }
}
