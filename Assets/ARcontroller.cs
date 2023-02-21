using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARcontroller : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject myObject;
    public ARRaycastManager raycastManager;
    private bool _spawned;

    void Start()
    {
        _spawned = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount <= 0 || Input.GetTouch(0).phase != TouchPhase.Began)
            return;

        List<ARRaycastHit> touches = new List<ARRaycastHit>();
        raycastManager.Raycast(Input.GetTouch(0).position, touches,
            UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        if (touches.Count <= 0)
            return;
        
        if(!_spawned)
        {
            Instantiate(myObject, touches[0].pose.position, touches[0].pose.rotation);
            _spawned = true;
        }
        else
        {
            
        }
    }
}