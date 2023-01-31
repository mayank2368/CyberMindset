using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
using Google;

public class MenuUi : MonoBehaviour
{
    public static MenuUi instance;
    public GameObject RegisterPanel, LoginPanel, HomePanel, SettingsPanel, UserPanel, ManualPanel, TutorialPanel, ErrorPanel, LoadingPanel;
    public GameObject level2Btn, level2TutorialBtn;
    public TextMeshProUGUI username_txt, error_txt;
    public Image profileImg;
    public TutorialPanel tutorialPanel;
    List<GameObject> AllPanels;

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
        AllPanels = new List<GameObject>();
        AllPanels.Add(LoginPanel);
        AllPanels.Add(RegisterPanel);
        AllPanels.Add(HomePanel);
        AllPanels.Add(SettingsPanel);
        AllPanels.Add(UserPanel);
        AllPanels.Add(ManualPanel);
        AllPanels.Add(ErrorPanel);
        AllPanels.Add(LoadingPanel);

        ShowLoginPanel();
        LoginPanel.GetComponent<LoginPanel>().ValidateUser();
    }

    public void CheckLevel2Lock()
    {
        if (GameManager.instance.data.Level2Unlocked)
        {
            level2Btn.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            level2Btn.GetComponent<Button>().interactable = true;
            level2TutorialBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            level2TutorialBtn.GetComponent<Button>().interactable = true;
        }
        else
        {
            level2Btn.GetComponent<Image>().color = new Color32(255, 255, 255, 50);
            level2Btn.GetComponent<Button>().interactable = false;
            level2TutorialBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 50);
            level2TutorialBtn.GetComponent<Button>().interactable = false;
        }
    }

    public void Level1Click()
    {
        GameManager.instance.LoadScene("Level1");
    }
    public void Level2Click()
    {
        GameManager.instance.LoadScene("Level2");
    }

    public void SettingsClick()
    {
        disableAllPanels();
        SettingsPanel.SetActive(true);
    }

    public void BackHomeClick()
    {
        disableAllPanels();
        HomePanel.SetActive(true);

        // lazy code
        profileImg.sprite = GameManager.instance.allImages.sprites[GameManager.instance.data.userImg];
    }

    public void setUsernameText(string _txt)
    {
        username_txt.SetText(_txt);
    }

    public void Logout()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        if (GoogleSignIn.Configuration != null)
        {
            GoogleSignIn.DefaultInstance.SignOut();
        }
        MenuUi.instance.ShowLoginPanel();
    }

    public void QuitGame()
    {
        if (Application.isEditor)
        {
            Debug.Log("Game closed");
            return;
        }
        else
        {
            Application.Quit();
        }
    }

    // ---------------------- panels ----------------------
    void disableAllPanels()
    {
        foreach (GameObject panel in AllPanels)
        {
            panel.SetActive(false);
        }
    }

    public void ShowRegisterPanel()
    {
        disableAllPanels();
        RegisterPanel.SetActive(true);
    }

    public void ShowLoginPanel()
    {
        disableAllPanels();
        LoginPanel.SetActive(true);
    }

    public void ShowLoadingPanel()
    {
        LoadingPanel.SetActive(true);
    }

    public void HideLoadingPanel()
    {
        LoadingPanel.SetActive(false);
    }

    public void ShowErrorPanel(string _msg)
    {
        ErrorPanel.SetActive(true);
        error_txt.SetText(_msg);
    }

    public void HideErrorPanel()
    {
        ErrorPanel.SetActive(false);
    }

    public void ShowUserPanel()
    {
        disableAllPanels();
        UserPanel.SetActive(true);
    }

    public void ShowManualPanel()
    {
        disableAllPanels();
        ManualPanel.SetActive(true);
    }

    public void ShowTutorialSP()
    {
        // disableAllPanels();
        TutorialPanel.SetActive(true);
        tutorialPanel.DisableAllSlides();
        tutorialPanel.ShowNextSPSlide(0);
    }

    public void ShowTutorialMP()
    {
        // disableAllPanels();
        TutorialPanel.SetActive(true);
        tutorialPanel.DisableAllSlides();
        tutorialPanel.ShowNextMPSlide(0);
    }
}
