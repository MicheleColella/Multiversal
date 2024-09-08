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
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        GeneratePatrolPoints();
        StartCoroutine(PatrolCoroutine());
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
                    rb.isKinematic = true; // Disabilita la fisica
                    agent.SetDestination(patrolPoints[currentPatrolIndex]);
                }

                if (agent.remainingDistance < 0.1f)
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
                    rb.isKinematic = false; // Abilita la fisica per farlo cadere
                }
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
