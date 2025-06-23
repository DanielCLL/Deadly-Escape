
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    public GameManager GameManager;
    public Camera playerCamera;
    public GameObject Flashlight, WhiteLight;
    public LayerMask interactables;

    [Header("Audio")]
    public AudioSource[] AudioManager;
    public AudioSource[] FlashlightAudio;

    [Header("Movimiento")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float crouchSpeed = 0.5f;
    public float jumpHeight = 0.2f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 100f;

    [Header("Linterna")]
    public float flashlightBattery = 60f;

    private CharacterController controller;
    private Vector3 velocity;
    private float verticalRotation = 0f;
    private bool isCrouched = false;
    private bool flashlightGet = false;
    private bool flashlightActive = false;

    private float originalCameraHeight;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        AudioManager = GameObject.Find("AudioManager").GetComponents<AudioSource>();
        FlashlightAudio = Flashlight.GetComponents<AudioSource>();

        mouseSensitivity = GameManager.GetSensibilidad();
        originalCameraHeight = playerCamera.transform.localPosition.y;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleFlashlight();
        HandleInteraction();
    }

    void HandleMovement()
    {
        float speed = isCrouched ? crouchSpeed : (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        controller.Move(move * speed * Time.deltaTime);

        // Saltar
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Agacharse
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouched = true;
            AdjustCameraHeight(-0.4f);
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouched = false;
            AdjustCameraHeight(0.4f);
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void AdjustCameraHeight(float offset)
    {
        Vector3 pos = playerCamera.transform.localPosition;
        pos.y += offset;
        playerCamera.transform.localPosition = pos;
    }

    void HandleFlashlight()
    {
        if (Input.GetKeyDown(KeyCode.F) && flashlightGet && flashlightBattery > 1f)
        {
            flashlightActive = !flashlightActive;
            FlashlightAudio[0].volume = GameManager.GetMusicaValue() / 100f;
            FlashlightAudio[0].Play();

            if (flashlightActive)
            {
                FlashlightAudio[1].volume = GameManager.GetMusicaValue() / 100f;
                FlashlightAudio[1].Play();
            }
            else
            {
                FlashlightAudio[1].Stop();
            }
        }

        if (flashlightActive)
        {
            flashlightBattery -= Time.deltaTime;
            if (flashlightBattery <= 1f)
            {
                flashlightActive = false;
                FlashlightAudio[1].Stop();
            }
        }

        Flashlight.SetActive(flashlightGet);
        WhiteLight.SetActive(flashlightActive);
    }

    void HandleInteraction()
    {
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
                        AudioManager[2].Play();
                    }
                    else
                    {
                        objetoGO.GetComponent<Door>().setOpen();
                        if (objetoGO.name == "Door")
                        {
                            if (objetoGO.GetComponent<Door>().getOpen())
                                AudioManager[1].Play();
                            else
                                AudioManager[3].Play();
                        }
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
                    }
                    else
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
        }
        else
        {
            GameManager.setIsActionsAviableTextFalse();
        }
    }

    // === Métodos públicos ===
    public void GetFlashlight() => flashlightGet = true;
    public bool HasFlashlight() => flashlightGet;
    public float GetFlashlightBattery() => flashlightBattery;
    public void AddBattery(float amount) => flashlightBattery = Mathf.Min(100f, flashlightBattery + amount);
}