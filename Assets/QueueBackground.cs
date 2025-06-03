using System;
using System.Collections;
using UnityEngine;

public class QueueBackground : MonoBehaviour
{
    public Transform spawnPoint;
    
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("box"))
        {
            other.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            other.gameObject.transform.position = spawnPoint.position;
            other.GetComponent<Rigidbody2D>().AddForce(Vector2.right, ForceMode2D.Impulse);
        }
    }
}
