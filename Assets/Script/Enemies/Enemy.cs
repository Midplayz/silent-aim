using UnityEngine;

public class Enemy : MonoBehaviour
{
    public PathDefiner path;
    public float speed = 5f; // WalkAnimation zombie is 0.25
    public float offsetRange = 10f;
    public EnemySpawner spawner;
    public Transform player;
    public float stopDistance = 3f;
    public Animator animator;

    private int currentWaypointIndex = 0;
    private Vector3 targetPosition;
    private bool targetingPlayer = false;
    private bool isDead = false;
    private float initialYPosition;

    void Start()
    {
        if (path != null && path.waypoints.Length > 0)
        {
            SetNextTarget();
        }
        initialYPosition = transform.position.y;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

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

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // Ignore vertical difference for rotation
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        }

        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        newPosition.y = initialYPosition; // Keep the Y position constant
        transform.position = newPosition;

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
        directionToPlayer.y = 0; // Ignore vertical difference for direction

        if (directionToPlayer.magnitude > stopDistance)
        {
            targetPosition = player.position - directionToPlayer.normalized * stopDistance;

            Vector3 direction = targetPosition - transform.position;
            direction.y = 0; // Ignore vertical difference for rotation
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
            }

            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            newPosition.y = initialYPosition; // Keep the Y position constant
            transform.position = newPosition;
        }
    }

    public void OnKilled(Vector3 force)
    {
        if (isDead) return;
        Debug.Log("KILLED");
        isDead = true;

        // Disable the animator
        if (animator != null)
        {
            animator.enabled = false;
        }

        // Enable ragdoll effect by setting rigidbodies to non-kinematic
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
        }

        // Apply general backward force
        Vector3 backwardForce = transform.forward * -force.magnitude;
        backwardForce.y = 0; // Ensure the force is horizontal

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.AddForce(backwardForce, ForceMode.Impulse);
        }

        // Destroy the enemy object after 3 seconds
        Destroy(gameObject, 3f);
    }


    void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.DecreaseEnemyCount();
        }
    }
}