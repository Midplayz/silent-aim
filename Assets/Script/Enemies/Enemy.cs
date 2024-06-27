using UnityEngine;

public class Enemy : MonoBehaviour
{
    public PathDefiner path;
    public float speed = 5f; //WalkAnimation zombie is 0.25
    public float offsetRange = 10f;
    public EnemySpawner spawner;
    public Transform player; 
    public float stopDistance = 3f; 

    private int currentWaypointIndex = 0;
    private Vector3 targetPosition;
    private bool targetingPlayer = false;

    void Start()
    {
        if (path != null && path.waypoints.Length > 0)
        {
            SetNextTarget();
        }
    }

    void Update()
    {
        if (targetingPlayer)
        {
            MoveTowardsPlayer();
        }
        else
        {
            MoveAlongPath();
        }
    }

    void MoveAlongPath()
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
            targetingPlayer = true; // Start targeting the player
        }
    }

    void MoveTowardsPlayer()
    {
        if (player == null)
            return;

        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Ignore vertical difference

        if (directionToPlayer.magnitude > stopDistance)
        {
            targetPosition = player.position - directionToPlayer.normalized * stopDistance;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }

    void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.DecreaseEnemyCount();
        }
    }
}
