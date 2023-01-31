using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserPanel : MonoBehaviour
{
    public Image dp1, dp2, dp3, dp4, dp5;
    public TextMeshProUGUI username_txt, level1Highscore_txt, level2Highscore_txt, pointsToUnlockLevel2_txt;
    public Image currentDP;

    Texture tex;

    void Start() 
    {
        UpdateUserData();
    }

    public void UpdateUserData() 
    {
        username_txt.SetText(GameManager.instance.Username);
        currentDP.sprite = GameManager.instance.allImages.sprites[GameManager.instance.data.userImg];

        // load images
        dp1.sprite = GameManager.instance.allImages.sprites[0];
        dp2.sprite = GameManager.instance.allImages.sprites[1];
        dp3.sprite = GameManager.instance.allImages.sprites[2];
        dp4.sprite = GameManager.instance.allImages.sprites[3];
        dp5.sprite = GameManager.instance.allImages.sprites[4];

        LoadProfileData();
    }

    public void ImgBtnClick(int _number)
    {
        GameManager.instance.updateImage(_number-1);

        currentDP.sprite = GameManager.instance.allImages.sprites[_number-1];
    }

    // stored locally
    public void LoadProfileData()
    {
        int pointsToUnlock2 = GameManager.instance.Level2UnlockPoints - GameManager.instance.data.Level1HighScore;
        if (pointsToUnlock2 < 0)
        {
            pointsToUnlock2 = 0;
        }

        level1Highscore_txt.SetText("Level 1 Highscore - "+GameManager.instance.data.Level1HighScore);
        level2Highscore_txt.SetText("Level 2 Highscore - "+GameManager.instance.data.Level2HighScore);
        pointsToUnlockLevel2_txt.SetText("Points required to unlock level 2 - "+pointsToUnlock2+" points remaining.");
    }
}
