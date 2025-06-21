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
    private float flashlightBatery = 60;

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

        if (Input.GetKeyDown(KeyCode.F) && flashlightGet && flashlightBatery > 1)
        {
            flashlightActive = !flashlightActive;
        }

        if (flashlightActive && flashlightBatery > 1) flashlightBatery -= Time.deltaTime;
        else if (flashlightBatery <= 1) flashlightActive = false;

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
                    if (objetoGO.GetComponent<Door>().getLocked() && !GameManager.isOnInventory(objRequired))
                    {
                        GameManager.setDescText("Está cerrado. Necesito la " + objRequired + ".");
                        GameManager.setIsDescAviableTextTrue();
                    }
                    else if (objetoGO.GetComponent<Door>().getLocked() && GameManager.GetComponent<GameManager>().isOnInventory(objRequired))
                    {
                        //GameManager.GetComponent<GameManager>().useItem(objRequired);
                        objetoGO.GetComponent<Door>().Unlock();
                        GameManager.setDescText("Puerta desbloqueada.");
                        GameManager.setIsDescAviableTextTrue();
                    }
                    else
                    {
                        objetoGO.GetComponent<Door>().setOpen();
                    }
                }

                if (!objetoGO.GetComponent<Door>().getOpen())
                {
                    GameManager.setActionsText("[E]\nAbrir");
                    GameManager.setIsActionsAviableTextTrue();
                }
                else
                {
                    GameManager.setActionsText("[E]\nCerrar");
                    GameManager.setIsActionsAviableTextTrue();
                }
            }
            else if (objetoGO.layer == LayerMask.NameToLayer("Locker"))
            {
                GameManager.setActionsText("[E]\nInspeccionar");
                GameManager.setIsActionsAviableTextTrue();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    GameManager.setDescText("Aquí solo hay documentos sin valor.");
                    GameManager.setIsDescAviableTextTrue();
                }
            }
            else if (objetoGO.layer == LayerMask.NameToLayer("PC"))
            {
                GameManager.setActionsText("[E]\nInspeccionar");
                GameManager.setIsActionsAviableTextTrue();
                if (objetoGO.name == "PC_Monitor" && Input.GetKeyDown(KeyCode.E))
                {
                    if (!GameManager.isWirePut())
                    {
                        if (GameManager.isOnInventory("Cable de alimentación"))
                        {
                            GameManager.setDescText("Cable enchufado.");
                            GameManager.setIsDescAviableTextTrue();
                            GameManager.useItem("Cable de alimentación");
                            GameManager.putWire();
                        }
                        else
                        {
                            GameManager.setDescText("Le falta un cable de alimentación");
                            GameManager.setIsDescAviableTextTrue();
                        }
                    } else
                    {
                        if (objetoGO.GetComponent<PC>().AllUnlocked())
                        {
                            GameManager.setDescText("Ya no me sirve para nada más.");
                            GameManager.setIsDescAviableTextTrue();
                        }
                        else
                        {
                            if (GameManager.isOnInventory("Tarjeta azul"))
                            {
                                GameManager.useItem("Tarjeta azul");
                                objetoGO.GetComponent<PC>().UnlockBox(1);
                                GameManager.setDescText("\"Cajón 1 desbloqueado\"");
                                GameManager.setIsDescAviableTextTrue();
                            }
                            else if (GameManager.isOnInventory("Tarjeta amarilla"))
                            {
                                GameManager.useItem("Tarjeta amarilla");
                                objetoGO.GetComponent<PC>().UnlockBox(2);
                                GameManager.setDescText("\"Cajón 2 desbloqueado\"");
                                GameManager.setIsDescAviableTextTrue();
                            }
                            else if (GameManager.isOnInventory("Tarjeta roja"))
                            {
                                GameManager.useItem("Tarjeta roja");
                                objetoGO.GetComponent<PC>().UnlockBox(3);
                                GameManager.setDescText("\"Cajón 3 desbloqueado\"");
                                GameManager.setIsDescAviableTextTrue();
                            }
                            else if (GameManager.isOnInventory("Tarjeta de seguridad"))
                            {
                                GameManager.useItem("Tarjeta de seguridad");
                                objetoGO.GetComponent<PC>().UnlockBox(4);
                                GameManager.setDescText("\"Cajón 4 desbloqueado\"");
                                GameManager.setIsDescAviableTextTrue();
                            }
                            else
                            {
                                GameManager.setDescText("Está encendido, pero me hace falta una tarjeta de identificación.");
                                GameManager.setIsDescAviableTextTrue();
                            }
                        }
                    }
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    GameManager.setDescText("No funciona.");
                    GameManager.setIsDescAviableTextTrue();
                }
            }
            else if (objetoGO.layer == LayerMask.NameToLayer("Elevator"))
            {
                GameManager.setActionsText("[E]\nInspeccionar");
                GameManager.setIsActionsAviableTextTrue();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    GameManager.GetComponent<GameManager>().setDescText("No funciona.");
                    GameManager.GetComponent<GameManager>().setIsDescAviableTextTrue();
                }
            }
            else if (objetoGO.layer == LayerMask.NameToLayer("Collectable"))
            {
                GameManager.setActionsText("[E]\nCoger");
                GameManager.setIsActionsAviableTextTrue();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if ((objetoGO.GetComponent<Collectable>().GetName() == "Batería pequeña" || objetoGO.GetComponent<Collectable>().GetName() == "Batería grande")
                        && !flashlightGet)
                    {
                        GameManager.GetComponent<GameManager>().setDescText("Necesito una linterna.");
                        GameManager.GetComponent<GameManager>().setIsDescAviableTextTrue();
                    }
                    else
                    {
                        GameManager.setDescText(objetoGO.GetComponent<Collectable>().GetName());
                        GameManager.setIsDescAviableTextTrue();
                        GameManager.GetComponent<GameManager>().ActivateTextByName(objetoGO.GetComponent<Collectable>().GetName());
                        objetoGO.GetComponent<Collectable>().GetItem();
                    }
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

    public bool isFlashlightGet()
    {
        return flashlightGet;
    }

    public float getFlashlightBatery()
    {
        return flashlightBatery;
    }

    public void addBatery(float amount)
    {
        flashlightBatery = Mathf.Min(100.99999f, flashlightBatery + amount);
    }
}
