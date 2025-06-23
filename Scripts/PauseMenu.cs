using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public MonoBehaviour[] scriptsToDisable; // scripts como CameraController, PlayerMovement, etc.

    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI = GameObject.Find("PauseMenuUI");
        scriptsToDisable[0] = GameObject.Find("Player").GetComponent<PlayerController>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        Debug.Log("Continua");
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        SetScriptsEnabled(true);
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        Debug.Log("Pausa");
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        SetScriptsEnabled(false);
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void SetScriptsEnabled(bool value)
    {
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            script.enabled = value;
        }
    }
}
