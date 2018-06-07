using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerController : MonoBehaviour {

    public GameObject spawnPoint;
    public float respawnDelay;
    private bool isActive;
    private bool isDead = false;

    public GameObject fireLight;        
    private Light fireFlash;
    private float flashRate = 0.1f;
    private float nextFlash;
    
    public Transform muzzleFlashPrefab;

    public GameObject basicProjectile;
    private GameObject currentProjectile;
    
    private BoxCollider2D myColl;

    public float moveSpeed;
    public float jumpHeight;
//  public float gravity;
    public float shootDelay;

    public float[] shootAngles;
    private Quaternion rotation;
    
    public Transform[] shootPoints;
    private Transform currentShootingPoint;

    public float groundCheckRadius;
    public Transform groundCheckLeft;
    public Transform groundCheckRight;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlatform;

    public int direction;

    private float horisontalSpeed;
  //private float verticalSpeed;
    private float shootDelayCounter;

    private float originColliderSize;
    private float originColliderOffset;
    public float duckColliderSize;
    public float duckColliderOffset;

    private Rigidbody2D myRigidBody2D;
   
    private int playerLayer;
    private int platformLayer;

    private bool onGround = false;
    
    private bool jumpOffCoroutineIsRunning = false;
    private bool facingRight;
    private bool jumped;
    private bool moving;
    
    private bool keyLeft;
    private bool keyRight;
    private bool keyUp;
    private bool keyDown;
    private bool keyJump;
    private bool keyAction;

    private Vector2 botLeft; //Coordinates of an angles
    private Vector2 botRight;
    private Vector2 topLeft;
    private Vector2 topRight;
    private Animator[] animators;
/*
    private bool isActive;
    private bool isDead;
    public float invencibilityTime;
    public float incactivityTime;
    private float invincCounter;
    private float inactCounter;
    public GameObject SpawnPoint;
*/    
	// Use this for initialization
	void Start() 
    {
        nextFlash = Time.time + flashRate;
        fireFlash = fireLight.GetComponent<Light>(); //Light
        fireFlash.intensity = 0f;

        shootDelayCounter = 0;
        currentProjectile = basicProjectile;
        rotation = new Quaternion(0,0,0,0);
        myColl = GetComponent<BoxCollider2D>();
        originColliderSize = myColl.size.y;
        originColliderOffset = myColl.offset.y;

        myRigidBody2D = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
        playerLayer = LayerMask.NameToLayer("Player");
        platformLayer = LayerMask.NameToLayer("Platform");
	}

	// Update is called once per frame
	void Update()
    {
        if (isDead) 
        {
            RespawnPlayer();
        }

        onGround = Physics2D.OverlapCircle(groundCheckLeft.position, groundCheckRadius, whatIsGround) 
                || Physics2D.OverlapCircle(groundCheckLeft.position, groundCheckRadius, whatIsPlatform)
                || Physics2D.OverlapCircle(groundCheckRight.position, groundCheckRadius, whatIsGround)
                || Physics2D.OverlapCircle(groundCheckRight.position, groundCheckRadius, whatIsPlatform);
        GetInput();
        ColliderSizeDuck();
        CalculateDirection();
        CalculateShootAngles();
        CalculateShootPoint();
        Animate();
        Move();
        JumpOn();
        Shoot();
	}

    //Key is pressed or not
    void GetInput()
    {
        keyLeft = Input.GetKey(KeyCode.LeftArrow);
        keyRight = Input.GetKey(KeyCode.RightArrow);
        keyUp = Input.GetKey(KeyCode.UpArrow);
        keyDown = Input.GetKey(KeyCode.DownArrow);
        keyJump = Input.GetKeyDown(KeyCode.Z);
        keyAction = Input.GetKey(KeyCode.X);
    }

    void Move()
    {
        if (keyJump && !keyDown && myRigidBody2D.velocity.y == 0) //(keyJump && onGround)
        {
            Jump(); 
            onGround = false;
            jumped = true;
        }

        if (myRigidBody2D.velocity.y == 0) jumped = false;
        //if (onGround) jumped = false;

        if (keyJump && keyDown)
        {
            onGround = false;
            StartCoroutine("JumpOff");
        }

        if (keyLeft) horisontalSpeed = -moveSpeed * Time.deltaTime;
        if (keyRight) horisontalSpeed = moveSpeed * Time.deltaTime;

        if (myRigidBody2D.velocity.x < 0 && horisontalSpeed < 0 && !facingRight) Flip();
        if (myRigidBody2D.velocity.x > 0 && horisontalSpeed > 0 && facingRight) Flip();

        if (keyLeft || keyRight) moving = true;
        if ((!keyLeft && !keyRight) || (keyLeft && keyRight))
        {
            moving = false;
            horisontalSpeed = 0;
        }
        
  //      if (!onGround) verticalSpeed -= gravity * Time.deltaTime;

        myRigidBody2D.velocity = new Vector2(horisontalSpeed, myRigidBody2D.velocity.y);
    }

    //Jumping , Jumping On Platform, Jumping Off Platform
    void Jump()
    {
        myRigidBody2D.velocity = new Vector2(myRigidBody2D.velocity.x, jumpHeight);
    }

    void JumpOn() 
    {
        if (myRigidBody2D.velocity.y > 0)
            Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, true);

        else if (myRigidBody2D.velocity.y <= 0 && !jumpOffCoroutineIsRunning)
            Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, false);	
    }

    IEnumerator JumpOff()
    {
        jumpOffCoroutineIsRunning = true;
        Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, true);
        yield return new WaitForSeconds(0.2f);
        Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, false);
        jumpOffCoroutineIsRunning = false;
    }

    //Shooting
    void Shoot() 
    {
        if (keyAction && shootDelayCounter <= 0)
        {
            if((currentProjectile == basicProjectile) && FindObjectsOfType<Projectile>().Length < 100)
            {
                Instantiate(currentProjectile, currentShootingPoint.position, rotation);
                shootDelayCounter = shootDelay;
            }
        }
        
        // Start/stop flickering from shooting
        if (keyAction)
        {
            ShootFlickering();
            MuzzleFlash();
        }
        if (!keyAction) fireFlash.intensity = 0f;

        shootDelayCounter -= Time.deltaTime;
    }

    //Flickering from shooting
    void ShootFlickering() 
    {
        if (Time.time >= nextFlash) 
        {
            nextFlash = Time.time + flashRate;
            fireFlash.intensity = 10f;
        }

        if (Time.time >= (nextFlash - 0.05f)) 
            fireFlash.intensity = 0f;
    }

    //Muzzle Flash
    void MuzzleFlash() 
    {
        Transform clone = Instantiate(muzzleFlashPrefab, currentShootingPoint.position, rotation) as Transform;
        clone.parent = currentShootingPoint;
        float size = Random.Range(0.5f, 2f);                // Randomize scale of muzzleFlash
        clone.localScale = new Vector3(size, size, size);
        Destroy(clone.gameObject, 0.02f);
    }

    //Calculate direction by clockwise on Numeric keypad/Numpad 8-9-6-3-2-1-4-7
    void CalculateDirection()
    {
        if (keyUp && !keyRight && !keyLeft && !keyDown)
        {
            direction = 8;
        }
        else if (jumped && keyDown && !keyRight && !keyLeft) direction = 2;
        else if (transform.localScale.x > 0)
        {
            if (keyUp && keyRight) direction = 9;
            else if (keyDown && keyLeft && keyRight) direction = 6;
            else if (keyDown && keyRight) direction = 3;
            else if (keyDown && !keyRight) direction = 6;
            else direction = 6;
        }
        else if (transform.localScale.x < 0) 
        {
            if (keyUp && keyLeft) direction = 7;
            else if (keyDown && keyLeft && keyRight) direction = 4;
            else if (keyDown && keyLeft) direction = 1;
            else if (keyDown && !keyLeft) direction = 4;
            else direction = 4;
        }
    }

    //Calculate point for shooting
    void CalculateShootPoint() 
    {
        if (onGround && direction == 8) currentShootingPoint = shootPoints[0];
        if (!jumped && (direction == 9 || direction == 7)) currentShootingPoint = shootPoints[1];
        if (!jumped && (direction == 4 || direction == 6)) currentShootingPoint = shootPoints[2];
        if (!jumped && (direction == 3 || direction == 1)) currentShootingPoint = shootPoints[3];
        if (!jumped && keyDown && direction != 3 && direction != 1) currentShootingPoint = shootPoints[4];
        if (!onGround && keyDown) currentShootingPoint = shootPoints[2];
        if (jumped) currentShootingPoint = transform;
        if (onGround && keyDown && keyRight && keyLeft) currentShootingPoint = shootPoints[4];
    }

    //Calculate shooting angles
    void CalculateShootAngles() 
    {
        if (direction == 8) rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, shootAngles[0]);
        if (direction == 9) rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, -shootAngles[1]);
        if (direction == 6) rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, -shootAngles[2]);
        if (direction == 3) rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, -shootAngles[3]);
        if (direction == 2) rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, shootAngles[4]);
        if (direction == 7) rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, shootAngles[1]);
        if (direction == 4) rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, shootAngles[2]);
        if (direction == 1) rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, shootAngles[3]);
    }
    
    //Flip the direction of the Player
    void Flip()
    {
        Vector3 startingScale = transform.localScale;
        facingRight = !facingRight;
        transform.localScale = new Vector3(-startingScale.x, startingScale.y, startingScale.z);
    }
     
    //Change BoxCollider size when ducked
    void ColliderSizeDuck() 
    {
        if (onGround && keyDown && direction != 3 && direction != 1)
        {
            myColl.size = new Vector2(myColl.size.x, duckColliderSize);
            myColl.offset = new Vector2(myColl.offset.x, duckColliderOffset);
        }
        else 
        {
            myColl.size = new Vector2(myColl.size.x, originColliderSize);
            myColl.offset = new Vector2(myColl.offset.x, originColliderOffset);
        }
    }

    //Player Respawn
    public void RespawnPlayer() 
    {
        transform.position = spawnPoint.transform.position;
        StartCoroutine("RespawnPlayerCo");
    }

    public IEnumerator RespawnPlayerCo() 
    {
        float gravityStore = myRigidBody2D.GetComponent<Rigidbody2D>().gravityScale;
        myRigidBody2D.gravityScale = 0f;
        isDead = true;
        //myRigidBody2D.GetComponent<Rigidbody2D>().gravityScale = 0f;
        yield return new WaitForSeconds(respawnDelay);
        myRigidBody2D.gravityScale = gravityStore;
        isDead = false;
    }
    
    //Animation
    void Animate()
    {
        for (int i = 0; i < animators.Length; i++)
        {
            animators[i].SetBool("OnGround", onGround);
            animators[i].SetBool("KeyDown", keyDown);
            animators[i].SetBool("Moving", moving);
            animators[i].SetBool("Shooting", keyAction);
            animators[i].SetBool("Jumped", jumped);
            animators[i].SetFloat("Velocity.y", myRigidBody2D.velocity.y);
            animators[i].SetInteger("Direction", direction);
        } 
    }
}
