using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks all keys the player has picked up.
/// Attach to the Player root GameObject.
/// </summary>
public class KeyRing : MonoBehaviour
{
    private readonly HashSet<string> _keys = new();

    public void AddKey(string keyId)
    {
        _keys.Add(keyId);
        Debug.Log($"[KeyRing] Key added: {keyId}");
    }

    public bool HasKey(string keyId) => _keys.Contains(keyId);

    public void RemoveKey(string keyId) => _keys.Remove(keyId);
}
