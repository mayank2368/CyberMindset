using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public string Username;
    public ProfileImages allImages;
    public SaveData data;
    public int Level2UnlockPoints = 200;
    string SavePath;
    [SerializeField]
    SaveDataList allUserData;

    void Awake()
    {
        if (instance != null) 
        {
            Destroy(gameObject);
        } 
        else 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadScene(string _scene)
    {
        if (SceneManager.GetActiveScene().Equals(_scene))
        {
            return;
        }

        SceneManager.LoadScene(_scene);
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Debug.Log("Level loaded: "+scene.name);
        AudioManager.instance.onSceneChanged(scene.name);
    }

    public void UserLoggedIn()
    {
        LoadData();
        MenuUi.instance.CheckLevel2Lock();
        MenuUi.instance.UserPanel.GetComponent<UserPanel>().UpdateUserData();
    }



    // ------------------------------------------------- save data -------------------------------------------------
    void LoadData()
    {
        string jsonString = "";
        SavePath = Path.Combine(Application.persistentDataPath, "SaveDataList.json");
        allUserData = new SaveDataList();
        allUserData.allSaves = new List<SaveData>();
        var firebaseUser = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser;
        
        if (File.Exists(SavePath)) // if save file exists
        {
            jsonString = File.ReadAllText(SavePath);
            allUserData = JsonUtility.FromJson<SaveDataList>(jsonString);
            data = allUserData.allSaves.Find( (user) => user.UserID == firebaseUser.UserId);
            
            if (data != null)
            {
                return;   
            }
        }
        // else
        // {
        //     data = new SaveData();
        //     data.UserID = firebaseUser.UserId;
        //     data.Level1HighScore = 0;
        //     data.Level2HighScore = 0;
        //     data.Level2UnlockPoints = Level2UnlockPoints;
        //     data.Level2Unlocked = false;
        //     data.userImg = 0;
        //     allUserData.Add(data);
        //     SaveData();
        // }
        data = new SaveData();
        data.UserID = firebaseUser.UserId;
        data.Level1HighScore = 0;
        data.Level2HighScore = 0;
        data.Level2UnlockPoints = Level2UnlockPoints;
        data.Level2Unlocked = false;
        data.userImg = 0;
        allUserData.allSaves.Add(data);
        SaveData();
    }

    void SaveData()
    {
        string jsonString = JsonUtility.ToJson(allUserData, true);
        File.WriteAllText(SavePath, jsonString);
    }

    public void updateLevel1Highscore(int _score)
    {
        if (_score > data.Level1HighScore)
        {
            data.Level1HighScore = _score;
            if (data.Level1HighScore >= Level2UnlockPoints && !data.Level2Unlocked)
            {
                data.Level2Unlocked = true;
                StartCoroutine("UnlockLevel2");
            }
            SaveData();
        }
    }

    public void updateLevel2Highscore(int _score)
    {
        if (_score > data.Level2HighScore)
        {
            data.Level2HighScore = _score;
            SaveData();
        }
    }

    public void updateImage(int _imgNo)
    {
        data.userImg = _imgNo;
        SaveData();
    }


    IEnumerator UnlockLevel2()
    {
        yield return new WaitForSeconds(1.5f);
        GameUi.instance.ToastMsg("Level 2 is now Unlocked !");
    }


    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
