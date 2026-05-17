using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that holds the list of tasks shown in the notebook HUD.
/// Create via: Assets > Create > Puzzle > Task List
/// </summary>
[CreateAssetMenu(menuName = "Puzzle/Task List", fileName = "TaskList")]
public class TaskData : ScriptableObject
{
    [System.Serializable]
    public class Task
    {
        [Tooltip("Short description shown in the notebook.")]
        public string description;

        [HideInInspector]
        public bool isCompleted;
    }

    public List<Task> tasks = new();

    /// <summary>Marks the task at the given index as completed.</summary>
    public void CompleteTask(int index)
    {
        if (index >= 0 && index < tasks.Count)
            tasks[index].isCompleted = true;
    }

    /// <summary>Marks the first task whose description matches (case-insensitive).</summary>
    public void CompleteTask(string description)
    {
        foreach (var t in tasks)
        {
            if (string.Equals(t.description, description, System.StringComparison.OrdinalIgnoreCase))
            {
                t.isCompleted = true;
                return;
            }
        }
        Debug.LogWarning($"[TaskData] No task found with description: '{description}'");
    }

    /// <summary>Resets all tasks to incomplete (call when restarting a level).</summary>
    public void ResetAll()
    {
        foreach (var t in tasks)
            t.isCompleted = false;
    }
}
