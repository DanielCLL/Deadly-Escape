using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameManager GameManager;

    public Camera playerCamera;
    public GameObject Flashlight, WhiteLight;
    public LayerMask interactables;

    private Rigidbody _rb;
    private float velocidad;
    private float mouseSensitivity;
    private float verticalRotation = 0f;
    private bool isCrouched = false;
    private bool flashlightActive = true;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        mouseSensitivity = GameManager.GetComponent<GameManager>().GetSensibilidad();
        velocidad = 1;
        _rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // Esconde el cursor y lo bloquea en el centro
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
        if (Input.GetKey(KeyCode.S)) transform.Translate(Vector3.back * velocidad * 0.8f * Time.deltaTime);
        if (Input.GetKey(KeyCode.A)) transform.Translate(Vector3.left * velocidad * 0.8f * Time.deltaTime);
        if (Input.GetKey(KeyCode.D)) transform.Translate(Vector3.right * velocidad * 0.8f * Time.deltaTime);

        if (Input.GetKey(KeyCode.LeftShift) && !isCrouched) velocidad = 5;
        else velocidad = 2;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            velocidad = 0.5f;
            isCrouched = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + 0.36f, transform.position.z);
            velocidad = 1;
            isCrouched = false;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlightActive = !flashlightActive;
        }

        WhiteLight.SetActive(flashlightActive);
        /*
        if (flashlightActive)
        {
            if (Flashlight.transform.position.y < cameraTransform.position.y - 0.345f)
            {
                Flashlight.transform.Translate(Vector3.forward * 3 * Time.deltaTime);
            }
        }
        else
        {
            if (Flashlight.transform.position.y > cameraTransform.position.y - 0.8f)
            {
                Flashlight.transform.Translate(Vector3.back * 3 * Time.deltaTime);
            }
        }
        */

        // Rotación de cámara y personaje
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Raycast del Jugador
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 1.5f, Color.blue);
        //Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * 3f, Color.blue);

        // Lanza el rayo y detecta colisiones
        if (Physics.Raycast(ray, out hit, 1.5f, interactables))
        {
            Debug.Log("Hit: " + hit.collider.name);
            // Aquí puedes interactuar con lo que se golpea

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Door"))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    hit.collider.gameObject.GetComponent<Door>().setOpen();
                }

                if (!hit.collider.GetComponent<Door>().getOpen())
                {
                    GameManager.setInfoText("[E]\nAbrir");
                    GameManager.setIsActionsAviableTextTrue();
                }
                else
                {
                    GameManager.setInfoText("[E]\nCerrar");
                    GameManager.setIsActionsAviableTextTrue();
                }
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Collectable"))
            {
                GameManager.setInfoText("[E]\nCoger");
                GameManager.setIsActionsAviableTextTrue();
                //hit.collider.gameObject.GetComponent<Collectable>()
            }
        } else
        {
            GameManager.setIsActionsAviableTextFalse();
        }
    }
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space)) _rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
    }
}
