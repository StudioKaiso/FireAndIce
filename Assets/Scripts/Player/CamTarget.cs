using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTarget : MonoBehaviour {
    //Initialize Components
    [SerializeField] private Camera target;

    private void FixedUpdate() {
        if (target != null) {
            transform.rotation = Quaternion.Euler(
                new Vector3 (0.0f,target.transform.rotation.eulerAngles.y, 0.0f)  
            );    
        }
        
    }
}
