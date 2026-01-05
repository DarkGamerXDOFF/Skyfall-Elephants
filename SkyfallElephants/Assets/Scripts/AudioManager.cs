using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager i;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private GameObject sfxPrefab; // Prefab with just an AudioSource
    [SerializeField] private int poolSize = 10;

    [SerializeField] private List<SoundData> sounds;

    private Dictionary<string, SoundData> soundLibrary = new();
    private Queue<AudioSource> sfxPool = new();

    [SerializeField] private AudioMixer musicMixer;
    [SerializeField] private AudioMixer sfxMixer;

    private const string MUSIC_VOL = "MusicVolume";
    private const string SFX_VOL = "SfxVolume";

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Awake()
    {
        if (i == null) i = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        foreach (var sound in sounds)
            if (!soundLibrary.ContainsKey(sound.id))
                soundLibrary.Add(sound.id, sound);

        for (int j = 0; j < poolSize; j++)
        {
            AudioSource src = Instantiate(sfxPrefab, transform).GetComponent<AudioSource>();
            src.playOnAwake = false;
            sfxPool.Enqueue(src);
        }
    }

    private void Start()
    {
        float musicVol = PlayerPrefs.GetFloat(MUSIC_VOL, 1f);
        float sfxVol = PlayerPrefs.GetFloat(SFX_VOL, 1f);

        SetMusicVolume(musicVol);
        SetSfxVolume(sfxVol);
        
        if (musicSlider != null)
            musicSlider.value = musicVol;
        if (sfxSlider != null)
            sfxSlider.value = sfxVol;

        PlayMusic("Theme");
    }

    public void PlayMusic(string soundId)
    {
        if (!soundLibrary.TryGetValue(soundId, out var sound)) return;
        musicSource.clip = sound.clip;
        musicSource.loop = true;
        musicSource.volume = sound.volume;
        musicSource.Play();
    }

    public void PlaySfx(string soundId)
    {
        if (!soundLibrary.TryGetValue(soundId, out var sound)) return;

        AudioSource src = GetFreeAudioSource();
        src.clip = sound.clip;
        src.volume = sound.volume;
        src.loop = false;
        src.Play();
        StartCoroutine(ReturnToPool(src, sound.clip.length));
    }

    public AudioSource PlayLoop(string soundId)
    {
        if (!soundLibrary.TryGetValue(soundId, out var sound)) return null;

        AudioSource src = GetFreeAudioSource();
        src.clip = sound.clip;
        src.volume = sound.volume;
        src.loop = true;
        src.Play();
        return src; // store reference to stop later
    }

    public void StopLoop(AudioSource source)
    {
        if (source == null) return;
        source.Stop();
        sfxPool.Enqueue(source);
    }

    private AudioSource GetFreeAudioSource()
    {
        if (sfxPool.Count == 0)
            return Instantiate(sfxPrefab, transform).GetComponent<AudioSource>();
        return sfxPool.Dequeue();
    }

    private System.Collections.IEnumerator ReturnToPool(AudioSource src, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!src.loop) sfxPool.Enqueue(src);
    }

    private float LinearToDb(float value)
    {
        return value <= 0.001f ? -80f : Mathf.Log10(value) * 20f;
    }

    public void SetMusicVolume(float value)
    {
        musicMixer.SetFloat(MUSIC_VOL, LinearToDb(value));
        PlayerPrefs.SetFloat(MUSIC_VOL, value);
    }

    public void SetSfxVolume(float value)
    {
        sfxMixer.SetFloat(SFX_VOL, LinearToDb(value));
        PlayerPrefs.SetFloat(SFX_VOL, value);
    }
}


[System.Serializable]
public class SoundData
{
    public string id;
    public AudioClip clip;
    public float volume = 1f;
    public bool loop = false;
}