using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    //Initialize Variables
    private Vector3 speed;

    [Header("- Player Variables -")]
    [SerializeField] private float moveSpeed;

    //State Machine
    public enum State { onGround, inAir, jumping };
    [Header("- Player State Machine -")]
    public State state;

    //Input System
    [Header("- Player Inputs -")]
    [SerializeField] private InputActionMap input;

    //Initialize Components
    private Rigidbody rb;

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void Start() {
        //Start in the air by default
        state = State.inAir;

        //Assign components
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        //Update Player position
        UpdatePosition();

        //Switch between player states
        switch(state) {
            case State.onGround:
                //Prevent the player from going through the ground
                speed.y = Mathf.Clamp(speed.y, 0.0f, speed.y);
            break;

            case State.inAir:
                Gravity();
            break;
        }
    }

    //Update Player position
    private void UpdatePosition() {
        rb.position += speed * Time.deltaTime;
    }

    //Apply Gravity
    private void Gravity(float intensity = 10.0f, float scale = 1.0f, float maxIntensity = 20.0f) {
        if (speed.y >= -maxIntensity) { 
            speed.y -= (intensity * scale) * Time.deltaTime;
        }
    }

    /* ---------------
    ------------------
        Collisions
    ------------------
    --------------- */

    private void OnCollisionEnter(Collision col) {
        //Touch the ground
        if (col.collider.tag == "Ground") { state = State.onGround; }
    }

    private void OnCollisionExit(Collision col) {
        //Get in the air
        if (col.collider.tag == "Ground") { state = State.inAir; }
    }
}
