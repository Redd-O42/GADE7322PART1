using TMPro;
using UnityEngine;

public class AIBehavior : MonoBehaviour
{
  

    public float movementSpeed = 5f;
    public float rotationSpeed = 5f;

    private UnityEngine.AI.NavMeshAgent agent; 
    private GameObject player; 
    private GameObject aiFlag;  
    private GameObject flagStandAI;
    private enum State
    {
        Idle,       // AI is idle, waiting for player
        Chase,      // AI is chasing the player
        GrabFlag,   // AI is grabbing the flag
        ReturnFlag  // AI is returning the flag to the stand
    }

    private State currentState; // Current state of the AI

    void Start()
    {
    
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("aiAgent");
        aiFlag = GameObject.FindGameObjectWithTag("aiFlag");
        flagStandAI = GameObject.FindGameObjectWithTag("FlagStandAI");

       
        currentState = State.Idle;
    }
    void MoveTowards(Vector3 targetPosition)
    {
        // Calculate direction to target
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Calculate rotation towards target
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        // Smoothly rotate towards target
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Move towards target
        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
    }
    void Update()
    {
        // State machine logic
        switch (currentState)
        {
            case State.Idle:
                // Check if the player is in sight, transition to Chase if true
                if (PlayerInSight())
                {
                    currentState = State.Chase;
                }
                break;

            case State.Chase:
                agent.SetDestination(aiFlag.transform.position);

                // If close enough to the flag, transition to GrabFlag
                if (Vector3.Distance(transform.position, aiFlag.transform.position) < 1.5f)
                {
                    currentState = State.GrabFlag;
                }
                break;

            case State.GrabFlag:
                agent.SetDestination(aiFlag.transform.position);

                if (Vector3.Distance(transform.position, aiFlag.transform.position) < 1.5f)
                {
                    player.transform.parent = transform; // Attach the flag to the AI
                    currentState = State.ReturnFlag;
                }
                break;

            case State.ReturnFlag:
                agent.SetDestination(flagStandAI.transform.position);

                // If close enough to the flag stand, drop the flag and transition to Idle
                if (Vector3.Distance(transform.position, flagStandAI.transform.position) < 1.5f)
                {
                    aiFlag.transform.parent = null; // Detach the flag from the AI
                    aiFlag.transform.position = flagStandAI.transform.position;
                    currentState = State.Idle;
                }
                break;
        }
    }
    
    bool PlayerInSight()
    {
        // Perform raycast to check if the player is in sight
        RaycastHit hit;
        if (Physics.Raycast(transform.position, aiFlag.transform.position - transform.position, out hit))
        {
            if (hit.collider.CompareTag("aiFlag"))
            {
                return true;
            }
        }
        return false;
    }
}