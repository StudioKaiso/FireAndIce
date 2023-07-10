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
    public Transform cameraTarget;
    public CamTarget cameraTargetComponent;

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void Start() {
        //Assign Components
        if (GameObject.FindGameObjectWithTag("CameraTarget") != null) {
            cameraTarget = GameObject.FindGameObjectWithTag("CameraTarget").transform;
            cameraTargetComponent = cameraTarget.GetComponent<CamTarget>();
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

        //Input Events
        input.FindAction("Jump").started += ctx => Jump(jumpSpeed);
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
                    speed = new Vector3(
                        Mathf.Clamp(speed.x, -maxSpeed, maxSpeed), speed.y, 
                        Mathf.Clamp(speed.z, -maxSpeed, maxSpeed)
                    ) + Quaternion.LookRotation(cameraTarget.forward) * new Vector3(
                        input.FindAction("Move").ReadValue<Vector2>().x * (moveSpeed * 20.0f) * Time.deltaTime, 0.0f,
                        input.FindAction("Move").ReadValue<Vector2>().y * (moveSpeed * 20.0f) * Time.deltaTime
                    );
                }

                //Follow the player only when falling
                if (speed.y > 0) { cameraTargetComponent.followHeight = false; }
                else { cameraTargetComponent.followHeight = true; }
            break;
        }
    }
}
