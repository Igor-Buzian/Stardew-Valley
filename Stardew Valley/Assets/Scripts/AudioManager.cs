// File: Scripts/Systems/AudioManager.cs
// Purpose: Centralized audio playback with pooled AudioSources.
// WHY: Spawning AudioSources on every SFX call causes GC spikes.
//      Pooling + centralized management lets us control volume, 
//      mixing categories, and audio-on-event without scattered AudioSource calls.

using System.Collections.Generic;
using UnityEngine;
using FarmSim.Core;

namespace FarmSim.Systems
{
    [System.Serializable]
    public class SoundEntry
    {
        public string id;
        public AudioClip clip;
        [Range(0f,1f)] public float volume = 1f;
        [Range(0.8f,1.2f)] public float pitchVariance = 0.1f;
    }

    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private int sfxPoolSize = 8;

        [Header("Sound Library")]
        [SerializeField] private List<SoundEntry> sounds = new();

        [Header("Volume")]
        [SerializeField, Range(0,1)] private float masterVolume = 1f;
        [SerializeField, Range(0,1)] private float musicVolume  = 0.6f;
        [SerializeField, Range(0,1)] private float sfxVolume    = 1f;

        private readonly Queue<AudioSource>      _sfxPool  = new();
        private readonly Dictionary<string, SoundEntry> _map = new();

        public void Init()
        {
            // Build lookup dictionary
            foreach (var s in sounds)
                _map[s.id] = s;

            // Create SFX pool
            for (int i = 0; i < sfxPoolSize; i++)
            {
                var go = new GameObject($"SFX_{i}");
                go.transform.SetParent(transform);
                var src = go.AddComponent<AudioSource>();
                src.playOnAwake = false;
                _sfxPool.Enqueue(src);
            }

            ApplyVolumes();
        }

        public void PlaySFX(string id)
        {
            if (!_map.TryGetValue(id, out var entry)) return;
            if (_sfxPool.Count == 0) return;  // pool exhausted; skip

            var src = _sfxPool.Dequeue();
            src.clip   = entry.clip;
            src.volume = entry.volume * sfxVolume * masterVolume;
            src.pitch  = 1f + Random.Range(-entry.pitchVariance, entry.pitchVariance);
            src.Play();

            // Return to pool after clip finishes
            StartCoroutine(ReturnToPool(src, entry.clip != null ? entry.clip.length + 0.05f : 0.5f));
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (musicSource.clip == clip) return;
            musicSource.clip   = clip;
            musicSource.loop   = loop;
            musicSource.volume = musicVolume * masterVolume;
            musicSource.Play();
        }

        public void StopMusic() => musicSource.Stop();

        public void SetMasterVolume(float v) { masterVolume = v; ApplyVolumes(); }
        public void SetMusicVolume(float v)  { musicVolume  = v; ApplyVolumes(); }
        public void SetSFXVolume(float v)    { sfxVolume    = v; }

        private void ApplyVolumes()
        {
            if (musicSource) musicSource.volume = musicVolume * masterVolume;
        }

        private System.Collections.IEnumerator ReturnToPool(AudioSource src, float delay)
        {
            yield return new WaitForSeconds(delay);
            _sfxPool.Enqueue(src);
        }
    }
}