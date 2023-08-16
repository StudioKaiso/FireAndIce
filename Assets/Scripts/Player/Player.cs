using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character {
    //Player Variables
    [Header("- Player Variables -")]
    public int playerId;
    public int playerCharacter = -1;

    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashCooldown;
    [SerializeField] private bool isDashing;
    
    //Initialize Components
    [Header("- Player Components -")]
    public CamTarget cameraTargetComponent;

    //Input System
    [Header("- Player Inputs -")]
    public InputActionMap input;

    //Initialize Player Events
    public delegate void AnimationTrigger(Transform parent, string animation);
    public static event AnimationTrigger onTriggerAnimation;

    public delegate void PlayerAction(Transform parent);
    public static event PlayerAction onPlayerSkill;

    private void OnEnable() => input.Enable();
    private void OnDisable() {
        input.Disable();
        onTriggerAnimation = null;
        onPlayerSkill = null;
    }

    protected override void Start() { base.Start();
        //Define character Relative space
        if (GameObject.FindGameObjectWithTag("CameraTarget") != null) {
            relativeSpace = GameObject.FindGameObjectWithTag("CameraTarget").transform;
            cameraTargetComponent = relativeSpace.GetComponent<CamTarget>();
        } else { relativeSpace = Camera.main.transform; }

        //Hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //---------------- Set up player character ----------------
        
        if (GameController.instance != null) { 
            if (!GameController.instance.players.Contains(this)) {
                GameController.instance.players.Add(this);
                playerId = GameController.instance.players.Count;
            }

            if (input.actions.Count == 0) { 
                input = GameController.instance.inputs[playerId - 1]; 
                input.Enable();
            }
         }

        SpawnCharacter(playerCharacter);

        canMove = true;
        states.Add("dodging", 2);
        states.Add("attacking", 3);

        //Player Input Events
        input.FindAction("Move").performed += ctx => {
            airDirection = new Vector3(
                Mathf.Clamp(direction.x + (ctx.ReadValue<Vector2>().x / 2), -1.0f, 1.0f), 0.0f,
                Mathf.Clamp(direction.z + (ctx.ReadValue<Vector2>().y / 2), -1.0f, 1.0f)
            );

            direction = new Vector3(ctx.ReadValue<Vector2>().x, 0.0f, ctx.ReadValue<Vector2>().y);
        };

        input.FindAction("Move").canceled += ctx => direction = Vector3.zero;

        input.FindAction("Jump").started += ctx => Jump(jumpSpeed);

        input.FindAction("Dash").started += ctx => Dash(dashSpeed);

        //----------------- Subscribe to Events -------------------

        CharacterAnimator.onStateSwitch += (animator, nextState) => {
            if (animator.transform.parent == this.transform) {
                state = states[nextState];
                if (nextState == "onGround") { jumps = maxJumps; }
            }

            //Refresh the dash ability if the player dashed
            if (isDashing) { StartCoroutine(DashCooldown(dashCooldown)); }
        };
    }

    protected override void FixedUpdate() { base.FixedUpdate();
        //Specific player behaviour
        if (state == states["onGround"]) {
            //Fix the camera to follow the player
            if (cameraTargetComponent != null) { cameraTargetComponent.followHeight = true; }

            //Allow the character to move around
            if (canMove) { hSpeed = Move(hSpeed, direction, moveSpeed, maxSpeed); }
        }

        if (state == states["inAir"]) {
            //Allow the character to move around in the air
            if (canMove) { hSpeed = Move(hSpeed, airDirection, moveSpeed, maxSpeed); }

            //Follow the player only when falling
            if (cameraTargetComponent != null) {
                if (vSpeed > 0) { cameraTargetComponent.followHeight = false; }
                else { cameraTargetComponent.followHeight = true; }
            }
            
        }

        if (state == states["dodging"]) {
            //Remove the ability to dash again during dash
            isDashing = true;
            vSpeed = 0.0f;
        }
    }

    /* ----------------------------------------------------------
                        Player Action Methods
    ---------------------------------------------------------- */

    //Move the player rapidly in a specific direction
    private void Dash(float power) {
        if (!isDashing) {
            if (onPlayerSkill != null) { onPlayerSkill(this.transform); }
            if (onTriggerAnimation != null) { onTriggerAnimation(this.transform, "player_dash"); }

            state = states["dodging"];
            jumps = 0;

            if (direction != Vector3.zero) {
                StartCoroutine(ImpulseForce(
                    Quaternion.LookRotation(relativeSpace.forward) * (direction * power), 15.0f * Time.deltaTime)
                );
            } else {
                StartCoroutine(ImpulseForce(normalDirection.up * power, 2.5f * Time.deltaTime));
            }
        }
    }

    /* ----------------------------------------------------------
                        Player Utility Methods
    ---------------------------------------------------------- */

    //Dash Handling 
    private IEnumerator DashCooldown(float targetTime) {
        float dashTimer = 0.0f;
        while (dashTimer < targetTime) {
            if (!inAir) { dashTimer += Time.deltaTime; }
            yield return null;
        } isDashing = false;
    }

    //Spawn the character
    private void SpawnCharacter(int character) {
        if (GameController.instance.characters[character] != null)
            Instantiate(GameController.instance.characters[character], this.transform);
    }
}
