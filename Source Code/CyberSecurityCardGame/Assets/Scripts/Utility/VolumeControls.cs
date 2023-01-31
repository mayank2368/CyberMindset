using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControls : MonoBehaviour
{
    public AudioMixer mixer;
    public string SliderName;
    public Slider slider;

    void Start() {

        float _value = 0;

        if (SliderName.Equals("Master"))
        {
            _value = PlayerPrefs.GetFloat("Master", 1f);
            slider.value = _value;
            SetMaster(_value);
            // mixer.GetFloat("Master", out _value);
            // slider.value = DecibelToLinear(_value);
        }
        else if (SliderName.Equals("SFX"))
        {
            _value = PlayerPrefs.GetFloat("SFX", 1f);
            slider.value = _value;
            SetSfx(_value);
            // mixer.GetFloat("SFX", out _value);
            // slider.value = DecibelToLinear(_value);
        }
        else if (SliderName.Equals("BGM"))
        {
            _value = PlayerPrefs.GetFloat("BGM", 1f);
            slider.value = _value;
            SetBgm(_value);
            // mixer.GetFloat("BGM", out _value);
            // slider.value = DecibelToLinear(_value);
        }
        
    }

    public void SetMaster(float _vol)
    {
        PlayerPrefs.SetFloat("Master", _vol);
        if (!AudioManager.instance.Sound)
        {
            return;
        }
        mixer.SetFloat("Master", LinearToDecibel(_vol));
    }

    public void SetSfx(float _vol)
    {
        PlayerPrefs.SetFloat("SFX", _vol);
        mixer.SetFloat("SFX", LinearToDecibel(_vol));
    }
    
    public void SetBgm(float _vol)
    {
        PlayerPrefs.SetFloat("BGM", _vol);
        mixer.SetFloat("BGM", LinearToDecibel(_vol));
    }



    private float LinearToDecibel(float linear)
    {
        float dB;
        
        if (linear != 0)
            dB = 20.0f * Mathf.Log10(linear);
        else
            dB = -144.0f;
        
        return dB;
    }

    private float DecibelToLinear(float dB)
    {
        float linear = Mathf.Pow(10.0f, dB/20.0f);

        return linear;
    }
}
