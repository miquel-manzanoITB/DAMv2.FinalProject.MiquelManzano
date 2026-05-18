using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// AudioManager - Singleton para gestión centralizada de audio.
/// Coloca este script en un GameObject vacío llamado "AudioManager" en tu primera escena.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("=== MÚSICA DE FONDO ===")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource musicSourceB; // Para crossfade
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.5f;

    [Header("=== EFECTOS DE SONIDO ===")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;

    [Header("=== SONIDOS REGISTRADOS ===")]
    [SerializeField] private Sound[] sounds;

    // Estado interno
    private bool isMusicMuted = false;
    private bool isSfxMuted   = false;
    private Coroutine crossfadeCoroutine;
    private bool usingSourceA = true; // Para el crossfade

    // Claves de PlayerPrefs
    private const string PREF_MUSIC_VOL = "MusicVolume";
    private const string PREF_SFX_VOL   = "SfxVolume";
    private const string PREF_MUSIC_MUTE = "MusicMuted";
    private const string PREF_SFX_MUTE   = "SfxMuted";

    // ─────────────────────────────────────────────
    // Propiedades públicas
    // ─────────────────────────────────────────────
    public float MusicVolume
    {
        get => musicVolume;
        set
        {
            musicVolume = Mathf.Clamp01(value);
            ApplyMusicVolume();
            PlayerPrefs.SetFloat(PREF_MUSIC_VOL, musicVolume);
        }
    }

    public float SfxVolume
    {
        get => sfxVolume;
        set
        {
            sfxVolume = Mathf.Clamp01(value);
            sfxSource.volume = isSfxMuted ? 0f : sfxVolume;
            PlayerPrefs.SetFloat(PREF_SFX_VOL, sfxVolume);
        }
    }

    public bool IsMusicMuted
    {
        get => isMusicMuted;
        set
        {
            isMusicMuted = value;
            ApplyMusicVolume();
            PlayerPrefs.SetInt(PREF_MUSIC_MUTE, isMusicMuted ? 1 : 0);
        }
    }

    public bool IsSfxMuted
    {
        get => isSfxMuted;
        set
        {
            isSfxMuted = value;
            sfxSource.volume = isSfxMuted ? 0f : sfxVolume;
            PlayerPrefs.SetInt(PREF_SFX_MUTE, isSfxMuted ? 1 : 0);
        }
    }

    // ─────────────────────────────────────────────
    // Inicialización
    // ─────────────────────────────────────────────
    private void Awake()
    {
        // Singleton persistente entre escenas
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Crear AudioSources automáticamente si no están asignados
        if (musicSource  == null) musicSource  = CreateAudioSource("MusicA",  true,  true);
        if (musicSourceB == null) musicSourceB = CreateAudioSource("MusicB",  true,  true);
        if (sfxSource    == null) sfxSource    = CreateAudioSource("SFX",     false, false);

        LoadPreferences();
    }

    private AudioSource CreateAudioSource(string sourceName, bool loop, bool playOnAwake)
    {
        var go = new GameObject(sourceName);
        go.transform.SetParent(transform);
        var src = go.AddComponent<AudioSource>();
        src.loop        = loop;
        src.playOnAwake = playOnAwake;
        return src;
    }

    private void LoadPreferences()
    {
        musicVolume  = PlayerPrefs.GetFloat(PREF_MUSIC_VOL,  0.5f);
        sfxVolume    = PlayerPrefs.GetFloat(PREF_SFX_VOL,    1.0f);
        isMusicMuted = PlayerPrefs.GetInt(PREF_MUSIC_MUTE, 0) == 1;
        isSfxMuted   = PlayerPrefs.GetInt(PREF_SFX_MUTE,   0) == 1;

        ApplyMusicVolume();
        sfxSource.volume = isSfxMuted ? 0f : sfxVolume;
    }

    // ─────────────────────────────────────────────
    // MÚSICA
    // ─────────────────────────────────────────────

    /// <summary>Reproduce música por nombre (registrada en el array sounds).</summary>
    public void PlayMusic(string soundName, float fadeDuration = 0f)
    {
        Sound s = GetSound(soundName);
        if (s == null) return;
        PlayMusicClip(s.clip, fadeDuration);
    }

    /// <summary>Reproduce un AudioClip directamente como música.</summary>
    public void PlayMusicClip(AudioClip clip, float fadeDuration = 0f)
    {
        if (clip == null) return;

        AudioSource active  = usingSourceA ? musicSource  : musicSourceB;
        AudioSource passive = usingSourceA ? musicSourceB : musicSource;

        if (fadeDuration > 0f)
        {
            if (crossfadeCoroutine != null) StopCoroutine(crossfadeCoroutine);
            passive.clip   = clip;
            passive.volume = 0f;
            passive.Play();
            crossfadeCoroutine = StartCoroutine(CrossfadeRoutine(active, passive, fadeDuration));
            usingSourceA = !usingSourceA;
        }
        else
        {
            active.clip = clip;
            active.volume = isMusicMuted ? 0f : musicVolume;
            active.Play();
            passive.Stop();
        }
    }

    /// <summary>Para la música con fade opcional.</summary>
    public void StopMusic(float fadeDuration = 0f)
    {
        AudioSource active = usingSourceA ? musicSource : musicSourceB;
        if (fadeDuration > 0f)
            StartCoroutine(FadeOutRoutine(active, fadeDuration));
        else
            active.Stop();
    }

    /// <summary>Pausa / reanuda la música.</summary>
    public void PauseMusic(bool pause)
    {
        AudioSource active = usingSourceA ? musicSource : musicSourceB;
        if (pause) active.Pause(); else active.UnPause();
    }

    // ─────────────────────────────────────────────
    // EFECTOS DE SONIDO
    // ─────────────────────────────────────────────

    /// <summary>Reproduce un SFX por nombre.</summary>
    public void PlaySFX(string soundName)
    {
        Sound s = GetSound(soundName);
        if (s != null) PlaySFXClip(s.clip, s.pitch, s.randomizePitch, s.pitchVariance);
    }

    /// <summary>Reproduce un SFX con volumen personalizado.</summary>
    public void PlaySFX(string soundName, float volumeScale)
    {
        Sound s = GetSound(soundName);
        if (s != null) sfxSource.PlayOneShot(s.clip, (isSfxMuted ? 0f : sfxVolume) * volumeScale);
    }

    /// <summary>Reproduce un AudioClip directamente como SFX.</summary>
    public void PlaySFXClip(AudioClip clip, float pitch = 1f, bool randomizePitch = false, float pitchVariance = 0.1f)
    {
        if (clip == null || isSfxMuted) return;

        float originalPitch  = sfxSource.pitch;
        sfxSource.pitch      = randomizePitch
            ? UnityEngine.Random.Range(pitch - pitchVariance, pitch + pitchVariance)
            : pitch;
        sfxSource.PlayOneShot(clip, sfxVolume);
        sfxSource.pitch = originalPitch;
    }

    /// <summary>Reproduce un SFX en una posición 3D del mundo.</summary>
    public void PlaySFXAt(string soundName, Vector3 position)
    {
        Sound s = GetSound(soundName);
        if (s != null) AudioSource.PlayClipAtPoint(s.clip, position, isSfxMuted ? 0f : sfxVolume);
    }

    // ─────────────────────────────────────────────
    // TOGGLE
    // ─────────────────────────────────────────────
    public void ToggleMusicMute() => IsMusicMuted = !isMusicMuted;
    public void ToggleSfxMute()   => IsSfxMuted   = !isSfxMuted;

    // ─────────────────────────────────────────────
    // HELPERS PRIVADOS
    // ─────────────────────────────────────────────
    private void ApplyMusicVolume()
    {
        float vol = isMusicMuted ? 0f : musicVolume;
        musicSource.volume  = vol;
        musicSourceB.volume = vol;
    }

    private Sound GetSound(string name)
    {
        Sound s = Array.Find(sounds, x => x.name == name);
        if (s == null) Debug.LogWarning($"[AudioManager] Sonido '{name}' no encontrado.");
        return s;
    }

    // ─────────────────────────────────────────────
    // COROUTINES
    // ─────────────────────────────────────────────
    private IEnumerator CrossfadeRoutine(AudioSource from, AudioSource to, float duration)
    {
        float elapsed = 0f;
        float targetVol = isMusicMuted ? 0f : musicVolume;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            from.volume = Mathf.Lerp(targetVol, 0f, t);
            to.volume   = Mathf.Lerp(0f, targetVol, t);
            yield return null;
        }

        from.Stop();
        from.volume = targetVol;
        to.volume   = targetVol;
    }

    private IEnumerator FadeOutRoutine(AudioSource source, float duration)
    {
        float startVol = source.volume;
        float elapsed  = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVol, 0f, elapsed / duration);
            yield return null;
        }

        source.Stop();
        source.volume = startVol;
    }
}