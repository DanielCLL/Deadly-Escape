using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Numerics;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
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
    public TextMeshProUGUI FlashlightBatery;
    private bool isActionsTextAviable = false;
    private bool isDescTextAviable = false;
    private bool isLoreTextAviable = false;
    private float descTimer = 0f;
    private float loreTimer = 0f;
    //private string[] loreTextBuffer;

    // Pausa
    public GameObject pauseMenuUI;
    public List<MonoBehaviour> scriptsToDisable; // scripts como CameraController, PlayerMovement, etc.

    private bool isPaused = false;

    // Inventario
    public List<string> Inventario;
    public GameObject inventoryUI;                 // Canvas del inventario
    //public MonoBehaviour[] scriptsToDisable;       // Scripts como movimiento y cámara

    private bool isInventoryOpen = false;

    // Jugador
    public GameObject PlayerGO;
    private bool playerSpotted = false;

    // Entorno
    public GameObject CablePC;
    private bool cablePuesto = false;

    // Audio
    public AudioSource SpottAudio;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
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
            if (MusicaSL == null) MusicaSL = GameObject.Find("MusicaSL").GetComponent<Slider>();
            if (SensibSL == null) SensibSL = GameObject.Find("SensibSL").GetComponent<Slider>();
            if (musicaValue == null) musicaValue = GameObject.Find("MusicaValue").GetComponent<TextMeshProUGUI>();
            if (sensibValue == null) sensibValue = GameObject.Find("SensibValue").GetComponent<TextMeshProUGUI>();
            porcMusica = MusicaSL.value;
            sensibilidad = SensibSL.value;

            musicaValue.text = Mathf.Floor(porcMusica).ToString() + "%";
            sensibValue.text = (Mathf.Floor(sensibilidad * 100) / 100).ToString();
        }
        else if (SceneManager.GetActiveScene().name == "Show")
        {
            if (ActionsText == null) ActionsText = GameObject.Find("ActionsText").GetComponent<TextMeshProUGUI>();
            if (DescText == null) DescText = GameObject.Find("DescText").GetComponent<TextMeshProUGUI>();
            if (LoreText == null) LoreText = GameObject.Find("LoreText").GetComponent<TextMeshProUGUI>();
            if (FlashlightBatery == null)  FlashlightBatery = GameObject.Find("FlashlightBatery").GetComponent<TextMeshProUGUI>();
            if (PlayerGO == null) PlayerGO = GameObject.Find("Player");
            if (CablePC == null) CablePC = GameObject.Find("Cable_Black");
            if (pauseMenuUI == null)
            {
                pauseMenuUI = GameObject.Find("PauseMenuUI");
                pauseMenuUI.SetActive(false);
            }
            if (inventoryUI == null)
            {
                inventoryUI = GameObject.Find("InventoryUI");
                inventoryUI.SetActive(false);
            }
            if (!scriptsToDisable.Contains(GameObject.Find("Player").GetComponent<PlayerController>())) scriptsToDisable.Add(GameObject.Find("Player").GetComponent<PlayerController>());

            ActionsText.gameObject.SetActive(isActionsTextAviable);
            DescText.gameObject.SetActive(isDescTextAviable);
            LoreText.gameObject.SetActive(isLoreTextAviable);
            CablePC.gameObject.SetActive(cablePuesto);

            if (PlayerGO.GetComponent<PlayerController>().HasFlashlight())
                FlashlightBatery.text = "Batería: " + Mathf.Floor(PlayerGO.GetComponent<PlayerController>().GetFlashlightBattery()).ToString() + " %";

            if (descTimer > 0)
            {
                descTimer -= Time.deltaTime;
                isDescTextAviable = true;
            }
            else
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

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                    Resume();
                else
                    Pause();
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                if (isInventoryOpen)
                    CloseInventory();
                else
                    OpenInventory();
            }
        }
    }

    public string GetActiveScene()
    {
        return SceneManager.GetActiveScene().name;
    }

    public bool GetPlayerSpotted()
    {
        return playerSpotted;
    }

    public void SpottPlayer()
    {
        if (!playerSpotted)
            SpottAudio.Play();
        playerSpotted = true;
    }

    /*****************************/
    //* SETS y GETS de Opciones *//
    /*****************************/
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
            PlayerGO.GetComponent<PlayerController>().GetFlashlight();
            setDescText("Pulsa [F] para usar la linterna.");
            setIsDescAviableTextTrue();
        }
        else if (s == "Batería pequeña")
        {
            PlayerGO.GetComponent<PlayerController>().AddBattery(50f);
        }
        else if (s == "Batería grande")
        {
            PlayerGO.GetComponent<PlayerController>().AddBattery(100f);
        }
        else if (s == "Tarjeta de seguridad")
        {
            setLoreText("Pone que pertenece a un tal \"Antonio Arcos\". ¿Me servirá de algo en el PC de abajo?");
            setIsLoreAviableTextTrue();
        }
        if (s == "Llave de la puerta principal")
        {
            setLoreText("¡Creo que por fin puedo salir de aquí!");
            setIsLoreAviableTextTrue();
        }
    }
    public void putWire()
    {
        cablePuesto = true;
    }

    public bool isWirePut()
    {
        return cablePuesto;
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

    public void OpenInventory()
    {
        inventoryUI.SetActive(true);
        Time.timeScale = 0f;
        SetScriptsEnabled(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isInventoryOpen = true;
    }

    public void CloseInventory()
    {
        inventoryUI.SetActive(false);
        Time.timeScale = 1f;
        SetScriptsEnabled(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isInventoryOpen = false;
    }

    void SetScriptsEnabled(bool value)
    {
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            script.enabled = value;
        }
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
