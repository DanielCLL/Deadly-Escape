using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventSystemManager : MonoBehaviour
{
    // Variables de opciones
    public GameObject MainMenuGO;
    public GameObject OpcionesGO;

    private bool menuOpcionesOn = false, menuCreditosOn = false;

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
            MainMenuGO.SetActive(!menuOpcionesOn && !menuCreditosOn);
            OpcionesGO.SetActive(menuOpcionesOn);
            if (Input.GetKeyDown(KeyCode.Escape) && (menuOpcionesOn || menuCreditosOn))
            {
                menuOpcionesOn = false;
                menuCreditosOn = false;
            }
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
        menuCreditosOn = true;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
