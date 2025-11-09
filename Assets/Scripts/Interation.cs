using UnityEngine;
using TMPro;

public class Interation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private string message = "Press E to interact";
    [SerializeField] private GameObject messagePanel;

    private void Start()
    {
        if (messagePanel != null)
            messagePanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
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
            if (messagePanel != null)
                messagePanel.SetActive(false);
        }
    }
}
