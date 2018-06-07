using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    Rigidbody2D myRigidbody;
    
    public float moveSpeed;

    public GameObject projectileEffect;

	// Use this for initialization
	void Start () {
        myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.AddRelativeForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
	}

    void OnBecameInvisible() 
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Enemy") 
        {
            if (other.GetComponent<EnemyManager>() != null) 
            {
                other.GetComponent<EnemyManager>().TakeDamage(); // Make damage by bullet to the enemy
            }

            Instantiate(projectileEffect, transform.position, transform.rotation);
            
            Destroy(gameObject); // Destroy bullet
        }
    }
}
