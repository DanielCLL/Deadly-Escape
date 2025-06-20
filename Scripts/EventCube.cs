using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EventCube : MonoBehaviour
{
    public GameManager GameManager;
    public int id;

    private float timer = 0f;
    private bool isTrigger = false;
    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (id == 0 && isTrigger)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            } else
            {
                GameManager.setDescText("Pulsa [LShift] para correr.");
                GameManager.setIsDescAviableTextTrue();
                Destroy(this.gameObject);
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (id == 0)
            {
                GameManager.setLoreText("¿Cómo he acabado en este lugar?");
                GameManager.setIsLoreAviableTextTrue();
                timer = 3f;
                isTrigger = true;
            }
        }
    }
}
