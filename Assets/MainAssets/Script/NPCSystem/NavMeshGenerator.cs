using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.AI.Navigation;

public class NavMeshGenerator : MonoBehaviour
{
    [SerializeField] private float generationDelay = 5f;
    [SerializeField] private float updateInterval = 0.5f; // intervallo di aggiornamento costante
    public NavMeshSurface surface;

    void Start()
    {
        StartCoroutine(GenerateNavMeshDelayed());
        StartCoroutine(UpdateNavMeshContinuously());
    }

    IEnumerator GenerateNavMeshDelayed()
    {
        yield return new WaitForSeconds(generationDelay);
        BuildNavMesh();
    }

    IEnumerator UpdateNavMeshContinuously()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);
            if (surface != null)
            {
                surface.UpdateNavMesh(surface.navMeshData);
                Debug.Log("NavMesh aggiornato.");
            }
            else
            {
                Debug.LogError("NavMeshSurface non trovato.");
            }
        }
    }

    private void BuildNavMesh()
    {
        if (surface != null)
        {
            surface.BuildNavMesh();
            Debug.Log("NavMesh generato con successo.");
        }
        else
        {
            Debug.LogError("NavMeshSurface non trovato.");
        }
    }
}
