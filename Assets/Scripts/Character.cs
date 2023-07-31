using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Character : MonoBehaviour {
    //Initialize Variables
    [Header("- Character Variables -")]
    public Vector3 speed;
    public Vector2 direction;
    public float moveSpeed, maxSpeed, turnSpeed;
    [SerializeField] private float airFriction = 5.0f;
    [SerializeField] protected float jumpSpeed;
    protected int jumps;
    [SerializeField] protected int maxJumps;
    protected float friction;
    protected bool canMove;
    protected bool inAir;

    //State Machine
    [Header("- Character State Machine -")]
    public Dictionary<string, int> states = new Dictionary<string, int>() { {"inAir", 0}, {"onGround", 1} };
    public int state;

    //Initialize Components
    [Header("- Character Components -")]
    [SerializeField] protected Rigidbody rb;

    protected virtual void FixedUpdate() {
        //Update Player position
        UpdatePosition();

        //Switch between player states
        if (state == states["onGround"]) { inAir = false;
            //Prevent the player from going through the ground
            speed.y = Mathf.Clamp(speed.y, 0.0f, 1000.0f);

            //Add friction to the player when on the ground
            Friction(friction);
        }

        if (state == states["inAir"]) { inAir = true;
            //Make the player fall down when in the air
            Gravity();

            //Add friction to the player when in the air
            Friction(airFriction);
        }
    
    }

    /* ----------------------------------------------------------
                             Action Methods
    ---------------------------------------------------------- */

    //Move a vector according to a relative space
    protected Vector3 Move(Vector3 target, Transform relativeSpace, 
    Vector2 direction, float intensity, float maxIntensity) {
        return Vector3.ClampMagnitude(
            target + (
                (relativeSpace.forward * direction.y * intensity) + 
                (relativeSpace.right * direction.x * intensity)
            ), maxIntensity
        );
    }

    //Jump
    protected void Jump(float power) {
        if (jumps > 0) { 
            StartCoroutine(ImpulseForce(Vector3.up * power, 1.0f * Time.deltaTime));
            jumps--; 
        }    
    }

    /* ----------------------------------------------------------
                           Utility Methods
    ---------------------------------------------------------- */

    //Update Player position
    protected void UpdatePosition() {
        rb.position += speed * Time.deltaTime;
    }

    //Apply Friction
    protected void Friction(float intensity) {
        if (speed.x > -0.5f && speed.x < 0.5f) { speed.x = 0.0f; }
        else { speed.x += (-Mathf.Sign(speed.x) * intensity) * Time.deltaTime; }

        if (speed.z > -0.5f && speed.z < 0.5f) { speed.z = 0.0f; }
        else { speed.z += (-Mathf.Sign(speed.z) * intensity) * Time.deltaTime; }
    }

    //Apply Gravity
    protected void Gravity(float intensity = 30.0f, float scale = 1.0f, float maxIntensity = 75.0f) {
        if (speed.y >= -maxIntensity) { 
            speed.y -= (intensity * scale) * Time.deltaTime;
        }
    }

    //Impulse
    protected IEnumerator ImpulseForce(Vector3 target, float targetTime) {
        float currentTime = 0;
        Vector3 startSpeed = speed;
        while (currentTime < targetTime) {
            speed = Vector3.Lerp(startSpeed, target, currentTime / targetTime);
            currentTime += Time.deltaTime;
            yield return null;
        } speed = target;
    }

    /* ----------------------------------------------------------
                            Collisions
    ---------------------------------------------------------- */

    private void OnCollisionEnter(Collision col) {
        //Touch the ground
        if (col.collider.tag == "Ground") { 
            state = states["onGround"]; 
            friction = col.collider.material.staticFriction;
            jumps = maxJumps;
        }
    }

    private void OnCollisionExit(Collision col) {
        //Get in the air
        if (col.collider.tag == "Ground") {
            state = states["inAir"];
        }
    }
}
