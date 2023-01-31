using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixer audioMixer;
    public AudioSource SFX_as, BGM_as;
    public Sound[] SFX;
    public Sound[] BGM;

    [HideInInspector]
    public bool Sound { get; set; }

    bool loopBGM = false;

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

    private void Start() {
        audioMixer.SetFloat("Master", LinearToDecibel(PlayerPrefs.GetFloat("Master", 1f)));
        audioMixer.SetFloat("SFX", LinearToDecibel(PlayerPrefs.GetFloat("SFX", 1f)));
        audioMixer.SetFloat("BGM", LinearToDecibel(PlayerPrefs.GetFloat("BGM", 1f)));

        // 1 = true, 0 = false
        Sound = (PlayerPrefs.GetInt("Sound", 1) == 1) ? true:false ;
        if (!Sound)
        {
            audioMixer.SetFloat("Master", LinearToDecibel(0));
        }
    }

    public void onSceneChanged(string _sceneName)
    {
        StopBGM();
    }

    public void PlayBGM(string name)
    {
        Sound s = Array.Find(BGM, bgm => bgm.name == name);

        if (s == null)
        {
            Debug.Log("Audio "+name+" doesnt exist!");
            return;
        }
        
        s.source = BGM_as;
        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.loop = s.loop;
        s.source.Play();
    }

    public void StopBGM()
    {
        BGM_as.Stop();
    }

    public void PlaySfx(string name)
    {
        Sound s = Array.Find(SFX, sfx => sfx.name == name);

        if (s == null)
        {
            Debug.Log("SFX "+name+" doesnt exist!");
            return;
        }
        
        s.source = SFX_as;
        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.loop = s.loop;
        s.source.Play();
    }

    // helper to conver from linear to log scale for audio
    private float LinearToDecibel(float linear)
    {
        float dB;
        
        if (linear != 0)
            dB = 20.0f * Mathf.Log10(linear);
        else
            dB = -144.0f;
        
        return dB;
    }
}

// Sound Class
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1;
    [HideInInspector]
    public AudioSource source;
    public bool loop;
}
