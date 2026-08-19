using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class VolumnSlider : MonoBehaviour
{
    GameObject audioManager;
    [SerializeField] TextMeshProUGUI percent;
    AudioSource[] allAudioSources;
    Slider slider;

    private void Awake()
    {
        audioManager = GameObject.Find("AudioManager");
        allAudioSources = audioManager.GetComponents<AudioSource>();
        slider = GetComponent<Slider>();
        slider.value = allAudioSources[0].volume;
    }


    void Update()
    {
        foreach (AudioSource source in allAudioSources)
        {
            if (source != null)
            {
                source.volume = slider.value;
                percent.text = (slider.value * 100).ToString("F2") + "%";
            }
        }
    }
}
