using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Variables de Opciones
    public Slider MusicaSL;
    public Slider SensibSL;
    public TextMeshProUGUI musicaValue;
    public TextMeshProUGUI sensibValue;

    private float porcMusica = 100;
    private float sensibilidad = 1;

    // Variables del juego
    // Canvas
    public TextMeshProUGUI ActionsText;
    private bool isActionsTextAviable = false;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            porcMusica = MusicaSL.value;
            sensibilidad = SensibSL.value;

            musicaValue.text = Mathf.Floor(porcMusica).ToString() + "%";
            sensibValue.text = (Mathf.Floor(sensibilidad * 100) / 100).ToString();
        }
        else if (SceneManager.GetActiveScene().name == "Show")
        {
            if (ActionsText != null)
            {
                if (GameObject.Find("ActionsText") != null)
                    ActionsText = GameObject.Find("ActionsText").GetComponent<TextMeshProUGUI>();
            }
            ActionsText.gameObject.SetActive(isActionsTextAviable);
        }
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

    public bool getIsActionsAviableText()
    {
        return isActionsTextAviable;
    }
    public void setIsActionsAviableTextTrue()
    {
        isActionsTextAviable = true;
    }
    public void setIsActionsAviableTextFalse()
    {
        isActionsTextAviable = false;
    }
    public void setInfoText(string s)
    {
        ActionsText.text = s;
    }
}
