using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ClickBehaviour : MonoBehaviour
{
    private float speed = 0.5f;
    private Vector3 targetPosition;
    private Animator animator;
    private ARSessionOrigin arSessionOrigin;
    private ARRaycastManager arRaycastManager;
    private bool _isMoving = false;
    
    public GameObject foodPrefab;

    void Start()
    {
        animator = GetComponent<Animator>();
        arSessionOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
    }
    
    private void HandleTouch(Vector3 touchPosition)
    {
        Debug.Log("Trying to find location to move");

        // Perform a raycast from the touch position into the AR scene
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.Planes))
        {
            Debug.Log("Raycast hit plane");
            // If the ray hits a plane, set the target position to the hit point
            Pose hitPose = hits[0].pose;
            targetPosition = arSessionOrigin.transform.InverseTransformPoint(hitPose.position);
            
            if((transform.position - targetPosition).magnitude < 0.4f)
                this.OnMouseDown();
            else
                _isMoving = true;
            
            Debug.Log("Target position set");
        }
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            this.HandleTouch(Input.GetTouch(0).position);

        if ((transform.position - targetPosition).magnitude > 0.1f && _isMoving)
        {
            Debug.Log("Moving");
            
            Vector3 direction = (targetPosition - transform.position).normalized;
            
            animator.SetTrigger("Walk");

            transform.Translate(direction * speed * Time.deltaTime, Space.Self);

            _isMoving = true;
            
            Debug.Log("Moved");
        }
        else if(_isMoving)
        {
            Debug.Log("Idle");
            animator.SetTrigger("Idle"); 
            transform.position = targetPosition;
            _isMoving = false;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered");
        
        animator.StopPlayback();
        animator.Play("Eat");
        Destroy(other.gameObject);
        
        Debug.Log("Trigger exited");
    }
    
    void OnMouseDown()
    {
        Debug.Log("Hungary dragon clicked");
        // Get the camera position and direction
        Vector3 cameraPosition = arSessionOrigin.transform.position;
        Vector3 cameraForward = arSessionOrigin.transform.forward;

        // Calculate the position to throw the food from (slightly in front of the camera)
        Vector3 throwPosition = cameraPosition + cameraForward * 0.5f;
        
        Debug.Log("Creating food");

        // Instantiate the food at the throw position
        GameObject food = Instantiate(foodPrefab, throwPosition, Quaternion.identity);

        // Get the food's rigidbody component
        Rigidbody foodRigidbody = food.GetComponent<Rigidbody>();

        // Calculate the direction to throw the food (towards the dragon)
        Vector3 throwDirection = (transform.position - throwPosition).normalized;

        // Calculate the velocity to throw the food (based on the desired throw distance and time)
        float throwDistance = 2.0f;
        float throwTime = 1.0f;
        Vector3 throwVelocity = throwDirection * (throwDistance / throwTime);
        
        Debug.Log("Throwing food");

        // Apply the velocity to the food's rigidbody
        foodRigidbody.velocity = throwVelocity;

        // Rotate the food to face the direction it is thrown
        food.transform.forward = throwDirection;
        
        Debug.Log("Food thrown");
    }

}