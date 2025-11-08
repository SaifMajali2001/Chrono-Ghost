using UnityEngine;
using TMPro;

public class Interation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private string message = "Press E to interact";
    [SerializeField] private GameObject messagePanel;

    private void Start()
    {
        // Hide message panel at start
        if (messagePanel != null)
            messagePanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Show message when player enters trigger
            if (messagePanel != null)
                messagePanel.SetActive(true);
            if (messageText != null)
                messageText.text = message;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Hide message when player exits trigger
            if (messagePanel != null)
                messagePanel.SetActive(false);
        }
    }
}
