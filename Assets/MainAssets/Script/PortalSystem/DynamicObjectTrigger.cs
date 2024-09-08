using System.Collections.Generic;
using UnityEngine;

public class DynamicObjectTrigger : MonoBehaviour
{
    private NonEuclideanZone zone;

    public void Initialize(NonEuclideanZone nonEuclideanZone)
    {
        zone = nonEuclideanZone;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (zone.IsZoneActive())
        {
            zone.AddDynamicObject(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        zone.RemoveDynamicObject(other.gameObject);
    }
}