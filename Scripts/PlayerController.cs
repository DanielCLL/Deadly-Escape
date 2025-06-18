using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject GameManager;

    public Transform cameraTransform;

    private float velocidad;
    private float mouseSensitivity;
    private bool isCrouched = false;
    private Rigidbody _rb;
    private float verticalRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager");

        mouseSensitivity = GameManager.GetComponent<GameManager>().GetSensibilidad();
        velocidad = 1;
        cameraTransform = GameObject.Find("Main Camera").transform;
        _rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // Esconde el cursor y lo bloquea en el centro
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(cameraTransform.position, cameraTransform.position * 3f, Color.blue);

        if (Input.GetKey(KeyCode.W)) transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
        if (Input.GetKey(KeyCode.S)) transform.Translate(Vector3.back * velocidad * 0.8f * Time.deltaTime);
        if (Input.GetKey(KeyCode.A)) transform.Translate(Vector3.left * velocidad * 0.8f * Time.deltaTime);
        if (Input.GetKey(KeyCode.D)) transform.Translate(Vector3.right * velocidad * 0.8f * Time.deltaTime);

        if (Input.GetKey(KeyCode.LeftShift) && !isCrouched) velocidad = 5;
        else velocidad = 2;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            cameraTransform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            velocidad = 0.5f;
            isCrouched = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            cameraTransform.position = new Vector3(transform.position.x, transform.position.y + 0.36f, transform.position.z);
            velocidad = 1;
            isCrouched = false;
        }

        // Rotación de cámara y personaje
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space)) _rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
    }
}
