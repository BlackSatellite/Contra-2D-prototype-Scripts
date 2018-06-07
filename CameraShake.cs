using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    [Range(0f, 2f)]
    public float intensity;

    Transform target;
    Vector3 initialPosition;

    public Camera mainCam;

    float shakeAmount = 0;

    void Awake() 
    {
        if (mainCam == null)
            mainCam = Camera.main;
    }

    void Start()
    {
        target = GetComponent<Transform>();
        initialPosition = target.localPosition;
    }

    public void Shake(float amt, float length) 
    {
        shakeAmount = amt;
        InvokeRepeating("DoShake", 0, 0.01f);
        Invoke("StopShake", length);
    }

    void DoShake() 
    {
        if (shakeAmount > 0) 
        {
            /*
            Vector3 camPos = mainCam.transform.position;

            float offsetX = Random.value * shakeAmount * 2 - shakeAmount;
            float offsetY = Random.value * shakeAmount * 2 - shakeAmount;
            camPos.x += offsetX;
            camPos.y += offsetY;
            */
            var camPos = new Vector3(Random.Range(-1f, 1f) * intensity, Random.Range(-1f, 1f) * intensity, initialPosition.z);

            target.localPosition = camPos;
        }
    }

    void StopShake() 
    {
        CancelInvoke("DoShake");
        mainCam.transform.localPosition = Vector3.zero;
    }
}
