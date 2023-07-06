using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour {
    //Initialize Singleton
    public static GameController instance;

    //Initialize Input
    public InputActionMap input;

    private void Awake() {
        //Assign Singleton
        if (GameObject.FindGameObjectsWithTag("GameController").Length > 1) { 
            Destroy(this.gameObject); 
        } else { instance = this; }
    }

    private void Update() {
        
    }
}
