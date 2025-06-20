using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public GameManager GameManager;
    public string collectableName;
    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetItem()
    {
        GameManager.getItem(collectableName);
        Destroy(this.gameObject);
    }

    public string GetName()
    {
        return collectableName;
    }
}
