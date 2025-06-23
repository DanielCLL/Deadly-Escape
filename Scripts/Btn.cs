using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Btn : MonoBehaviour
{
    public GameManager GameManager;
    //public Button btn;
    public string btnName;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (btnName == "ContinuarPausa")
            GetComponent<Button>().onClick.AddListener(Evento);
    }

    void Evento()
    {
        GameManager.Resume();
    }
}

