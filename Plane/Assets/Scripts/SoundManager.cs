using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField]private AudioMixer audioMixer;
    public string exposedParamName;
    public Slider slider;

    void Start()
    {       
        float currentValue;
        if (audioMixer.GetFloat(exposedParamName, out currentValue))
        {
            slider.value = Mathf.Pow(10, currentValue / 20);
        }

        slider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        audioMixer.SetFloat(exposedParamName, dB);
    }
}
