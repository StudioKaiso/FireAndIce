using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character {
    //Input System
    [Header("- Player Inputs -")]
    public InputActionMap input;

    //Initialize Components
    [Header("- Player Components -")]
    [SerializeField] private Transform cameraTarget;

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void Start() {
        //Assign Components
        if (GameObject.FindGameObjectWithTag("CameraTarget") != null) {
            cameraTarget = GameObject.FindGameObjectWithTag("CameraTarget").transform;
        } else { cameraTarget = Camera.main.transform; }

        //Assign inputs
        if (input.actions.Count == 0) { input = GameController.instance.input; }
        input.Enable();

        //Allow movement
        canMove = true;

        //Assign components
        rb = GetComponent<Rigidbody>();

        //Hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected override void FixedUpdate() { base.FixedUpdate();
        //Specific player behaviour
        switch (state) {
            case State.onGround:
                //Allow the player to move around
                if (canMove) {
                    speed = Move (speed, cameraTarget, 
                        input.FindAction("Move").ReadValue<Vector2>(), 
                        moveSpeed, maxSpeed
                    );    
                }
                
            break;

            case State.inAir:
                //Allow the player to move around in the air
                if (canMove) {
                    speed = Move (speed, cameraTarget, 
                        input.FindAction("Move").ReadValue<Vector2>(), 
                        moveSpeed / 2.0f, maxSpeed
                    );    
                }
            break;
        }
    }
}
