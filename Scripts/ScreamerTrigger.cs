using System.Collections;
using UnityEngine;

public class ScreamerTrigger : MonoBehaviour
{
    public GameObject screamerUI;     // Imagen del screamer en el Canvas
    public AudioSource audioSource;   // Audio del grito
    public float duration = 1f;       // Duración del screamer en pantalla

    private bool alreadyTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !alreadyTriggered)
        {
            alreadyTriggered = true;
            StartCoroutine(ShowScreamer());
        }
    }

    IEnumerator ShowScreamer()
    {
        screamerUI.SetActive(true);
        audioSource.Play();
        yield return new WaitForSecondsRealtime(duration);
        screamerUI.SetActive(false);
    }
}
