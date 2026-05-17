using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that defines the levels shown in the Level Selection menu.
/// Create via: Assets > Create > Puzzle > Level Menu Data
/// </summary>
[CreateAssetMenu(menuName = "Puzzle/Level Menu Data", fileName = "LevelMenuData")]
public class LevelMenuData : ScriptableObject
{
    [System.Serializable]
    public class LevelEntry
    {
        [Tooltip("Name shown on the button.")]
        public string displayName;

        [Tooltip("Exact scene name as registered in Build Settings.")]
        public string sceneName;

        [Tooltip("If false the button is greyed out and non-interactive.")]
        public bool isUnlocked = true;
    }

    public List<LevelEntry> levels = new();
}
