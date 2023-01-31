using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MPLoadingPanel : MonoBehaviour
{
    public TextMeshProUGUI waiting_txt, player1letter, player2letter;
    public Image profile1Img, profile2Img;

    int textIndex; 
    List<string> waitingTextList;

    public void SetLoadingText(string _msg)
    {
        waiting_txt.SetText(_msg);
    }
    
    public void GameStartingIn(int _sec)
    {
        waiting_txt.SetText("All players connected, starting in "+_sec);
    }

    public void setPlayerData(string _name, int _id)
    {
        if (_id == 1)
        {
            player1letter.SetText(_name);
            player1letter.transform.parent.gameObject.SetActive(true);
            profile1Img.sprite = GameManager.instance.allImages.sprites[MultiplayerManager.instance.player1DP];
        } 
        else if (_id == 2)
        {
            player2letter.SetText(_name);
            player2letter.transform.parent.gameObject.SetActive(true);
            profile2Img.sprite = GameManager.instance.allImages.sprites[MultiplayerManager.instance.player2DP];
        }
    }
}
