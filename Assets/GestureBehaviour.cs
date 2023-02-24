using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureBehaviour : MonoBehaviour
{
    public float flickThreshold = 1f;
    private Animator animator;
    private Vector3 lastAcceleration;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        Vector3 acceleration = Input.acceleration;

        // Calculate the difference in acceleration since the last frame
        Vector3 deltaAcceleration = acceleration - lastAcceleration;

        var x = Mathf.Abs(deltaAcceleration.x);
        var y = Mathf.Abs(deltaAcceleration.y);
        var z = Mathf.Abs(deltaAcceleration.z);
        
        Debug.Log("x: " + x + " y: " + y + " z: " + z);

        // Check if the difference in acceleration exceeds the flick threshold in any axis
        if (Mathf.Abs(deltaAcceleration.x) > flickThreshold || 
            Mathf.Abs(deltaAcceleration.y) > flickThreshold || 
            Mathf.Abs(deltaAcceleration.z) > flickThreshold)
        {
            animator.SetTrigger("Fly");
        }

        lastAcceleration = acceleration;
    }
}
