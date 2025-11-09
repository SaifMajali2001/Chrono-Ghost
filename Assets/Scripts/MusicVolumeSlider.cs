using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Slider))]
public class MusicVolumeSlider : MonoBehaviour
{
    private Slider slider;

    [Tooltip("If true, the slider will set the saved volume on Start; otherwise it only reacts to user input.")]
    public bool initializeFromSaved = true;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        if (slider == null) return;

        slider.minValue = 0f;
        slider.maxValue = 1f;

        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void Start()
    {
        if (initializeFromSaved)
        {
            float saved = 1f;
            if (MusicManager.Instance != null)
            {
                saved = MusicManager.Instance.GetSavedVolume();
                slider.value = saved;
                MusicManager.Instance.SetMusicVolumeLinear(saved);
            }
            else
            {
                saved = UnityEngine.PlayerPrefs.GetFloat("musicVolume", 1f);
                slider.value = saved;
            }
        }
    }

    private void OnSliderChanged(float value)
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.SetMusicVolumeLinear(value);
    }

    private void OnDestroy()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(OnSliderChanged);
    }
}
