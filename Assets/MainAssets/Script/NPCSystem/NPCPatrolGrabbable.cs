using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCPatrol : MonoBehaviour
{
    private NavMeshAgent agent;
    private Rigidbody rb;
    private Vector3[] patrolPoints;
    private int currentPatrolIndex = 0;
    private Animator animator;

    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private int numPatrolPoints = 5;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private float airThreshold = 1.0f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rotationSpeed = 720f; // Velocità di rotazione in gradi al secondo

    private Coroutine patrolCoroutine;

    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent non trovato!");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody non trovato!");
        }

        if (animator == null)
        {
            Debug.LogError("Animator non trovato!");
        }

        GeneratePatrolPoints();

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            patrolCoroutine = StartCoroutine(PatrolCoroutine());
        }
        else
        {
            Debug.LogError("Punti di pattuglia non validi o non generati.");
        }
    }

    void OnEnable()
    {
        if (patrolCoroutine == null && patrolPoints != null && patrolPoints.Length > 0)
        {
            patrolCoroutine = StartCoroutine(PatrolCoroutine());
        }
    }

    void OnDisable()
    {
        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;
        }
    }

    void GeneratePatrolPoints()
    {
        patrolPoints = new Vector3[numPatrolPoints];
        for (int i = 0; i < numPatrolPoints; i++)
        {
            Vector3 randomPoint = Random.insideUnitSphere * patrolRadius;
            randomPoint.y = 0;
            if (NavMesh.SamplePosition(transform.position + randomPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            {
                patrolPoints[i] = hit.position;
            }
            else
            {
                Debug.LogWarning("Punto di pattuglia non valido.");
            }
        }
    }

    IEnumerator PatrolCoroutine()
    {
        while (true)
        {
            bool isGrounded = IsCloseToGround();
            animator.SetBool(IsGrounded, isGrounded);

            if (isGrounded)
            {
                if (!agent.enabled)
                {
                    agent.enabled = true;
                }

                if (!rb.isKinematic)
                {
                    rb.isKinematic = true;
                }

                if (agent.remainingDistance < 0.1f && patrolPoints.Length > 0)
                {
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                    agent.SetDestination(patrolPoints[currentPatrolIndex]);
                }

                // Rotazione più veloce
                Vector3 targetDirection = (agent.steeringTarget - transform.position).normalized;
                if (targetDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
            else
            {
                if (agent.enabled)
                {
                    agent.enabled = false;
                }

                if (rb.isKinematic)
                {
                    rb.isKinematic = false;
                }
            }

            yield return null;
        }
    }

    bool IsCloseToGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, airThreshold, groundLayer);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * airThreshold);
    }
}