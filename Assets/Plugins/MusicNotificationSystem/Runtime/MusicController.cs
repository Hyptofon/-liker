using UnityEngine;
using System;
using System.Collections.Generic;
using Plugins.MusicNotificationSystem.Runtime;

public class MusicController : MonoBehaviour
{
    public static MusicController Instance;

    [Header("Playlist")]
    [SerializeField] private List<MusicTrack> playlist;
    
    [Header("Settings")]
    [SerializeField] private AudioSource musicSource;

    private int _currentTrackIndex = -1;
    public event Action<MusicTrack> OnTrackChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Invoke(nameof(PlayNext), 1f);
    }

    private void Update()
    {
        if (!musicSource.isPlaying && musicSource.clip != null)
        {
            PlayNext();
        }
    }

    public void PlayNext()
    {
        if (playlist.Count == 0) return;
        _currentTrackIndex = (_currentTrackIndex + 1) % playlist.Count;
        PlayTrack(playlist[_currentTrackIndex]);
    }

    private void PlayTrack(MusicTrack track)
    {
        musicSource.clip = track.clip;
        musicSource.Play();
        OnTrackChanged?.Invoke(track);
    }
}