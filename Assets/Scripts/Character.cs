using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(Collider))]
public class Character : MonoBehaviour {
    //Initialize Variables
    [Header("- Character Variables -")]
    public Transform relativeSpace;
    public Vector3 hSpeed;
    public float vSpeed;
    public Vector3 direction, airDirection;

    public float moveSpeed, maxSpeed, turnSpeed;
    [SerializeField] protected float jumpSpeed;
    protected int jumps;
    [SerializeField] protected int maxJumps;

    [SerializeField] private float airFriction = 5.0f;
    protected float friction;
    protected bool canMove;
    protected bool inAir;

    [SerializeField] protected LayerMask groundLayer;
    protected Vector3 groundDirection;
    [SerializeField] protected Transform normalDirection;

    //State Machine
    [Header("- Character State Machine -")]
    public Dictionary<string, int> states = new Dictionary<string, int>() { {"inAir", 0}, {"onGround", 1} };
    public int state;

    //Initialize Components
    [Header("- Character Components -")]
    [SerializeField] protected CharacterController cc;

    protected virtual void Start() {
        cc = GetComponent<CharacterController>();
        if (relativeSpace == null) { relativeSpace = transform; }
    }

    protected virtual void FixedUpdate() {
        //Update Player position
        UpdatePosition();

        //Check for ground collision entries and exits
        GroundCollisions();

        //Switch between player states
        if (state == states["onGround"]) { inAir = false; 
            airDirection = direction;

            //Prevent the player from going through the ground
            // hSpeed.y = Mathf.Clamp(hSpeed.y, 0.0f, 1000.0f);

            //Add friction to the player when on the ground
            if (direction == Vector3.zero) Friction(friction);
        }

        if (state == states["inAir"]) { inAir = true;
            //Make the player fall down when in the air
            Gravity();
            
            //Add friction to the player when in the air
            if (direction == Vector3.zero) Friction(airFriction);
            
            
        }
    
    }

    /* ----------------------------------------------------------
                             Action Methods
    ---------------------------------------------------------- */

    //Move a vector according to a relative space
    protected Vector3 Move(Vector3 target, Vector3 direction,
    float intensity, float maxIntensity) {
        return Vector3.ClampMagnitude (
            target + (
                (normalDirection.up * direction.z * intensity) + 
                (normalDirection.right * -direction.x * intensity)
            ), maxIntensity
        );
    }

    //Jump
    protected void Jump(float power) {
        if (jumps > 0) { 
            StartCoroutine(ImpulseVertical(power, 1.0f * Time.deltaTime));
            jumps--; 
        }    
    }

    /* ----------------------------------------------------------
                           Utility Methods
    ---------------------------------------------------------- */

    //Update Player position
    protected void UpdatePosition() {
        GetComponent<CharacterController>().Move(new Vector3(hSpeed.x, hSpeed.y + vSpeed, hSpeed.z) * Time.deltaTime);
        normalDirection.rotation = Quaternion.LookRotation(groundDirection, relativeSpace.forward);
    }

    private void GroundCollisions() {
        RaycastHit groundHit;
        float distance = GetComponent<Collider>().bounds.extents.y * 1.1f;
        Vector3 origin = new Vector3 (transform.position.x, 
            transform.position.y + GetComponent<Collider>().bounds.extents.y, 
            transform.position.z
        );

        if (Physics.Raycast(origin, Vector3.down, out groundHit, distance, groundLayer)) {
            //Has touched the ground
            if (inAir) {
                jumps = maxJumps;
                state = states["onGround"];
                friction = groundHit.collider.material.staticFriction;

                vSpeed = 0;
            } else {
                hSpeed.y = 0;
                if (Mathf.Abs(vSpeed) < 1.0f) {
                    transform.position = new Vector3(transform.position.x, groundHit.point.y, transform.position.z);    
                }
            }

            //Get the Grounds angle
            groundDirection = groundHit.normal;
        } else {
            //Has left the ground
            if (!inAir) { state = states["inAir"];}

            //Get the Grounds angle
            groundDirection = relativeSpace.up;
        }
    }

    //Apply Friction
    protected void Friction(float intensity) {
        if (hSpeed.x > -0.5f && hSpeed.x < 0.5f) { hSpeed.x = 0.0f; }
        else { hSpeed.x += (-Mathf.Sign(hSpeed.x) * intensity) * Time.deltaTime; }
        
        if (hSpeed.y > -0.5f && hSpeed.y < 0.5f) { hSpeed.y = 0.0f; }
        else { hSpeed.y += (-Mathf.Sign(hSpeed.y) * intensity) * Time.deltaTime; }

        if (hSpeed.z > -0.5f && hSpeed.z < 0.5f) { hSpeed.z = 0.0f; }
        else { hSpeed.z += (-Mathf.Sign(hSpeed.z) * intensity) * Time.deltaTime; }
    }

    //Apply Gravity
    protected void Gravity(float intensity = 30.0f, float scale = 1.0f, float maxIntensity = 75.0f) {
        if (vSpeed >= -maxIntensity) { 
            vSpeed -= (intensity * scale) * Time.deltaTime;
        }
    }

    //Impulse
    protected IEnumerator ImpulseForce(Vector3 target, float targetTime) {
        float currentTime = 0;
        Vector3 startSpeed = hSpeed;
        while (currentTime < targetTime) {
            hSpeed = Vector3.Lerp(startSpeed, target, currentTime / targetTime);
            currentTime += Time.deltaTime;
            yield return null;
        } hSpeed = target;
    }

    protected IEnumerator ImpulseVertical(float target, float targetTime) {
        float currentTime = 0;
        float startSpeed = vSpeed;
        while (currentTime < targetTime) {
            vSpeed = Mathf.Lerp(startSpeed, target, currentTime / targetTime);
            currentTime += Time.deltaTime;
            yield return null;
        } vSpeed = target;
    }
}
