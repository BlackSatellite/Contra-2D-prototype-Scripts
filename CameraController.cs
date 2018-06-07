using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public BoxCollider2D cameraBounds;

    public bool isFollowing;

    public GameObject leftBorder;

    private Transform player;

    private Vector2 min;
    private Vector2 max;

	// Use this for initialization
	void Start () 
    {
        player = FindObjectOfType<NewPlayerController>().transform;
	}
	
	// Update is called once per frame
	void Update () 
    {
        min = cameraBounds.bounds.min;
        max = cameraBounds.bounds.max;

        var x = transform.position.x;

        if (isFollowing)
        {
            if (player.position.x > x)
            {
                x = player.position.x;
            }
        }

        var cameraHalfWidth = GetComponent<Camera>().orthographicSize * ((float)Screen.width / Screen.height);
        x = Mathf.Clamp(x, min.x + cameraHalfWidth, max.x - cameraHalfWidth);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
        leftBorder.transform.position = new Vector2(x-cameraHalfWidth, transform.position.y);
	}
}
