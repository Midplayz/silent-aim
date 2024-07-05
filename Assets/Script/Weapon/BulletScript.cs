using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float impactForce = 10f; // You can adjust this value
    public float bulletSpeed = 100f; // Ensure this matches your shooting script
    private Vector3 lastPosition;
    public LayerMask hitMask; // Layer mask to ignore the bullet itself

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        // Calculate the movement for this frame
        Vector3 currentPosition = transform.position;
        Vector3 direction = currentPosition - lastPosition;
        float distance = direction.magnitude;

        if (distance > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(lastPosition, direction, out hit, distance, hitMask))
            {
                Debug.Log("Hit: " + hit.collider.name);
                if (hit.collider.tag == "Zombie")
                {
                    Vector3 directionToEnemy = hit.collider.transform.position - transform.position;
                    Vector3 force = -directionToEnemy.normalized * impactForce;

                    hit.collider.gameObject.GetComponent<Enemy>().OnKilled(force);
                }
                Destroy(gameObject); // Destroy bullet on hit
            }
        }

        lastPosition = currentPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit: " + other.name);
        if (other.tag == "Zombie")
        {
            Vector3 directionToEnemy = other.transform.position - transform.position;
            Vector3 force = -directionToEnemy.normalized * impactForce;

            other.gameObject.GetComponent<Enemy>().OnKilled(force);
            Destroy(gameObject); // Destroy bullet on hit
        }
    }
}