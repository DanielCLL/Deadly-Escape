using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;

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
    public TextMeshProUGUI DescText;
    public TextMeshProUGUI LoreText;
    private bool isActionsTextAviable = false;
    private bool isDescTextAviable = false;
    private bool isLoreTextAviable = false;
    private float descTimer = 0f;
    private float loreTimer = 0f;
    //private string[] loreTextBuffer;

    // Inventario
    public List<string> Inventario;

    // Jugador
    public GameObject PlayerGO;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        Inventario = new List<string>();
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
            if (ActionsText == null)
            {
                if (GameObject.Find("ActionsText") != null)
                    ActionsText = GameObject.Find("ActionsText").GetComponent<TextMeshProUGUI>();
            }
            if (DescText == null)
            {
                if (GameObject.Find("DescText") != null)
                    DescText = GameObject.Find("DescText").GetComponent<TextMeshProUGUI>();
            }
            if (LoreText == null)
            {
                if (GameObject.Find("LoreText") != null)
                    LoreText = GameObject.Find("LoreText").GetComponent<TextMeshProUGUI>();
            }
            if (PlayerGO == null) PlayerGO = GameObject.Find("Player");
            ActionsText.gameObject.SetActive(isActionsTextAviable);
            DescText.gameObject.SetActive(isDescTextAviable);
            LoreText.gameObject.SetActive(isLoreTextAviable);

            if (descTimer > 0)
            {
                descTimer -= Time.deltaTime;
                isDescTextAviable = true;
            } else
            {
                isDescTextAviable = false;
            }

            if (loreTimer > 0)
            {
                loreTimer -= Time.deltaTime;
                isLoreTextAviable = true;
            }
            else
            {
                isLoreTextAviable = false;
            }
        }
    }

    //* SETS y GETS de Opciones *//
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

    /********************************/
    //* SETS y GETS de ActionsText *//
    /********************************/

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
    public void setActionsText(string s)
    {
        ActionsText.text = s;
    }

    /*****************************/
    //* SETS y GETS de DescText *//
    /*****************************/

    public bool getIsDescAviableText()
    {
        return isDescTextAviable;
    }
    public void setIsDescAviableTextTrue()
    {
        descTimer = 3f;
        isDescTextAviable = true;
    }
    public void setIsDescAviableTextFalse()
    {
        isDescTextAviable = false;
    }

    public void setDescText(string s)
    {
        DescText.text = s;
    }

    /*****************************/
    //* SETS y GETS de LoreText *//
    /*****************************/

    public bool getIsLoreAviableText()
    {
        return isLoreTextAviable;
    }
    public void setIsLoreAviableTextTrue()
    {
        loreTimer = 3f;
        isLoreTextAviable = true;
    }
    public void setIsLoreAviableTextFalse()
    {
        isLoreTextAviable = false;
    }

    public void setLoreText(string s)
    {
        LoreText.text = s;
    }

    /*******************************/
    //* SETS y GETS de Inventario *//
    /*******************************/

    public bool isOnInventory(string objeto)
    {
        return Inventario.Contains(objeto);
    }

    public void getItem(string objeto)
    {
        Inventario.Add(objeto);
    }

    public void useItem(string objeto)
    {
        Inventario.Remove(objeto);
    }

    public void ActivateTextByName(string s)
    {
        if (s == "Linterna")
        {
            PlayerGO.GetComponent<PlayerController>().getFlashlight();
            setDescText("Pulsa [F] para usar la linterna.");
            setIsDescAviableTextTrue();
        }
        else if (s == "Llave del guarda")
        {
            setDescText(s);
            setIsDescAviableTextTrue();
        }
        else if (s == "Llave del sótano")
        {
            setDescText(s);
            setIsDescAviableTextTrue();
        }
        if (s == "Llave de la Puerta Principal")
        {
            setDescText("Llave de la Puerta Principal");
            setIsDescAviableTextTrue();
            setLoreText("¡Creo que por fin puedo salir de aquí!");
            setIsLoreAviableTextTrue();
        }
    }
}
