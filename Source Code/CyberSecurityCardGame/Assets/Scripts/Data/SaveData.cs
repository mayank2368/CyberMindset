using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData 
{
    public int Level1HighScore, Level2HighScore, Level2UnlockPoints;
    public bool Level2Unlocked;
    public int userImg;
    public string UserID; // incase you need multiple login users data
}

[System.Serializable]
public class SaveDataList
{
    public List<SaveData> allSaves;
}