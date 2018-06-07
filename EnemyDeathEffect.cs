using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathEffect : MonoBehaviour {

    public float height;
    private float scale;

	// Use this for initialization
	void Start () {
        scale = Mathf.Clamp((float)(FindObjectOfType<NewPlayerController>().transform.position.x * 10000 - transform.position.x * 10000), -transform.localScale.y, transform.localScale.z);
        transform.localScale = new Vector3(scale, transform.localScale.y, transform.localScale.z);
        if (GetComponent<Rigidbody2D>() != null) 
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(-scale * 0.5f , height);
        }
	}
}
