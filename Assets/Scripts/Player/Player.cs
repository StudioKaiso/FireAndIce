using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    //Initialize Variables
    [Header("- Player Variables -")]
    public Vector3 speed;
    public float moveSpeed, maxSpeed, turnSpeed;
    [SerializeField] private float jumpSpeed;
    private int jumps;
    [SerializeField] private int maxJumps;
    [SerializeField] private float friction;

    //State Machine
    public enum State { onGround, inAir, jumping };
    [Header("- Player State Machine -")]
    public State state;

    //Input System
    [Header("- Player Inputs -")]
    public InputActionMap input;

    //Initialize Components
    [Header("- Player Components -")]
    [SerializeField] private Camera playerCam;
    private Rigidbody rb; 

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void Start() {
        //Assign inputs
        input = GameController.instance.input;
        input.Enable();

        //Start in the air by default
        state = State.inAir;

        //Assign components
        rb = GetComponent<Rigidbody>();

        //Hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate() {
        //Update Player position
        UpdatePosition();

        //Switch between player states
        switch(state) {
            case State.onGround:
                //Allow the player to move around while on ground
                speed = Vector3.ClampMagnitude(
                    speed + Move(input.FindAction("Move").ReadValue<Vector2>(), moveSpeed),
                    maxSpeed
                );

                //Prevent the player from going through the ground
                speed.y = Mathf.Clamp(speed.y, 0.0f, 1000.0f);

                //Add friction to the player when on the ground
                Friction(friction);
            break;

            case State.inAir:
                //Make the player fall down when in the air
                Gravity();
            break;
        }
    }

    /* ----------------------------------------------------------
                        Action Methods
    ---------------------------------------------------------- */

    //Move a vector Horizontally
    private Vector3 Move(Vector2 direction, float intensity) {
        return (playerCam.transform.forward * direction.y * intensity) + 
            (playerCam.transform.right * direction.x * intensity);
    }

    //Jump
    private void Jump(float power) => speed.y = power;

    /* ----------------------------------------------------------
                        Utility Methods
    ---------------------------------------------------------- */

    //Update Player position
    private void UpdatePosition() {
        rb.position += speed * Time.deltaTime;
    }

    //Apply Friction
    private void Friction(float intensity) {
        if (speed.x > -0.5f && speed.x < 0.5f) { speed.x = 0.0f; }
        else { speed.x += (-Mathf.Sign(speed.x) * intensity) * Time.deltaTime; }

        if (speed.z > -0.5f && speed.z < 0.5f) { speed.z = 0.0f; }
        else { speed.z += (-Mathf.Sign(speed.z) * intensity) * Time.deltaTime; }
    }

    //Apply Gravity
    private void Gravity(float intensity = 30.0f, float scale = 1.0f, float maxIntensity = 50.0f) {
        if (speed.y >= -maxIntensity) { 
            speed.y -= (intensity * scale) * Time.deltaTime;
        }
    }

    /* ----------------------------------------------------------
                            Collisions
    ---------------------------------------------------------- */

    private void OnCollisionEnter(Collision col) {
        //Touch the ground
        if (col.collider.tag == "Ground") { 
            state = State.onGround; 
            jumps = maxJumps;
        }
    }

    private void OnCollisionExit(Collision col) {
        //Get in the air
        if (col.collider.tag == "Ground") {
            state = State.inAir;
        }
    }
}
