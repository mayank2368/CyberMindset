using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    public List<GameObject> SinglePlayerTutorial, MultiPlayerTutorial;

    int singlePlayerIndex = 0, multiPlayerIndex = 0;

    // single player next slide
    public void ShowNextSPSlide(int _index)
    {
        singlePlayerIndex = _index;

        foreach (GameObject panel in SinglePlayerTutorial)
        {
            panel.SetActive(false);
        }

        if (_index < SinglePlayerTutorial.Count)
        {
            SinglePlayerTutorial[_index].SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // multiplayer player next slide
    public void ShowNextMPSlide(int _index)
    {
        multiPlayerIndex = _index;

        foreach (GameObject panel in MultiPlayerTutorial)
        {
            panel.SetActive(false);
        }

        if (_index < MultiPlayerTutorial.Count)
        {
            MultiPlayerTutorial[_index].SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void DisableAllSlides()
    {
        foreach (GameObject panel in SinglePlayerTutorial)
        {
            panel.SetActive(false);
        }
        foreach (GameObject panel in MultiPlayerTutorial)
        {
            panel.SetActive(false);
        }
    }
}
