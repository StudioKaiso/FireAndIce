using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class CharacterAnimator :  MonoBehaviour {
    //Initialize Variables
    private bool inAir;

    //Initialize Components
    [SerializeField] private Character character;
    [SerializeField] private Player player;
    private Animator anim;

    private void Start() {
        //Assign Components
        character = GetComponentInParent<Character>();
        player = character.GetComponent<Player>();
        anim = GetComponent<Animator>();

        //Assign Events
        Player.onTriggerAnimation += (animation) => anim.Play(animation);
    }

    private void Update() {
        //Control the behaviour of this script according to the character states
        if (character.state == character.states["onGround"]) {
            //Set the character back on the ground
            inAir = false;

            //Rotate the character to the movement direction
            RotateCharacter(new Vector3(character.speed.x, 0.0f, character.speed.z));    
        }

        if (character.state == character.states["inAir"]) { 
            //Set the character in the air
            inAir = true;

            //Rotate the character to the direction they are trying to go
            if (player != null) {
                if (player.input.FindAction("Move").ReadValue<Vector2>() != Vector2.zero) {
                    RotateCharacter(player.cameraTarget.rotation * 
                        new Vector3 (
                            player.input.FindAction("Move").ReadValue<Vector2>().x, 0.0f,
                            player.input.FindAction("Move").ReadValue<Vector2>().y
                        )
                    );
                }
            } else { 
                RotateCharacter(new Vector3(character.speed.x, 0.0f, character.speed.z));
            }
        }

        //Update the animations
        if (anim != null) { HandleAnimation(); }
    }
    
    /* ----------------------------------------------------------
                           Utility Methods
    ---------------------------------------------------------- */

    private void RotateCharacter(Vector3 target) {
        if (new Vector3 (character.speed.x, 0.0f, character.speed.z) != Vector3.zero) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                Quaternion.LookRotation(target, Vector3.up), 
                character.turnSpeed * Time.deltaTime
            );
        }
    }

    private void HandleAnimation() {
        anim.SetBool("InAir", inAir);

        anim.SetFloat("VertSpeed", character.speed.y);

        anim.SetFloat("HorSpeed", Mathf.Lerp(
            0.0f, 2.0f, Vector3.Magnitude(
                new Vector3(character.speed.x, 0.0f, character.speed.z)) / character.maxSpeed
            )
        ); 
    }
}
