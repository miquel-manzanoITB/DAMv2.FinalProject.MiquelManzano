using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Place this on the Glasses GameObject. When the player interacts with them
/// the puzzle is solved: the blur material property is set to 0, a task is
/// completed and a win event fires.
///
/// Requires: Interactable component on the same GameObject.
/// </summary>
[RequireComponent(typeof(Interactable))]
public class GlassesPuzzle : MonoBehaviour
{
    [Header("Blur Material")]
    [Tooltip("El material que tiene el shader de blur (BlurGraph).")]
    public Material blurMaterial;

    [Tooltip("Nombre de la propiedad Blur en el shader. Por defecto '_Blur'.")]
    public string blurProperty = "_Blur";

    [Tooltip("Duración en segundos de la transición de blur a 0. 0 = instantáneo.")]
    public float blurFadeDuration = 1f;

    [Header("Task")]
    [Tooltip("Index in TaskData to mark complete when glasses are found.")]
    public int taskIndex = 0;

    [Header("Messages")]
    public string foundMessage = "!Has encontrado las gafas!";

    [Header("Events")]
    public UnityEvent onGlassesFound;

    // ── Internal ──────────────────────────────────────────────────────────────

    private Interactable _interactable;
    private float _blurFrom;
    private float _elapsed;
    private bool _fading;

    void Awake()
    {
        _interactable = GetComponent<Interactable>();
        _interactable.hintText = "Coger E";
    }

    void OnEnable()  => _interactable.onInteract.AddListener(OnFound);
    void OnDisable() => _interactable.onInteract.RemoveListener(OnFound);

    void Update()
    {
        if (!_fading) return;

        _elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsed / blurFadeDuration);
        blurMaterial.SetFloat(blurProperty, Mathf.Lerp(_blurFrom, 0f, t));

        if (t >= 1f) _fading = false;
    }

    void OnFound()
    {
        // Fade blur to 0
        if (blurMaterial != null)
        {
            if (blurFadeDuration > 0f)
            {
                _blurFrom = blurMaterial.GetFloat(blurProperty);
                _elapsed = 0f;
                _fading = true;
            }
            else
            {
                blurMaterial.SetFloat(blurProperty, 0f);
            }
        }

        // Show message
        Managers.UIManager.Instance?.ShowMessage(foundMessage);

        // Complete task in notebook
        var notebook = FindFirstObjectByType<Managers.NotebookUI>();
        if (notebook?.taskData != null)
        {
            notebook.taskData.CompleteTask(taskIndex);
            Managers.UIManager.Instance?.RefreshNotebook();
        }

        onGlassesFound?.Invoke();

        gameObject.SetActive(false);
    }
}