using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCrazyRunner : MonoBehaviour {

    private bool onGround;
    public Transform groundSensor;

    private bool cliffAhead;
    public Transform cliffSensor;

    private bool hittingWall;
    public Transform wallSensor;

    public LayerMask whatIsGround;
    public LayerMask whatIsWall;

    private Rigidbody2D myBody;
    private Animator myAnimator;

    public float moveSpeed;
    public float jumpHeight;

    private bool reacted;

	// Use this for initialization
	void Start () {
        myBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        onGround = Physics2D.OverlapCircle(groundSensor.position, 0.1f, whatIsGround);    // Is enemy on the ground
        cliffAhead = !Physics2D.OverlapCircle(cliffSensor.position, 0.1f, whatIsGround);  // Is there a cliff ahead
        hittingWall = Physics2D.OverlapCircle(wallSensor.position, 0.1f, whatIsWall); // Is there a wall ahead

        // React to cliff
        if (onGround && cliffAhead && !reacted) 
        {
            ReactToCliff(Random.Range(0, 3));
        }
        if (onGround && !cliffAhead && reacted)
        {
            reacted = false;
        }
        // Change direction after hitting the wall
        if (hittingWall) transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        
        // Moving
        myBody.velocity = new Vector2(moveSpeed * transform.localScale.x, myBody.velocity.y);

        // Каждый кадр вызывать в Update - Плохой тон
        myAnimator.SetBool("OnGround", onGround);
	}

    // React to cliff
    void ReactToCliff(float r) 
    {
        if (r == 0) // Jump
        {
            myBody.velocity = new Vector2(myBody.velocity.x, jumpHeight);
        }
        if (r == 1) // Fall
        {
            myBody.velocity = new Vector2(myBody.velocity.x, jumpHeight/2);
        }
        if (r > 1) // Turn around 
        {
            myBody.velocity = new Vector2(0, myBody.velocity.y);
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        reacted = true;
    }
}
