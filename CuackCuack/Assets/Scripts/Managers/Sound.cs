using UnityEngine;

/// <summary>
/// Representa un sonido registrado en el AudioManager.
/// Se configura desde el Inspector de Unity.
/// </summary>
[System.Serializable]
public class Sound
{
    [Tooltip("Nombre único para referenciar este sonido en código.")]
    public string name;

    [Tooltip("El AudioClip a reproducir.")]
    public AudioClip clip;

    [Tooltip("Pitch base del sonido (1 = normal).")]
    [Range(0.1f, 3f)]
    public float pitch = 1f;

    [Tooltip("¿Aleatorizar el pitch ligeramente para más naturalidad?")]
    public bool randomizePitch = false;

    [Tooltip("Variación de pitch cuando randomizePitch está activo.")]
    [Range(0f, 0.5f)]
    public float pitchVariance = 0.1f;

    [Tooltip("¿Es música de fondo? Solo informativo, no afecta la reproducción.")]
    public bool isMusic = false;
}