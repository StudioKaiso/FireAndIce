using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTarget : MonoBehaviour {
    //Initialize Variables
    public bool followHeight;
    private float previousHeight, heightOffset;
    private float refVelocity;

    //Initialize Components
    [SerializeField] private Camera target;
    private Transform parent;

    private void Start() {
        //Assign followHeight
        followHeight = true;

        //Assign parent and offset
        if (transform.parent != null) {
            heightOffset = transform.localPosition.y;
            parent = transform.parent;
            transform.SetParent(null);
        }
    }

    private void FixedUpdate() {
        if (target != null) {
            transform.rotation = Quaternion.Euler(
                new Vector3 (0.0f,target.transform.rotation.eulerAngles.y, 0.0f)  
            );
        }

        if (parent != null) {
            if (followHeight) { previousHeight = parent.position.y + heightOffset; }
            
            transform.position = new Vector3 (
                parent.position.x, 
                Mathf.SmoothDamp(transform.position.y, previousHeight, ref refVelocity, .35f), 
                parent.position.z
            );  
        }
        
    }
}
