using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float impactForce = 10f; 
    public float bulletSpeed = 100f; 
    private Vector3 lastPosition;
    public LayerMask hitMask; 

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
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
                Destroy(gameObject);
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
            Destroy(gameObject);
        }
    }
}