using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.AI.Navigation;

public class NavMeshGenerator : MonoBehaviour
{
    [SerializeField] private float generationDelay = 5f;
    public NavMeshSurface surface;
    
    void Start()
    {
        StartCoroutine(GenerateNavMeshDelayed());
    }

    IEnumerator GenerateNavMeshDelayed()
    {
        yield return new WaitForSeconds(generationDelay);
        if (surface != null)
        {
            surface.BuildNavMesh();
            Debug.Log("NavMesh generato con successo dopo " + generationDelay + " secondi.");
        }
        else
        {
            Debug.LogError("NavMeshSurface non trovato. Assicurati di aggiungere il componente NavMeshSurface all'oggetto.");
        }
    }
}