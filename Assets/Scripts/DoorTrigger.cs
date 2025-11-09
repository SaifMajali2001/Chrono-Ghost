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

        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.2f);
    }
}
