using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShackerOne : MonoBehaviour {
    [Range(0f, 2f)] 
    public float intensity;

    Transform target;
    Vector3 initialPosition;
    
    float pendingShakeDuration = 0f;
    
    bool isShaking = false;

	// Use this for initialization
	void Start () {
        target = GetComponent<Transform>();
        initialPosition = target.localPosition;
	}
	
	// Update is called once per frame
/*	void Update () {
        if (pendingShakeDuration > 0 && !isShaking) 
        {
            StartCoroutine(DoShake());
        }
	}
*/
    public void Shake(float duration) 
    {
      
        if (duration > 0) 
        {
            pendingShakeDuration += duration;
            StartCoroutine(DoShake());
        }
    }

    IEnumerator DoShake()
    {
        isShaking = true;

        var startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < startTime + pendingShakeDuration)
        {
            var randomPoint = new Vector3(Random.Range(-1f, 1f) * intensity, Random.Range(-1f, 1f) * intensity, initialPosition.z);
            target.localPosition = randomPoint;
            yield return null;
        }

        pendingShakeDuration = 0f;
        target.localPosition = initialPosition;
        isShaking = false;
    }
}
