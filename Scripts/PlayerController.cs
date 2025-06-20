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
    private bool flashlightGet = false;
    private bool flashlightActive = false;

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

        if (Input.GetKeyDown(KeyCode.F) && flashlightGet)
        {
            flashlightActive = !flashlightActive;
        }

        Flashlight.SetActive(flashlightGet);
        WhiteLight.SetActive(flashlightActive);
        /*
        if (flashlightGet) {
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
            GameObject objetoGO = hit.collider.gameObject;
            Debug.Log("Hit: " + hit.collider.name);
            // Aquí puedes interactuar con lo que se golpea

            if (objetoGO.layer == LayerMask.NameToLayer("Door"))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    string objRequired = objetoGO.GetComponent<Door>().getLockerRequires();
                    if (objetoGO.GetComponent<Door>().getLocked() && !GameManager.GetComponent<GameManager>().isOnInventory(objRequired))
                    {
                        GameManager.GetComponent<GameManager>().setDescText("Está cerrado. Necesito la " + objRequired + ".");
                        GameManager.GetComponent<GameManager>().setIsDescAviableTextTrue();
                    }
                    else if (objetoGO.GetComponent<Door>().getLocked() && GameManager.GetComponent<GameManager>().isOnInventory(objRequired))
                    {
                        GameManager.GetComponent<GameManager>().useItem(objRequired);
                        objetoGO.GetComponent<Door>().Unlock();
                        GameManager.GetComponent<GameManager>().setDescText("Puerta desbloqueada.");
                        GameManager.GetComponent<GameManager>().setIsDescAviableTextTrue();
                    }
                    else
                    {
                        objetoGO.GetComponent<Door>().setOpen();
                    }
                }

                if (!objetoGO.GetComponent<Door>().getOpen())
                {
                    GameManager.setActionsText("[E] Abrir");
                    GameManager.setIsActionsAviableTextTrue();
                }
                else
                {
                    GameManager.setActionsText("[E] Cerrar");
                    GameManager.setIsActionsAviableTextTrue();
                }
            }
            else if (objetoGO.layer == LayerMask.NameToLayer("Locker"))
            {
                GameManager.setActionsText("[E] Inspeccionar");
                GameManager.setIsActionsAviableTextTrue();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    GameManager.GetComponent<GameManager>().setDescText("Aquí solo hay documentos sin valor.");
                    GameManager.GetComponent<GameManager>().setIsDescAviableTextTrue();
                }
            }
            else if (objetoGO.layer == LayerMask.NameToLayer("PC"))
            {
                GameManager.setActionsText("[E] Inspeccionar");
                GameManager.setIsActionsAviableTextTrue();
                if (objetoGO.name == "PC_Monitor (1)" && Input.GetKeyDown(KeyCode.E))
                {
                    GameManager.GetComponent<GameManager>().setDescText("No funciona.");
                    GameManager.GetComponent<GameManager>().setIsDescAviableTextTrue();
                }
                else if (objetoGO.name == "PC_Monitor" && Input.GetKeyDown(KeyCode.E))
                {
                    GameManager.GetComponent<GameManager>().setDescText("Está encendido, pero me hace falta una contraseña.");
                    GameManager.GetComponent<GameManager>().setIsDescAviableTextTrue();
                }
            }
            else if (objetoGO.layer == LayerMask.NameToLayer("Elevator"))
            {
                GameManager.setActionsText("[E] Inspeccionar");
                GameManager.setIsActionsAviableTextTrue();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    GameManager.GetComponent<GameManager>().setDescText("No funciona.");
                    GameManager.GetComponent<GameManager>().setIsDescAviableTextTrue();
                }
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Collectable"))
            {
                GameManager.setActionsText("[E] Coger");
                GameManager.setIsActionsAviableTextTrue();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    GameManager.GetComponent<GameManager>().ActivateTextByName(hit.collider.gameObject.GetComponent<Collectable>().GetName());
                    hit.collider.GetComponent<Collectable>().GetItem();
                }
            }
            else
            {
                GameManager.setIsActionsAviableTextFalse();
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

    public void getFlashlight()
    {
        flashlightGet = true;
    }
}
