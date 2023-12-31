using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class CharacterAnimator :  MonoBehaviour {
    //Initialize Variables
    private bool inAir;

    //Initialize Components
    private Character character;
    private Player player;
    private Animator anim;

    //Initialize Events
    public delegate void StateSwitch(Transform animator, string state);
    public static event StateSwitch onStateSwitch;

    private void OnDisable() {
        onStateSwitch = null;
    }

    private void Start() {
        //Assign Components
        character = GetComponentInParent<Character>();
        player = character.GetComponent<Player>();
        anim = GetComponent<Animator>();

        //Assign Events
        Player.onTriggerAnimation += (parent, animation) => { if (transform.parent == parent) anim.Play(animation); };
    }

    private void Update() {
        //Control the behaviour of this script according to the character states
        if (character.state == character.states["onGround"]) {
            //Set the character back on the ground
            inAir = false;

            //Rotate the character to the movement direction
            RotateCharacter(character.hSpeed);    
        }

        if (character.state == character.states["inAir"]) { 
            //Set the character in the air
            inAir = true;

            //Rotate the character to the direction they are trying to go
            if (player != null) {
                if (player.direction != Vector3.zero) {
                    RotateCharacter(player.relativeSpace.rotation * 
                        new Vector3 (player.direction.x, 0.0f, player.direction.z)
                    );
                }
            } else { 
                RotateCharacter(character.hSpeed);
            }
        }

        if (character.state == character.states["dodging"]) {
            RotateCharacter(character.hSpeed, 5.0f);
        }

        //Update the animations
        if (anim != null) { HandleAnimation(); }
    }
    
    /* ----------------------------------------------------------
                           Utility Methods
    ---------------------------------------------------------- */

    private void RotateCharacter(Vector3 target, float scale = 1.0f) {
        if (character.hSpeed != Vector3.zero) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                Quaternion.LookRotation(target, Vector3.up), 
                character.turnSpeed * scale * Time.deltaTime
            );
        }
    }

    private void HandleAnimation() {
        anim.SetBool("InAir", inAir);

        anim.SetFloat("VertSpeed", character.vSpeed);

        anim.SetFloat("HorSpeed", Mathf.Lerp(
            0.0f, 2.0f, Vector3.Magnitude( character.hSpeed) / character.maxSpeed)
        ); 
    }

    private void SwitchToNeutral() {
        if (inAir) {
            if (onStateSwitch != null) { onStateSwitch(this.transform, "inAir"); }
        } else {
            if (onStateSwitch != null) { onStateSwitch(this.transform, "onGround"); }    
        }
    }

    private void SwitchToState(string state) {
        if (onStateSwitch != null) { onStateSwitch(this.transform, state); }
    }
}
