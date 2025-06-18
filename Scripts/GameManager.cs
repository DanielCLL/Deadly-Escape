using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Variables de Opciones
    public Slider MusicaSL;
    public Slider SensibSL;
    public TextMeshProUGUI musicaValue;
    public TextMeshProUGUI sensibValue;

    private float porcMusica;
    private float sensibilidad;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        porcMusica = MusicaSL.value;
        sensibilidad = SensibSL.value;

        musicaValue.text = Mathf.Floor(porcMusica).ToString() + "%";
        sensibValue.text = (Mathf.Floor(sensibilidad * 100) / 100).ToString();
    }

    public float GetMusicaValue()
    {
        return porcMusica;
    }

    public float GetSensibilidad()
    {
        return sensibilidad;
    }

    public void RestablecerValues()
    {
        MusicaSL.value = 100f;
        SensibSL.value = 1f;
    }
}
