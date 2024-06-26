using UnityEngine;
using System.Collections;
using System.IO;

public class Enemy : MonoBehaviour
{
    public PathDefiner path;
    public float speed = 5f;
    public float offsetRange = 10f;

    private int currentWaypointIndex = 0;
    private Vector3 targetPosition;

    void Start()
    {
        if (path != null && path.waypoints.Length > 0)
        {
            SetNextTarget();
        }
    }

    void Update()
    {
        if (path == null || path.waypoints.Length == 0)
            return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNextTarget();
        }
    }

    void SetNextTarget()
    {
        if (currentWaypointIndex < path.waypoints.Length)
        {
            Vector3 waypointPosition = path.waypoints[currentWaypointIndex].position;

            float offsetX = Random.Range(-offsetRange, offsetRange);
            float offsetZ = Random.Range(-offsetRange, offsetRange);
            targetPosition = waypointPosition + new Vector3(offsetX, 0, offsetZ);

            currentWaypointIndex++;
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("REACHED END");
        }
    }
}
