using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character {
    //Player Variables
    [Header("- Player Variables -")]
    [SerializeField] private float dashSpeed, dashCooldown;
    [SerializeField] private bool isDashing;
    //Input System
    [Header("- Player Inputs -")]
    public InputActionMap input;

    //Initialize Components
    [Header("- Player Components -")]
    public Transform cameraTarget;
    public CamTarget cameraTargetComponent;

    //Initialize Player Events
    public delegate void AnimationTrigger(string animation);
    public static event AnimationTrigger onTriggerAnimation;

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

        //Set up player character script
        canMove = true;
        states.Add("dodging", 2);
        states.Add("attacking", 3);

        //Assign components
        rb = GetComponent<Rigidbody>();

        //Hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Input Events
        input.FindAction("Jump").started += ctx => Jump(jumpSpeed);
        input.FindAction("Dash").started += ctx => Dash(dashSpeed);
    }

    protected override void FixedUpdate() { base.FixedUpdate();
        //Specific player behaviour
        if (state == states["onGround"]) {
            //Fix the camera to follow the player
            cameraTargetComponent.followHeight = true;

            //Allow the player to move around
            if (canMove) {
                speed = Move (speed, cameraTarget, 
                    input.FindAction("Move").ReadValue<Vector2>(), 
                    moveSpeed, maxSpeed
                );    
            }
        }
                
        if (state == states["inAir"]) {
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
        }

        if (state == states["dodging"]) {
            //Remove the ability to dash again during dash
            isDashing = true;
        }
    }

    /* ----------------------------------------------------------
                        Player Action Methods
    ---------------------------------------------------------- */

    //Move the player rapidly in a specific direction
    private void Dash(float power) {
        if (!isDashing) {
            if (onTriggerAnimation != null) { onTriggerAnimation("player_dash"); }
            StartCoroutine(DashCooldown(dashCooldown));

            state = states["dodging"];

            speed.y = 1;

            if (input.FindAction("Move").ReadValue<Vector2>() != Vector2.zero) {
                StartCoroutine(ImpulseForce(
                    Quaternion.LookRotation(cameraTarget.forward) * new Vector3(
                        input.FindAction("Move").ReadValue<Vector2>().x * dashSpeed, 0.0f,
                        input.FindAction("Move").ReadValue<Vector2>().y * dashSpeed
                    ), 15.0f * Time.deltaTime)
                );    
            } else {
                StartCoroutine(ImpulseForce(cameraTarget.forward * dashSpeed, 15.0f * Time.deltaTime)); 
            }
        }
    }

    /* ----------------------------------------------------------
                        Player Utility Methods
    ---------------------------------------------------------- */

    //Dash Timer 
    private IEnumerator DashCooldown(float targetTime) {
        float dashTimer = 0.0f;
        while (dashTimer < targetTime) {
            dashTimer += Time.deltaTime;
            yield return null;
        }
        isDashing = false;
    }
}
