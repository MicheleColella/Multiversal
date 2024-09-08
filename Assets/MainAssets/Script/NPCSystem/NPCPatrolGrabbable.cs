using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCPatrol : MonoBehaviour
{
    private NavMeshAgent agent;
    private Rigidbody rb;
    private Vector3[] patrolPoints;
    private int currentPatrolIndex = 0;

    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private int numPatrolPoints = 5;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private float airThreshold = 1.0f; // Altezza massima per considerare l'NPC in aria
    [SerializeField] private LayerMask groundLayer; // Per determinare il layer del terreno
    [SerializeField] private GameObject objectToToggle; // GameObject da attivare/disattivare quando tocca il terreno
    [SerializeField] private GameObject anotherObjectToToggle; // Il nuovo oggetto da attivare/disattivare

    private Coroutine patrolCoroutine; // Memorizza la coroutine

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        // Controlla che i componenti siano assegnati correttamente
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent non trovato!");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody non trovato!");
        }

        GeneratePatrolPoints();

        // Assicurati che i punti di pattuglia siano stati generati prima di iniziare la coroutine
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            patrolCoroutine = StartCoroutine(PatrolCoroutine());
        }
        else
        {
            Debug.LogError("Punti di pattuglia non validi o non generati.");
        }
    }

    // Riavvia la coroutine quando il GameObject viene riattivato
    void OnEnable()
    {
        if (patrolCoroutine == null && patrolPoints != null && patrolPoints.Length > 0)
        {
            patrolCoroutine = StartCoroutine(PatrolCoroutine());
        }
    }

    // Ferma la coroutine quando il GameObject viene disabilitato
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
        if (IsCloseToGround())
        {
            if (!agent.enabled)
            {
                agent.enabled = true;
            }

            if (!rb.isKinematic)
            {
                rb.isKinematic = true; // Abilita isKinematic solo quando l'NPC tocca il pavimento
            }

            if (agent.remainingDistance < 0.1f && patrolPoints.Length > 0)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentPatrolIndex]);
            }
        }
        else
        {
            // Se è "in aria", disabilita il NavMeshAgent e attiva la fisica
            if (agent.enabled)
            {
                agent.enabled = false;
            }

            if (rb.isKinematic)
            {
                rb.isKinematic = false; // Disabilita isKinematic quando l'NPC è in aria
            }
        }

        // Attiva o disattiva il nuovo GameObject in base a isKinematic
        if (anotherObjectToToggle != null)
        {
            anotherObjectToToggle.SetActive(rb.isKinematic);
        }

        yield return null;
    }
}


    // Controlla se l'NPC è abbastanza vicino al suolo tramite un Raycast
    bool IsCloseToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, airThreshold, groundLayer))
        {
            return true; // Se il Raycast colpisce qualcosa sotto una certa distanza, consideriamo l'NPC "a terra"
        }
        else
        {
            return false; // Altrimenti è "in aria"
        }
    }

    // Disegna il gizmo per visualizzare l'airThreshold
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Disegna una linea che rappresenta l'airThreshold
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * airThreshold);
    }
}
