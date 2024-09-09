using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.AI.Navigation;

public class NavMeshGenerator : MonoBehaviour
{
    [SerializeField] private float generationDelay = 5f;
    [SerializeField] private float updateInterval = 2f; // intervallo tra gli aggiornamenti
    public NavMeshSurface surface;

    private Vector3 lastPosition;
    private Quaternion lastRotation;

    void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        StartCoroutine(GenerateNavMeshDelayed());
        StartCoroutine(UpdateNavMeshPeriodically());
    }

    IEnumerator GenerateNavMeshDelayed()
    {
        yield return new WaitForSeconds(generationDelay);
        BuildNavMesh();
    }

    IEnumerator UpdateNavMeshPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);
            if (HasTransformChanged())
            {
                BuildNavMesh();
            }
        }
    }

    private void BuildNavMesh()
    {
        if (surface != null)
        {
            surface.BuildNavMesh();
            Debug.Log("NavMesh aggiornato.");
        }
        else
        {
            Debug.LogError("NavMeshSurface non trovato.");
        }
    }

    private bool HasTransformChanged()
    {
        if (transform.position != lastPosition || transform.rotation != lastRotation)
        {
            lastPosition = transform.position;
            lastRotation = transform.rotation;
            return true;
        }
        return false;
    }
}
