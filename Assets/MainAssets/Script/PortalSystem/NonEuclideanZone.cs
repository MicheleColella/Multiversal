using System.Collections.Generic;
using UnityEngine;

public class NonEuclideanZone : MonoBehaviour
{
    [Header("Player and Zone Settings")]
    public Transform player;
    public Transform actionOrigin;
    public float angleThreshold = 45f;
    public float radius = 10f;

    [Header("Gizmo Settings")]
    public Color gizmoColor = Color.green;
    public int arcResolution = 50;

    [Header("Static Objects to Toggle")]
    public List<GameObject> staticObjectsToToggle;

    [Header("Dynamic Objects")]
    private List<GameObject> dynamicObjectsToToggle = new List<GameObject>();

    [Header("Trigger Settings")]
    public Collider dynamicObjectTrigger;

    private bool isZoneActive = false;

    private void Start()
    {
        if (dynamicObjectTrigger == null)
        {
            Debug.LogError("Dynamic Object Trigger is not assigned!");
            return;
        }

        dynamicObjectTrigger.gameObject.AddComponent<DynamicObjectTrigger>().Initialize(this);
    }

    private void Update()
    {
        if (actionOrigin == null || player == null) return;

        Vector3 directionToPlayer = player.position - actionOrigin.position;
        directionToPlayer.y = 0;
        float distanceToPlayer = directionToPlayer.magnitude;

        bool shouldActivate = false;

        if (distanceToPlayer <= radius)
        {
            float angle = Vector3.Angle(actionOrigin.forward, directionToPlayer);
            shouldActivate = (angle <= angleThreshold);
        }

        // Aggiorna lo stato della zona
        isZoneActive = shouldActivate;

        SetObjectsActive(staticObjectsToToggle, shouldActivate);
        SetObjectsActive(dynamicObjectsToToggle, shouldActivate);
    }

    private void SetObjectsActive(List<GameObject> objectsList, bool isActive)
    {
        foreach (GameObject obj in objectsList)
        {
            if (obj != null)
                obj.SetActive(isActive);
        }
    }

    public void AddDynamicObject(GameObject obj)
    {
        if (isZoneActive && !staticObjectsToToggle.Contains(obj) && !dynamicObjectsToToggle.Contains(obj) && obj != player.gameObject)
        {
            dynamicObjectsToToggle.Add(obj);
            obj.SetActive(true);
        }
    }

    public void RemoveDynamicObject(GameObject obj)
    {
        if (dynamicObjectsToToggle.Remove(obj))
        {
            obj.SetActive(true); // Riattiva l'oggetto quando esce dalla zona
        }
    }

    public bool IsZoneActive()
    {
        return isZoneActive;
    }

    private void OnDrawGizmos()
    {
        if (actionOrigin == null) return;

        Gizmos.color = gizmoColor;

        Vector3 forward = actionOrigin.forward * radius;

        Quaternion leftRayRotation = Quaternion.AngleAxis(-angleThreshold, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(angleThreshold, Vector3.up);

        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        Gizmos.DrawLine(actionOrigin.position, actionOrigin.position + leftRayDirection);
        Gizmos.DrawLine(actionOrigin.position, actionOrigin.position + rightRayDirection);

        DrawArcManual(actionOrigin.position, angleThreshold, radius, arcResolution);
    }

    private void DrawArcManual(Vector3 center, float angleThreshold, float radius, int segments)
    {
        float angleStep = (angleThreshold * 2) / segments;
        float currentAngle = -angleThreshold;

        Vector3 lastPoint = GetPointOnCircle(center, radius, currentAngle);

        for (int i = 1; i <= segments; i++)
        {
            currentAngle += angleStep;
            Vector3 nextPoint = GetPointOnCircle(center, radius, currentAngle);
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }

    private Vector3 GetPointOnCircle(Vector3 center, float radius, float angle)
    {
        float radianAngle = Mathf.Deg2Rad * angle;
        return new Vector3(
            center.x + Mathf.Sin(radianAngle) * radius,
            center.y,
            center.z + Mathf.Cos(radianAngle) * radius
        );
    }
}