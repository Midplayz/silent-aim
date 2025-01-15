using UnityEngine;

public class Enemy : MonoBehaviour
{
    public PathDefiner path;
    public float speed = 5f;
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
        direction.y = 0; 
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        }

        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        newPosition.y = initialYPosition; 
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
            targetingPlayer = true; 
        }
    }

    void MoveTowardsPlayer()
    {
        if (player == null)
            return;

        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; 

        if (directionToPlayer.magnitude > stopDistance)
        {
            targetPosition = player.position - directionToPlayer.normalized * stopDistance;

            Vector3 direction = targetPosition - transform.position;
            direction.y = 0; 
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
            }

            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            newPosition.y = initialYPosition; 
            transform.position = newPosition;
        }
    }

    public void OnKilled(Vector3 force)
    {
        if (isDead) return;

        GameManager.Instance.UpdateScore(1);

        isDead = true;

        if (animator != null)
        {
            animator.enabled = false;
        }

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
        }

        Vector3 backwardForce = transform.forward * -force.magnitude;
        backwardForce.y = 0;

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.AddForce(backwardForce, ForceMode.Impulse);
        }

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