using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Centralizes GameObject activation/deactivation.
    /// Access from anywhere via ObjectManager.Instance.
    /// </summary>
    public class ObjectManager : MonoBehaviour
    {
        public static ObjectManager Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }

        // ── Public API ────────────────────────────────────────────────────────────

        public void Activate(GameObject obj)   => SetActive(obj, true);
        public void Deactivate(GameObject obj) => SetActive(obj, false);

        public void Activate(string objName)   => SetActive(objName, true);
        public void Deactivate(string objName) => SetActive(objName, false);

        // ── Internal ──────────────────────────────────────────────────────────────

        void SetActive(GameObject obj, bool active)
        {
            if (obj == null) { Debug.LogWarning("[ObjectManager] GameObject is null."); return; }
            obj.SetActive(active);
        }

        void SetActive(string objName, bool active)
        {
            var obj = GameObject.Find(objName);
            if (obj == null) { Debug.LogWarning($"[ObjectManager] GameObject '{objName}' not found."); return; }
            obj.SetActive(active);
        }
    }
}