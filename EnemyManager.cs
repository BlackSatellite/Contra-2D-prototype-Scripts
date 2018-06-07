using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    // Handle camera shaking
    public CameraShake camShake;
    public float camShakeAmount = 0.1f;
    public float camShakeLength = 0.1f;

    public int health;
    private int currentHealth;

    public GameObject deathEffect;

	// Use this for initialization
	void Start () {
        currentHealth = health;
	}

    // Take damage
    public void TakeDamage() 
    {
        health--;
        if (health <= 0)
        {
            Die();
        }
    }

    // Die
    public void Die() 
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
        }
        Destroy(gameObject);

        camShake.Shake(camShakeAmount, camShakeLength);
        
    }
}
