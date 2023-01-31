using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiplayerUi : MonoBehaviour
{
    public static MultiplayerUi instance;
    public GameObject LobbyPanel, SettingsPanel, LoadingPanel, GameOverPanel, ChatPanel, TutorialPanel, PopupPanel, ErrorPanel;
    public TextMeshProUGUI popupMsg_txt;
    public TMP_InputField roomName_input;
    public MPLoadingPanel loadingPanel;
    public ChatPanel chatPanel;
    public GameOverPanel gameOverPanel;
    public Toast toast;
    public TutorialPanel tutorialPanel;

    List<GameObject> allPanels;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start() {
        // TableManager.instance.gameObject.SetActive(false);

        allPanels = new List<GameObject>();
        allPanels.Add(LobbyPanel);
        allPanels.Add(SettingsPanel);
        allPanels.Add(LoadingPanel);
        allPanels.Add(GameOverPanel);
        allPanels.Add(ChatPanel);
        allPanels.Add(ErrorPanel);

        // hideAllPanels();
        LoadingPanel.SetActive(true);
        loadingPanel.SetLoadingText("Loading . . .");
    }

    public void ShowLobbyPanel()
    {
        hideAllPanels();
        LobbyPanel.SetActive(true);
    }

    public void HideLobbyPanel()
    {
        LobbyPanel.SetActive(false);
    }

    public void ShowLoadingPanel()
    {
        hideAllPanels();
        LoadingPanel.SetActive(true);
        loadingPanel.SetLoadingText("Waiting for opponent to connect");
    }

    public void ShowSettingsPanel()
    {
        hideAllPanels();
        SettingsPanel.SetActive(true);
    }

    public void ShowGameOverPanel(string _winnerName, string _player1Name, string _player2Name)
    {
        hideAllPanels();
        GameOverPanel.SetActive(true);
        gameOverPanel.SetGameOverData(_winnerName+" Won!", _player1Name, _player2Name);
        MultiplayerManager.instance.disconnectPlayer();
    }

    public void ShowErrorPanel(string _errMsg)
    {
        hideAllPanels();
        ErrorPanel.SetActive(true);
    }

    public void ShowChatPanel()
    {
        ChatPanel.SetActive(true);
        MultiplayerTableManager.instance.newMsgNotification.SetActive(false);
    }

    public void ShowPopupMessage(string _msg)
    {
        popupMsg_txt.SetText(_msg);
        PopupPanel.SetActive(true);
    }

    public void HidePopup()
    {
        PopupPanel.SetActive(false);
    }
    
    // ---------------- button clicks ----------------
    public void JoinRoomClick()
    {
        if (string.IsNullOrEmpty(roomName_input.text))
        {
            ShowPopupMessage("Room name cannot be empty! Enter a valid room name");
            return;
        }
        MultiplayerManager.instance.JoinRoom(roomName_input.text);
    }

    public void BackToMenuClick()
    {
        MultiplayerManager.instance.disconnectPlayer();
        GameManager.instance.LoadScene("Menu");
    }

    public void BackToLobby()
    {
        MultiplayerManager.instance.disconnectPlayer();
        GameManager.instance.LoadScene("Level2");
    }

    public void BackToGameClick()
    {
        hideAllPanels();
    }

    // -------------------------------- UI Updates --------------------------------
    public void ChatMessageRecived(string _msg, string _user, int _imgId)
    {
        chatPanel.AddChatMessage(_msg, _user, _imgId);
        AudioManager.instance.PlaySfx("notification");
        if (!chatPanel.gameObject.activeSelf)
        {
            MultiplayerTableManager.instance.newMsgNotification.SetActive(true);
        }
    }

    public void ToastMsg(string _msg)
    {
        toast.Display(_msg);
    }

    public void ShowTutorial()
    {
        hideAllPanels();
        TutorialPanel.SetActive(true);
        tutorialPanel.ShowNextMPSlide(0);
    }
    
    // ---------------- utility functions ----------------
    public void hideAllPanels()
    {
        foreach (GameObject panel in allPanels)
        {
            panel.SetActive(false);
        }
    }
}
