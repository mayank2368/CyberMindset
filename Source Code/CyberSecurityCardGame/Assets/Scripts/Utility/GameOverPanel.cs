using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GameOverPanel : MonoBehaviour
{
    public TextMeshProUGUI player1Score_txt, player2Score_txt, playerWonMsg_txt, player1Name_txt, player2Name_txt;
    public Image player1DP, player2DP;

    public void SetGameOverData(string _wonMsg, string _player1Name, string _player2Name)
    {
        playerWonMsg_txt.SetText(_wonMsg);
        player1Name_txt.SetText(_player1Name);
        player2Name_txt.SetText(_player2Name);
        player1Score_txt.SetText("Score : "+ MultiplayerManager.instance.Player1Score);
        player2Score_txt.SetText("Score : "+ MultiplayerManager.instance.Player2Score);
        player1DP.sprite = GameManager.instance.allImages.sprites[MultiplayerManager.instance.player1DP];
        player2DP.sprite = GameManager.instance.allImages.sprites[MultiplayerManager.instance.player2DP];
    }
}
