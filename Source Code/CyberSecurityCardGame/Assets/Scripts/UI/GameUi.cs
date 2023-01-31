using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUi : MonoBehaviour
{
    public static GameUi instance;

    public GameObject SettingsPanel, HintPanel, TutorialPanel;
    public TextMeshProUGUI hint_txt;
    public Toast toast;
    public TutorialPanel tutorialPanel;
    List<GameObject> allPanels;

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

    private void Start() {
        allPanels = new List<GameObject>();
        allPanels.Add(SettingsPanel);
        allPanels.Add(HintPanel);
        allPanels.Add(TutorialPanel);
        HideAllPanels();
    }

    public void HideAllPanels()
    {
        foreach (GameObject panel in allPanels)
        {
            panel.SetActive(false);
        }
    }

    public void ShowSettingsPanel()
    {
        HideAllPanels();
        SettingsPanel.SetActive(true);
    }

    public void ShowHint(DeckType _type)
    {
        if (_type == DeckType.AttackDeck)
        {
            hint_txt.SetText(CardManager.instance.currentScenario.AttackHint);
        }
        else if (_type == DeckType.DefenseDeck)
        {
            hint_txt.SetText(CardManager.instance.currentScenario.DefenseHint);
        }

        // HintPanel.SetActive(true);
    }

    public void ShowHintPanel()
    {
        HintPanel.SetActive(true);
    }
    
    public void MainMenuBtnClick()
    {
        GameManager.instance.LoadScene("Menu");
    }

    public void ToastMsg(string _msg)
    {
        toast.Display(_msg);
    }

    public void ShowTutorial()
    {
        HideAllPanels();
        TutorialPanel.SetActive(true);
        tutorialPanel.ShowNextSPSlide(0);
    }
}
