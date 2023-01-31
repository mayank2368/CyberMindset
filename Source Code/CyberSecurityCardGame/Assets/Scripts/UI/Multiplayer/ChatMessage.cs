using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ChatMessage : MonoBehaviour
{
    public TextMeshProUGUI msg;
    public Image pic;

    public void SetData(string _msg, string _user, int _imgId)
    {
        msg.SetText(_user+": "+_msg);
        pic.sprite = GameManager.instance.allImages.sprites[_imgId];
    }
}
