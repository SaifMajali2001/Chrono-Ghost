using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenu;
    private InputSystem_Actions playerInput;

    private void Awake()
    {
        playerInput = new InputSystem_Actions();
        Debug.Log("Pause script initialized");
    }

    private void Start()
    {
        // Ensure pause menu is hidden at start
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        else
        {
            Debug.LogError("Pause Menu reference is not set in the Inspector!");
        }
    }

    private void OnEnable()
    {
        playerInput.Enable();
        playerInput.UI.Pause.performed += OnPausePerformed;
        Debug.Log("Input system enabled");
    }

    private void OnDisable()
    {
        playerInput.UI.Pause.performed -= OnPausePerformed;
        playerInput.Disable();
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Pause action triggered");
        if (isPaused)
        {
            Resume();
        }
        else
        {
            PauseGame();
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
}
