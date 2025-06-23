using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VolumeControl : MonoBehaviour
{
    public Slider volumeSlider;
    public AudioSource audioSource;

    void Start()
    {
        // Asociar el evento de cambio de escena
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (audioSource != null && volumeSlider != null)
        {
            volumeSlider.value = audioSource.volume * 100f;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // Verificar en qué escena estamos al inicio
        HandleSceneSound(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleSceneSound(scene.name);
    }

    void HandleSceneSound(string sceneName)
    {
        if (sceneName == "MainMenu")
        {
            if (audioSource != null && !audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    public void SetVolume(float volumePercent)
    {
        if (audioSource != null)
            audioSource.volume = volumePercent / 100f;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
