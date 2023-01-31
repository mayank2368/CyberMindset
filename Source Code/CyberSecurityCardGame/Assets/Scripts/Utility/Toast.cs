using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Toast : MonoBehaviour
{
    public TextMeshProUGUI msg_txt;
    public Animator animator;

    public void Display(string _msg)
    {
        msg_txt.SetText(_msg);
        animator.Play("toast_in_out", -1, 0f);
    }
}
