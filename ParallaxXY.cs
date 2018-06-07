using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxXY : MonoBehaviour {
    
    public bool EnableParallaxY = true;
    public float smoothing;
    public Transform[] backgrounds;
    
    private float[] parallaxScales;
    
    private Transform cam;
    private Vector3 previousCamPos;
    private Vector3 backgroundTargetPos;

	// Use this for initialization
	void Start () {

        cam = Camera.main.transform;

        previousCamPos = cam.position;

        parallaxScales = new float[backgrounds.Length];

        for (int i = 0; i < backgrounds.Length; i++) 
        {
            parallaxScales[i] = backgrounds[i].position.z * -1;
        }
	}
	// Update is called once per frame
	void LateUpdate () 
    {
        for (int i = 0; i < backgrounds.Length; i++) 
        {
            float parallaxX = (previousCamPos.x - cam.position.x) * parallaxScales[i];

            float backgroundTargetPosX = backgrounds[i].position.x + parallaxX;

            float parallaxY = (previousCamPos.y - cam.position.y) * parallaxScales[i];

            float backgroundTargetPosY = backgrounds[i].position.y + parallaxY;

            if (EnableParallaxY)
            {
                backgroundTargetPos = new Vector3(backgroundTargetPosX, backgroundTargetPosY, backgrounds[i].position.z); 
            }
            else
            {
                backgroundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z); 
            }

                backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
        }

        previousCamPos = cam.position;
	}
}
