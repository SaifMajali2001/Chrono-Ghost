using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class VictoryTrigger : MonoBehaviour
{
    [Header("Win UI")]
    public GameObject winPanel;

    [Header("Scene / Timing")]
    public string mainMenuSceneName = "MainMenu";
    public float delayBeforeLoad = 3f;

    [Header("Behavior")]
    public bool disablePlayerController = true;

    private bool triggered = false;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        StartCoroutine(HandleVictory(other.gameObject));
    }

    private IEnumerator HandleVictory(GameObject player)
    {
        if (winPanel == null)
        {
            var found = GameObject.Find("Win");
            if (found != null) winPanel = found;
        }

        if (winPanel != null)
            winPanel.SetActive(true);

        if (disablePlayerController)
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.enabled = false;

            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.simulated = false;
        }

        yield return new WaitForSecondsRealtime(delayBeforeLoad);

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
