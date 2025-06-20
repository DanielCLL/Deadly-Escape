using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject ParentDoor;
    public float rotationSpeed = 90f;
    public float offsetRotationY = 0f;
    public float minY, maxY = 130f;
    public bool isOpen = false;
    public bool isLeft = true;
    public bool hasParent = true;

    //private float timer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        if (ParentDoor == null) ParentDoor = hasParent ? transform.parent.gameObject : this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float targetY;
        if (isOpen)
        {
            minY = offsetRotationY;
            targetY = (isLeft ? -maxY : maxY) + offsetRotationY;
        }
        else
            targetY = minY;

        // Obtener la rotación actual en euler angles
        Vector3 currentRotation = ParentDoor.transform.localRotation.eulerAngles;

        // Convertir el ángulo a rango [-180, 180] para evitar giros innecesarios
        float y = NormalizeAngle(currentRotation.y);

        // Interpolar hacia el ángulo deseado
        float newY = Mathf.MoveTowardsAngle(y, targetY, rotationSpeed * Time.deltaTime);

        // Mantener X y Z originales
        float originalX = currentRotation.x;
        float originalZ = currentRotation.z;

        ParentDoor.transform.localRotation = Quaternion.Euler(0f, newY, 0f);
    }
    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    public void setOpen()
    {
        //if (timer <= 0f)
        isOpen = !isOpen;
        //timer = 1f;
    }

    public bool getOpen()
    {
        return isOpen;
    }
}
