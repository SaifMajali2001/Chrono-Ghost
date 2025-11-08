using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class DoorTrigger : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private string sceneToLoad;

    public string SceneName => sceneToLoad;
    public string playerTag = "Player";

    private void Reset()
    {
        // If this component is added in the editor, ensure there's a Collider2D and it's a trigger
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning($"{name}: sceneToLoad not set on DoorTrigger.");
            return;
        }

        // Load the requested scene by name. Make sure the scene is added to Build Settings.
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a small icon in the editor so it's easier to find door triggers
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.2f);
    }
}
