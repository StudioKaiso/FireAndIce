using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTarget : MonoBehaviour {
    //Initialize Components
    private Player player;

    private void Start() {
        //Assign Components
        player = GetComponentInParent<Player>();
    }

    private void FixedUpdate() {
        
    }
}
