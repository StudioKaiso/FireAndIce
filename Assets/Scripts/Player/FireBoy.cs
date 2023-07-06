using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBoy :  MonoBehaviour {
    //Initialize Variables
    private bool inAir;
    private Vector3 refVelocity = Vector3.zero;

    //Initialize Components
    [SerializeField] private Player player;
    private Animator anim;

    private void Start() {
        //Assign Components
        player = GetComponentInParent<Player>();
        anim = GetComponent<Animator>();
    }

    private void Update() {
        if (player.input.FindAction("Move").ReadValue<Vector2>() != Vector2.zero) {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(new Vector3(player.speed.x, 0.0f, player.speed.z), Vector3.up), 
                (player.moveSpeed * 300.0f) * Time.deltaTime
            );
        }
        //Control the behaviour of this script according to the player states
        switch (player.state) {
            case Player.State.onGround:
                //Set the player back on the ground
                inAir = false;
            break;

            case Player.State.inAir:
                //Set the player in the air
                inAir = true;
            break;
        }

        //Update the animations
        anim.SetBool("InAir", inAir);
        anim.SetFloat("VertSpeed", player.speed.y);
        anim.SetFloat("HorSpeed", 
            Mathf.Lerp(
                0.0f, 2.0f, 
                Vector3.Magnitude(new Vector3(player.speed.x, 0.0f, player.speed.z)) / player.maxSpeed
            )
        );
    }
}
