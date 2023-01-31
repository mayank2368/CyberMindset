using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatPanel : MonoBehaviour
{
    public Transform content;
    public ChatMessage messagePrefab;
    public TMP_InputField messageToSend;


    public void AddChatMessage(string _msg, string _user, int _imgId)
    {
        ChatMessage chatMsg = Instantiate(messagePrefab);
        chatMsg.SetData(_msg, _user, _imgId);
        chatMsg.transform.parent = content.transform;
        chatMsg.transform.localScale = Vector3.one;
    }

    public void SendMessage()
    {
        if (!string.IsNullOrEmpty(messageToSend.text))
        {
            MultiplayerManager.instance.RaiseChatMsg(messageToSend.text, GameManager.instance.Username, GameManager.instance.data.userImg);
            messageToSend.SetTextWithoutNotify("");
        }
    }

    public void CloseChatPanel()
    {
        gameObject.SetActive(false);
    }
}
