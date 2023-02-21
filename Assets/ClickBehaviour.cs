using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ClickBehaviour : MonoBehaviour
{
    private float speed = 2.0f;
    private Vector3 targetPosition;
    private Animator animator;
    private ARSessionOrigin arSessionOrigin;
    private ARRaycastManager arRaycastManager;
    private Camera arCamera;
    
    private bool _isMoving = false;
    
    public GameObject foodPrefab;

    void Start()
    {
        animator = GetComponent<Animator>();
        arSessionOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        arCamera = arSessionOrigin.GetComponent<Camera>();
    }
    
    private void HandleTouch(Vector3 touchPosition)
    {
        Debug.Log("Handling touch");
        
        if(arCamera == null)
            return;
        
        Debug.Log("Camera found");
        
        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;
        
        Debug.Log("Raycast using camera");

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        {
            Debug.Log("Raycast hit dragon");
            // Instantiate the cube at a position in front of the dragon
            Vector3 foodPosition = transform.position + transform.forward * 2f;
            GameObject food = Instantiate(foodPrefab, foodPosition, Quaternion.identity);

            // Get the cube's rigidbody component and throw it towards the dragon
            Rigidbody foodRigidbody = food.GetComponent<Rigidbody>();
            foodRigidbody.AddForce(transform.forward * 500f);
            
            Debug.Log("Food spawned");
            
            return;
        }
        
        Debug.Log("Raycast hit nothing, trying to find location to move");

        // Perform a raycast from the touch position into the AR scene
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.Planes))
        {
            Debug.Log("Raycast hit plane");
            // If the ray hits a plane, set the target position to the hit point
            Pose hitPose = hits[0].pose;
            targetPosition = arSessionOrigin.transform.InverseTransformPoint(hitPose.position);
            animator.SetLookAtPosition(hitPose.position);
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
            animator.SetLookAtPosition(direction);
            transform.Translate(direction * speed * Time.deltaTime, Space.Self);
            
            _isMoving = true;
            
            Debug.Log("Moved");
        }
        else if(_isMoving)
        {
            Debug.Log("Idle");
            animator.SetTrigger("Idle"); 
            _isMoving = false;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered");
        
        if (other.CompareTag("Food"))
            animator.SetTrigger("Eat");
    }
}