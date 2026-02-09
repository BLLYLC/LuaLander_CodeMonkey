using System;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private const int MUSIC_VOLUME_MAX = 10;
    private static int musicVolume = 4;

    private AudioSource musicAudioSource;
    private static float musicTime;

    public static MusicManager Instance;
    public event EventHandler OnMusicVolumeChanged;

    private void Awake()
    {
        Instance = this;
        musicAudioSource = GetComponent<AudioSource>();
        musicAudioSource.time = musicTime;
    }
    private void Start()
    {
        musicAudioSource.volume = GetMusicVolumeNormalized();
    }
    private void Update()
    {
        musicTime = musicAudioSource.time;
    }
    public void ChangeMusicVolume()
    {

        musicVolume = (musicVolume + 1) % MUSIC_VOLUME_MAX;
        musicAudioSource.volume = GetMusicVolumeNormalized();
        OnMusicVolumeChanged?.Invoke(this, EventArgs.Empty);
    }
    public int GetMusicVolume()
    {
        return musicVolume;
    }
    public float GetMusicVolumeNormalized()
    {
        return ((float)musicVolume) / MUSIC_VOLUME_MAX;
    }
}
