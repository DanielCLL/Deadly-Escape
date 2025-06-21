using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC : MonoBehaviour
{
    public GameManager GameManager;
    public GameObject Box1, Box2, Box3, Box4;

    private float z_Target = -0.4f;
    private bool box1 = false, box2 = false, box3 = false, box4 = false;
    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        /*
        if (Box1 != null) Box1 = GameObject.Find("SheetRackCase_9");
        if (Box2 != null) Box2 = GameObject.Find("SheetRackCase_10");
        if (Box3 != null) Box3 = GameObject.Find("SheetRackCase_11");
        if (Box4 != null) Box4 = GameObject.Find("SheetRackCase_12");
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (box1) Box1.transform.localPosition = new Vector3(Box1.transform.localPosition.x, Box1.transform.localPosition.y, z_Target);
        if (box2) Box2.transform.localPosition = new Vector3(Box2.transform.localPosition.x, Box2.transform.localPosition.y, z_Target);
        if (box3) Box3.transform.localPosition = new Vector3(Box3.transform.localPosition.x, Box3.transform.localPosition.y, z_Target);
        if (box4) Box4.transform.localPosition = new Vector3(Box4.transform.localPosition.x, Box4.transform.localPosition.y, z_Target);
    }

    public void UnlockBox(int id)
    {
        if (id == 1) box1 = true;
        else if (id == 2) box2 = true;
        else if (id == 3) box3 = true;
        else if (id == 4) box4 = true;
    }

    public bool AllUnlocked()
    {
        return box1 && box2 && box3 && box4;
    }
}
