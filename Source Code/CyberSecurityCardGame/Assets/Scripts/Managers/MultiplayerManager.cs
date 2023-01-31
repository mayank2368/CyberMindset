using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

// using this class for multiplayer and multiplayer UI as well
public class MultiplayerManager : MonoBehaviourPunCallbacks, IInRoomCallbacks, IOnEventCallback
{
    public static MultiplayerManager instance;

    public bool canJoinRoom;
    public int MaxUsersPerRoom = 2, TimePerPlayerTurn = 30;
    public int player1DP { get { return player1Img; } }
    public int player2DP { get { return player2Img; } }
    public int player1Streak { get { return p1Streak; } }
    public int player2Streak { get { return p2Streak; } }
    public int Player1Score { get { return player1Score; } }
    public int Player2Score { get { return player2Score; } }
    public int RoundNumber { get { return roundNumber; } }

    int StreakMultiplier = 1;
    string playerName = "default";
    string gameVersion = "0.1";
    string roomName = "defaultRoom";
    bool startGame = false, hasRecivedGameId, manualDisconnect, endGameReached;

    // game session related data
    int timeLeft = 30;
    int roundNumber = 0;
    [SerializeField]
    float player1Health = 1f, player2Health = 1f; // calculated in percentage from 0 to 1
    int player1Score = 0, player2Score = 0, player1Img, player2Img, p1Streak = 1, p2Streak = 1;
    string player1Name, player2Name;
    public DeckType playerCanMove { get; set; } // the type of card player can affect this turn //redundant due to new logic changes but left incase bugs...
    public DeckType currentTurn { get; set; } // current turn is attack/defense
    public int activeActorId { get; set; }


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

    void Start()
    {
        MultiplayerUi.instance.HideLobbyPanel();
        manualDisconnect = false; // to check if player left the game or got disconnected
        endGameReached = false;
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.KeepAliveInBackground = 1;
        canJoinRoom = true;
        timeLeft = TimePerPlayerTurn;
        JoinMaster();
    }

    public void JoinMaster()
    {
        PhotonNetwork.NickName = playerName;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        Debug.Log(PhotonNetwork.IsConnected);
    }

    public void JoinRoom(string _roomName)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = (byte)MaxUsersPerRoom;
        roomOptions.EmptyRoomTtl = 0;
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "countdown" };

        Hashtable properties = new Hashtable();
        properties["img"] = GameManager.instance.data.userImg;
        PhotonNetwork.NickName = GameManager.instance.Username;
        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);

        PhotonNetwork.JoinOrCreateRoom(_roomName, roomOptions, TypedLobby.Default);
    }

    public void ClosePhotonRoom() // close room manually if any issues later, should be fine since max players = 2
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");

        if (canJoinRoom) // OnConnectedToMaster gets called even on LeaveLobby()... werid logic, hence this check
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        MultiplayerUi.instance.ShowLobbyPanel();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected. StatusCode: " + cause.ToString() + " ServerAddress: " + PhotonNetwork.ServerAddress);
        if(!manualDisconnect)
            MultiplayerUi.instance.ShowErrorPanel("Player Disconnected");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("Master Switched : " + newMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
        MultiplayerUi.instance.ShowPopupMessage(message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(returnCode + " OnJoinRoomFailed got called. This can happen if the room is not existing or full or closed." + message);
        MultiplayerUi.instance.ShowPopupMessage(message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed got called. This can happen if the room is not existing or full or closed.");
        MultiplayerUi.instance.ShowPopupMessage(message);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");

        if (PhotonNetwork.IsMasterClient)
        {   
            Debug.Log("is master");
        }
    }

    public override void OnJoinedRoom()
    {
        manualDisconnect = false;
        RaiseEventPlayerJoined();
        Debug.Log("players in room " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom");
        Debug.Log("" + PhotonNetwork.CurrentRoom.PlayerCount);
        //check if room count = 10 i.e. full
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("OnPlayerLeftRoom");
        Debug.Log("" + PhotonNetwork.CurrentRoom.PlayerCount);
        MultiplayerUi.instance.ShowErrorPanel("Left the game");
    }

    // Custom functs
    public void disconnectPlayer()
    {
        manualDisconnect = true;
        PhotonNetwork.Disconnect();
    }

    public void TurnUpdate()
    {
        StopCoroutine("turnTimer");
        currentTurn = DeckType.AttackDeck; // first attack then defender...
        timeLeft = TimePerPlayerTurn;
        roundNumber++;
        MultiplayerCardManager.instance.UpdateCurrentAssetData();
        
        if (PhotonNetwork.IsMasterClient)
        {
            if (roundNumber%2 != 0)
            {
                playerCanMove = DeckType.AttackDeck;
                MultiplayerTableManager.instance.profile1.setCardTurnState(DeckType.AttackDeck);
                MultiplayerTableManager.instance.profile2.setCardTurnState(DeckType.DefenseDeck);
            }
            else
            {
                playerCanMove = DeckType.DefenseDeck;
                MultiplayerTableManager.instance.profile1.setCardTurnState(DeckType.DefenseDeck);
                MultiplayerTableManager.instance.profile2.setCardTurnState(DeckType.AttackDeck);
            }

            if (playerCanMove == currentTurn)
            {
                activeActorId = PhotonNetwork.MasterClient.ActorNumber;
            }
            else
            {
                activeActorId = PhotonNetwork.MasterClient.GetNext().ActorNumber;
            }
        }
        else
        {
            if (roundNumber%2 != 0)
            {
                playerCanMove = DeckType.DefenseDeck;
                MultiplayerTableManager.instance.profile1.setCardTurnState(DeckType.AttackDeck);
                MultiplayerTableManager.instance.profile2.setCardTurnState(DeckType.DefenseDeck);
            }
            else
            {
                playerCanMove = DeckType.AttackDeck;
                MultiplayerTableManager.instance.profile1.setCardTurnState(DeckType.DefenseDeck);
                MultiplayerTableManager.instance.profile2.setCardTurnState(DeckType.AttackDeck);
            }

            if (playerCanMove == currentTurn)
            {
                activeActorId = PhotonNetwork.MasterClient.GetNext().ActorNumber;
            }
            else
            {
                activeActorId = PhotonNetwork.MasterClient.ActorNumber;
            }
        }

        MultiplayerCardManager.instance.UpdateMyControls(currentTurn, activeActorId);
        MultiplayerUi.instance.ToastMsg("Round "+roundNumber);

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine("turnTimer");
        }
    }

    // add score for player who controls decktype X this round
    public void addScore(DeckType _forDeckType, int _score)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (_forDeckType == playerCanMove)
            {
                player1Score += (_score * p1Streak);
                p1Streak++;
                MultiplayerTableManager.instance.profile1.setScore(player1Score, p1Streak);
            }
            else
            {
                player2Score += (_score * p2Streak);
                p2Streak++;
                MultiplayerTableManager.instance.profile2.setScore(player2Score, p2Streak);
            }

            GameManager.instance.updateLevel2Highscore(player1Score);
        }
        else
        {
            if (_forDeckType == playerCanMove)
            {
                player2Score += (_score * p2Streak);
                p2Streak++;
                MultiplayerTableManager.instance.profile2.setScore(player2Score, p2Streak);
            }
            else
            {
                player1Score += (_score * p1Streak);
                p1Streak++;
                MultiplayerTableManager.instance.profile1.setScore(player1Score, p1Streak);
            }

            GameManager.instance.updateLevel2Highscore(player2Score);
        }
    }

    // subtract health and score for player who controls decktype X this round
    public void subtractHealth(DeckType _forDeckType, int _subtractScore) 
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (_forDeckType == playerCanMove)
            {
                player1Health = player1Health - 0.1f; // temporary fixed value for testing
                player1Health = Mathf.Round(player1Health * 100f) / 100f;
                MultiplayerTableManager.instance.profile1.setHealth(player1Health);
                // player1Score += _subtractScore;
                // if (player1Score < 0)
                // {
                //     player1Score = 0;
                // }
                p1Streak = 1;
                MultiplayerTableManager.instance.profile1.setScore(player1Score, p1Streak);
            }
            else
            {
                player2Health = player2Health - 0.1f; // temporary fixed value for testing
                player2Health = Mathf.Round(player2Health * 100f) / 100f;
                MultiplayerTableManager.instance.profile2.setHealth(player2Health);
                // player2Score += _subtractScore;
                // if (player2Score < 0)
                // {
                //     player2Score = 0;
                // }
                p2Streak = 1;
                MultiplayerTableManager.instance.profile2.setScore(player2Score, p2Streak);
            }
        }
        else
        {
            if (_forDeckType == playerCanMove)
            {
                player2Health = player2Health - 0.1f; // temporary fixed value for testing
                player2Health = Mathf.Round(player2Health * 100f) / 100f;
                MultiplayerTableManager.instance.profile2.setHealth(player2Health);
                // player2Score += _subtractScore;
                // if (player2Score < 0)
                // {
                //     player2Score = 0;
                // }
                p2Streak = 1;
                MultiplayerTableManager.instance.profile2.setScore(player2Score, p2Streak);
            }
            else
            {
                player1Health = player1Health - 0.1f; // temporary fixed value for testing
                player1Health = Mathf.Round(player1Health * 100f) / 100f;
                MultiplayerTableManager.instance.profile1.setHealth(player1Health);
                // player1Score += _subtractScore;
                // if (player1Score < 0)
                // {
                //     player1Score = 0;
                // }
                p1Streak = 1;
                MultiplayerTableManager.instance.profile1.setScore(player1Score, p1Streak);
            }
        }

        // check if a players health is 0
        if (player1Health <= 0)
        {
            TouchHandler.instance.TouchIsEnabled = false;
            RaiseGameOver(2); // int indicates player who won... 1 = master 2 = non master client
        }
        else if (player2Health <= 0)
        {
            TouchHandler.instance.TouchIsEnabled = false;
            RaiseGameOver(1);
        }
    }

    // EVENTS -------------
    public void RaiseEventPlayerJoined()
    {
        byte evCode = 1; 
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }

    void RaiseCountdown(int _timeLeft)
    {
        byte evCode = 2; 
        object[] content = new object[] { _timeLeft };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; 
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }

    public void UpdateCardPos(int _cardId, DeckType _type, Vector3 _pos)
    {
        byte evCode = 3; 
        object[] content = new object[] { _cardId, _type, _pos };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; 
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }

    public void NetworkedShuffelCards(List<int> _attackCardsIds, List<int> _defenseCardsIds) // unused
    {
        byte evCode = 4; 
        object[] content = new object[] { _attackCardsIds.ToArray(), _defenseCardsIds.ToArray() };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; 
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }

    public void NetworkedShareSelectedScenario(int _scenarioId) // scenario changed to assets so unused
    {
        byte evCode = 5; 
        object[] content = new object[] { _scenarioId };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; 
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }

    public void NetworkDeckClicked(DeckType _type)
    {
        byte evCode = 6; 
        object[] content = new object[] { _type };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; 
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }

    public void RaiseTurnUpdate()
    {
        byte evCode = 7;
        Debug.Log("RaiseTurnUpdate");
        object[] content = new object[] { roundNumber };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; 
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }

    public void RaiseGameOver(int _playerWonId) //int indicates player who won... 1 = master 2 = non master client
    {
        byte evCode = 8;
        object[] content = new object[] { _playerWonId };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; 
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }

    public void RaiseTurnTimer(int _timeLeft, int _activeActorId) // timer for each players time to play their respective card
    {
        byte evCode = 9;
        object[] content = new object[] { _timeLeft, _activeActorId };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; 
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }

    public void RaiseChatMsg(string _msg, string _username, int _imgId) // sendind chat messages
    {
        byte evCode = 10;
        object[] content = new object[] { _msg, _username, _imgId };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; 
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }

    public void RaiseControlsSwitch() // switches control from the attack to defender after the attack card is played
    {
        byte evCode = 11;
        // Debug.Log("RaiseControlsSwitch");
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; 
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }










    // network data is shared here in the form of events using a code for each event
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        
        //Optimize: use switch statement instead 
        if (eventCode == 1) // RaiseEventPlayerJoined
        {
            // object[] data = (object[])photonEvent.CustomData;
            setPlayers();
            MultiplayerUi.instance.ShowLoadingPanel();
        } 
        else if (eventCode == 2) // RaiseCountdown
        {
            object[] data = (object[])photonEvent.CustomData;
            int countdown = (int)data[0];
            MultiplayerUi.instance.loadingPanel.GameStartingIn(countdown);

            if (countdown <= 0)
            {
                startGame = true;
                MultiplayerCardManager.instance.gameObject.SetActive(true);
                MultiplayerCardManager.instance.Initalize();
                MultiplayerUi.instance.hideAllPanels();
                TurnUpdate();
            }
        }
        else if (eventCode == 3) // update a card pos across the network
        {
            object[] data = (object[])photonEvent.CustomData;
            int _cardId = (int)data[0];
            DeckType _type = (DeckType)data[1];
            Vector3 _pos = (Vector3)data[2];

            MultiplayerCardManager.instance.SyncMultiplayerCardPos(_cardId, _type, _pos);
        }
        else if (eventCode == 4) // update shuffeled decks data, unused
        {
            object[] data = (object[])photonEvent.CustomData;
            List<int> _attackCardsIds = new List<int>((int[])data[0]);
            List<int> _defenseCardsIds = new List<int>((int[])data[1]);
            // MultiplayerCardManager.instance.ApplyNetworkedShuffelDecks(_attackCardsIds, _defenseCardsIds);
        }
        else if (eventCode == 5) // scenario changed to assets so unused
        {
            object[] data = (object[])photonEvent.CustomData;
            int _scenarioId = (int)data[0];

            // MultiplayerCardManager.instance.RecieveSharedScenario(_scenarioId);
        }
        else if (eventCode == 6) // deck clicked update
        {
            object[] data = (object[])photonEvent.CustomData;
            DeckType _type = (DeckType)data[0];

            MultiplayerCardManager.instance.ApplyNetworkDeckClicked(_type);
        }
        else if (eventCode == 7)
        {
            object[] data = (object[])photonEvent.CustomData;
            int prevRnd = (int)data[0];
            roundNumber = prevRnd + 1;
            Debug.Log("TurnUpdate");
            TurnUpdate();
        }
        else if (eventCode == 8)
        {
            object[] data = (object[])photonEvent.CustomData;
            int _playerWonId = (int)data[0];
            
            if (_playerWonId == 1)
            {
                MultiplayerUi.instance.ShowGameOverPanel(PhotonNetwork.MasterClient.NickName, PhotonNetwork.MasterClient.NickName, PhotonNetwork.MasterClient.GetNext().NickName);
            }
            else
            {
                MultiplayerUi.instance.ShowGameOverPanel(PhotonNetwork.MasterClient.GetNext().NickName, PhotonNetwork.MasterClient.NickName, PhotonNetwork.MasterClient.GetNext().NickName);
            }
        }
        else if (eventCode == 9) // turn timer update
        {
            object[] data = (object[])photonEvent.CustomData;
            int _timer = (int)data[0];
            int _activeActorId = (int)data[1];
            updateTimerForPlayers(_timer, _activeActorId);
        }
        else if (eventCode == 10) // chat msg recived
        {
            object[] data = (object[])photonEvent.CustomData;
            string _msg = (string)data[0];
            string _user = (string)data[1];
            int _imgId = (int)data[2];
            MultiplayerUi.instance.ChatMessageRecived(_msg, _user, _imgId);
        }
        else if (eventCode == 11) // attacker played his card
        {
            Debug.Log("RaiseControlsSwitch evt");
            StopCoroutine("turnTimer");
            currentTurn = DeckType.DefenseDeck;
            timeLeft = TimePerPlayerTurn;
            activeActorId = PhotonNetwork.CurrentRoom.GetPlayer(activeActorId).GetNext().ActorNumber;
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine("turnTimer");
            }

            TouchHandler.instance.TouchIsEnabled = true;
            MultiplayerCardManager.instance.UpdateMyControls(currentTurn, activeActorId);
        }
    }

    void setPlayers()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            player1Name = PhotonNetwork.MasterClient.NickName;
            player1Img = (int)PhotonNetwork.MasterClient.CustomProperties["img"];
            
            MultiplayerUi.instance.loadingPanel.setPlayerData(PhotonNetwork.MasterClient.NickName, 1);
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            player1Name = PhotonNetwork.MasterClient.NickName;
            player1Img = (int)PhotonNetwork.MasterClient.CustomProperties["img"];
            player2Name = PhotonNetwork.MasterClient.GetNext().NickName;
            player2Img = (int)PhotonNetwork.MasterClient.GetNext().CustomProperties["img"];

            MultiplayerUi.instance.loadingPanel.setPlayerData(PhotonNetwork.MasterClient.NickName, 1);
            MultiplayerUi.instance.loadingPanel.setPlayerData(PhotonNetwork.MasterClient.GetNext().NickName, 2);

            startGame = true;
            
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(countdown(10));
            }
        }
    }

    void updateTimerForPlayers(int _timer, int _activeActorId)
    {
        if (_activeActorId == PhotonNetwork.MasterClient.ActorNumber && _timer >= 0)
        {
            MultiplayerTableManager.instance.profile1.showTimer(true);
            MultiplayerTableManager.instance.profile1.updateTimeLeft(_timer);
            MultiplayerTableManager.instance.profile2.showTimer(false);
        }
        else if (_activeActorId == (PhotonNetwork.MasterClient.ActorNumber+1) && _timer >= 0)
        {
            MultiplayerTableManager.instance.profile2.showTimer(true);
            MultiplayerTableManager.instance.profile2.updateTimeLeft(_timer);
            MultiplayerTableManager.instance.profile1.showTimer(false);
        }

        if (_timer == -2 && currentTurn == DeckType.AttackDeck)
        {
            MultiplayerCardManager.instance.DisableDragForDeck(DeckType.AttackDeck);
            if (PhotonNetwork.IsMasterClient)
            {
                RaiseControlsSwitch();
            }
        }
        else if (_timer == -1 && currentTurn == DeckType.DefenseDeck)
        {
            Debug.Log("ForceNextScenario");
            MultiplayerCardManager.instance.DisableDragForDeck(DeckType.DefenseDeck);
            MultiplayerCardManager.instance.CheckCardsPlaced(null);
        }

        if (_timer == 0)
        {
            TouchHandler.instance.TouchIsEnabled = false;
            TouchHandler.instance.ResetCard();
            
            if (currentTurn == DeckType.AttackDeck)
            {
                MultiplayerUi.instance.ToastMsg("Turn Switch");
            }
        }
    }

    public void ForceStopTimer()
    {
        StopCoroutine("turnTimer");
    }

    IEnumerator countdown(int _time)
    {
        while (_time >= 0)
        {
            RaiseCountdown(_time);
            yield return new WaitForSeconds(1);
            _time--;
        }
    }

    IEnumerator turnTimer()
    {
        while (timeLeft >= -2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                RaiseTurnTimer(timeLeft, activeActorId);
            }
            yield return new WaitForSeconds(1);
            timeLeft--;
        }
    }





    //-------------------Register events
    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
