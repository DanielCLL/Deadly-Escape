using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameManager GameManager;
    public AudioSource[] BackgroundMusic;

    private float porcMusica;
    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        porcMusica = GameManager.GetMusicaValue();
    }

    // Update is called once per frame
    void Update()
    {
        porcMusica = GameManager.GetMusicaValue();
        foreach (var item in BackgroundMusic)
        {
            item.volume = porcMusica / 100;
        }
    }
}
