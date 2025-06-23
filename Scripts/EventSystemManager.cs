using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventSystemManager : MonoBehaviour
{

    // Variables de opciones
    public GameObject MainMenuGO;
    public GameObject OpcionesGO;
    public GameObject GameTitle;
    public GameObject creditsGO;
    public GameObject CreditosTxt;
    public GameObject pulsaEscText;
    public float speed = 1f;        // Velocidad de subida

    private bool menuOpcionesOn = false, menuCreditosOn = false;
    
    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (MainMenuGO == null) MainMenuGO = GameObject.Find("MainMenu");
            if (OpcionesGO == null) OpcionesGO = GameObject.Find("Opciones");
            if (GameTitle == null) GameTitle = GameObject.Find("GameTitle");
            if (creditsGO == null) creditsGO = GameObject.Find("Creditos");
            if (CreditosTxt == null) CreditosTxt = GameObject.Find("CreditsTxt");
            if (pulsaEscText == null) pulsaEscText = GameObject.Find("PulsaEsc");
            MainMenuGO.SetActive(!menuOpcionesOn && !menuCreditosOn);
            OpcionesGO.SetActive(menuOpcionesOn);
            creditsGO.SetActive(menuCreditosOn);
            //pulsaEscText.SetActive(menuCreditosOn);
            if (Input.GetKeyDown(KeyCode.Escape) && (menuOpcionesOn || menuCreditosOn))
            {
                creditsGO.SetActive(false);
                GameTitle.SetActive(true);
                pulsaEscText.SetActive(false);
                menuOpcionesOn = false;
                menuCreditosOn = false;
            }

            if (menuCreditosOn)
            {
                GameTitle.SetActive(false);
                CreditosTxt.transform.Translate(Vector3.up * speed * Time.deltaTime);
            }
        }
        else if (SceneManager.GetActiveScene().name == "End")
        {
            if (CreditosTxt == null) CreditosTxt = GameObject.Find("CreditsTxt");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (Input.GetKeyDown(KeyCode.Escape)) {
                SceneManager.LoadScene("MainMenu");
            }

            CreditosTxt.transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Show");
    }
    public void MostrarOpciones()
    {
        menuOpcionesOn = true;
    }
    public void OcultarOpciones()
    {
        menuOpcionesOn = false;
    }
    public void MostrarCreditos()
    {
        CreditosTxt.transform.localPosition = new Vector3(0f,-620f,0f);
        menuCreditosOn = true;
    }
    public void EndGame()
    {
        SceneManager.LoadScene("End");
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void DeadScene()
    {
        SceneManager.LoadScene("DeadScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
