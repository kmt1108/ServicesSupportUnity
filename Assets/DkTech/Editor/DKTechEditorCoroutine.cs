using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class DKTechEditorCoroutine
{
    /// <summary>
    /// Keeps track of the coroutine currently running.
    /// </summary>
    private IEnumerator enumerator;

    /// <summary>
    /// Keeps track of coroutines that have yielded to the current enumerator.
    /// </summary>
    private readonly List<IEnumerator> history = new List<IEnumerator>();

    private DKTechEditorCoroutine(IEnumerator enumerator)
    {
        this.enumerator = enumerator;
    }

    /// <summary>
    /// Creates and starts a coroutine.
    /// </summary>
    /// <param name="enumerator">The coroutine to be started</param>
    /// <returns>The coroutine that has been started.</returns>
    public static DKTechEditorCoroutine StartCoroutine(IEnumerator enumerator)
    {
        var coroutine = new DKTechEditorCoroutine(enumerator);
        coroutine.Start();
        return coroutine;
    }

    private void Start()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    /// <summary>
    /// Stops the coroutine.
    /// </summary>
    public void Stop()
    {
        if (EditorApplication.update == null) return;

        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnEditorUpdate()
    {
        if (enumerator.MoveNext())
        {
            // If there is a coroutine to yield for inside the coroutine, add the initial one to history and continue the second one
            if (enumerator.Current is IEnumerator)
            {
                history.Add(enumerator);
                enumerator = (IEnumerator)enumerator.Current;
            }
        }
        else
        {
            // Current coroutine has ended, check if we have more coroutines in history to be run.
            if (history.Count == 0)
            {
                // No more coroutines to run, stop updating.
                Stop();
            }
            // Step out and finish the code in the coroutine that yielded to it
            else
            {
                var index = history.Count - 1;
                enumerator = history[index];
                history.RemoveAt(index);
            }
        }
    }
}
