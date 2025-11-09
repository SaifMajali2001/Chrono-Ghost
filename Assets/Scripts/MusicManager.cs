using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string exposedVolumeParam = "MusicVolume";
    [SerializeField] private AudioMixerGroup outputGroup;

    [Header("Defaults")]
    [Range(0f,1f)]
    [SerializeField] private float defaultVolume = 1f;

    private AudioSource audioSource;
    private const string PlayerPrefKey = "musicVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.clip = musicClip;

        if (outputGroup != null)
            audioSource.outputAudioMixerGroup = outputGroup;
    }

    private void Start()
    {
        float saved = PlayerPrefs.GetFloat(PlayerPrefKey, defaultVolume);
        SetMusicVolumeLinear(saved);

        if (audioSource.clip != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void SetMusicVolumeLinear(float linear)
    {
        linear = Mathf.Clamp01(linear);

        float dB = (linear <= 0.0001f) ? -80f : Mathf.Log10(Mathf.Max(0.0001f, linear)) * 20f;

        if (audioMixer != null && !string.IsNullOrEmpty(exposedVolumeParam))
        {
            audioMixer.SetFloat(exposedVolumeParam, dB);
        }

        PlayerPrefs.SetFloat(PlayerPrefKey, linear);
        PlayerPrefs.Save();
    }

    public float GetSavedVolume()
    {
        return PlayerPrefs.GetFloat(PlayerPrefKey, defaultVolume);
    }

    public void PlayClip(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
    }
}
