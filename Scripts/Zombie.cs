using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public GameManager GameManager;
    public EventSystemManager EventSystemManager;
    public Animator ZombieAnimation;
    public GameObject Player;
    public AudioSource[] AudioManager;
    public AudioSource[] ZombieSounds;
    public AudioSource Screamer;
    public NavMeshAgent agent;

    private enum ZombieState { Roaming, Waiting, ChasingDelay, Chasing, IdleAfterChase }
    private ZombieState currentState = ZombieState.Roaming;

    private float waitTimer = 0f;
    private float chaseDelayTimer = 1f;
    private float chaseTimer = 0f;
    private float idleTimer = 0f;

    public Transform[] destination;
    private Transform currentDestination;
    private int currentDestinationID;
    private bool playerVisible = false;

    [SerializeField] float visionRange = 10f;
    [SerializeField] float visionAngle = 60f;
    [SerializeField] int rayCount = 5;
    [SerializeField] LayerMask visionMask;

    private float timerSound = 15f;
    private float timerSoundRestart = 15f;
    private float maxDistance = 10f;


    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        EventSystemManager = GameObject.Find("EventSystem").GetComponent<EventSystemManager>();
        AudioManager = GameObject.Find("AudioManager").GetComponents<AudioSource>();
        ZombieSounds = GetComponents<AudioSource>();

        if (agent == null) agent = GetComponent<NavMeshAgent>();
        PickNewDestination();
        //transform.position = currentDestination.localPosition;
    }

    void Update()
    {
        playerVisible = GameManager.GetPlayerSpotted();

        CheckVision();

        if (timerSound > 0f)
        {
            timerSound -= Time.deltaTime;
        }
        else
        {
            int rndSound = UnityEngine.Random.Range(0, 3);
            ZombieSounds[rndSound].maxDistance = maxDistance;
            ZombieSounds[rndSound].Play();
            timerSound = timerSoundRestart;
        }

        switch (currentState)
        {
            case ZombieState.Roaming:
                ZombieAnimation.SetBool("isWalking", true);
                ZombieAnimation.SetBool("isRunning", false);
                agent.speed = 0.4f;
                if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    currentState = ZombieState.Waiting;
                    waitTimer = 3f;
                    agent.ResetPath();
                }

                if (playerVisible)
                {
                    currentState = ZombieState.ChasingDelay;
                    chaseDelayTimer = 1f;
                    agent.ResetPath();
                }
                break;

            case ZombieState.Waiting:
                waitTimer -= Time.deltaTime;
                ZombieAnimation.SetBool("isWalking", false);
                if (waitTimer <= 0f)
                {
                    PickNewDestination();
                    currentState = ZombieState.Roaming;
                }

                if (playerVisible)
                {
                    currentState = ZombieState.ChasingDelay;
                    chaseDelayTimer = 1f;
                }
                break;

            case ZombieState.ChasingDelay:
                ZombieAnimation.SetBool("isWalking", false);
                ZombieAnimation.SetBool("isRunning", false);
                // ROTAR HACIA EL JUGADOR
                Vector3 directionToPlayer = Player.transform.position - transform.position;
                directionToPlayer.y = 0f; // evita que gire verticalmente
                if (directionToPlayer.magnitude > 0.1f)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }

                chaseDelayTimer -= Time.deltaTime;
                if (chaseDelayTimer <= 0f)
                {
                    AudioManager[4].Play();
                    timerSoundRestart = 5f;
                    maxDistance = 100f;
                    currentState = ZombieState.Chasing;
                    chaseTimer = 5f;
                }
                break;

            case ZombieState.Chasing:
                ZombieAnimation.SetBool("isRunning", true);
                agent.speed = 4f;
                agent.SetDestination(Player.transform.position);

                if (playerVisible)
                {
                    chaseTimer = 10f; // Reinicia el tiempo si ve al jugador
                }
                else
                {
                    chaseTimer -= Time.deltaTime;
                    if (chaseTimer <= 0f)
                    {
                        currentState = ZombieState.IdleAfterChase;
                        idleTimer = 5f;
                        agent.ResetPath();
                    }
                }
                break;

            case ZombieState.IdleAfterChase:
                ZombieAnimation.SetBool("isRunning", false);
                ZombieAnimation.SetBool("isWalking", false);
                idleTimer -= Time.deltaTime;

                if (playerVisible)
                {
                    currentState = ZombieState.ChasingDelay;
                    chaseDelayTimer = 1f;
                }
                else if (idleTimer <= 0f)
                {
                    PickNewDestination();
                    timerSoundRestart = 15f;
                    maxDistance = 10f;
                    currentState = ZombieState.Roaming;
                }
                break;
        }
    }

    private void PickNewDestination()
    {
        int rndNum;
        do
        {
            rndNum = UnityEngine.Random.Range(1, 18);
        } while (rndNum == currentDestinationID);
        currentDestinationID = rndNum;
        currentDestination = destination[currentDestinationID];
        agent.SetDestination(currentDestination.position);
        Debug.Log("Voy al punto " + currentDestinationID);
    }

    void CheckVision()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        float halfAngle = visionAngle / 2f;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = -halfAngle + (visionAngle / (rayCount - 1)) * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, visionRange, visionMask))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.DrawRay(origin, dir * hit.distance, Color.green);
                    Debug.Log("¡Jugador avistado!");
                    playerVisible = true;

                }
                else
                {
                    Debug.DrawRay(origin, dir * visionRange, Color.red);
                }
            }
            else
            {
                Debug.DrawRay(origin, dir * visionRange, Color.gray);
            }
        }
    }

    public void SpottPlayer()
    {
        GameManager.SpottPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && playerVisible) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            EventSystemManager.DeadScene();
        }
    }
}