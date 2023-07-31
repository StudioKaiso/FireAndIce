using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireboy : MonoBehaviour {
    //Initialize Variables
    [Header("- FireBoy Variables -")]
    [SerializeField] private float fireHairTime;
    private float fireHairIntensity;

    //Initialize Components
    [Header("- FireBoy Components -")]
    [SerializeField] private Material hairMat; 
    [SerializeField] private Material eyesMat;
    private Light fireLight;
    private ParticleSystem fireHairFx;
    private Character character;

    private void Start() {
        //Assign Components
        character = GetComponentInParent<Character>();
        fireLight = GetComponentInChildren<Light>();
        fireHairFx = GetComponentInChildren<ParticleSystem>();

        //Subscribe to events
        Player.onPlayerSkill += (player) => { 
            if (player == transform.parent) { FireHair(); } 
        };
    }

    private void Update() {
        //Control the intensity of the fire hair
        if (hairMat != null) { hairMat.SetFloat("_FireIntensity", Mathf.Clamp01(fireHairIntensity)); }
        if (eyesMat != null) { eyesMat.SetFloat("_FireIntensity", Mathf.Clamp01(fireHairIntensity)); }
        if (fireLight != null) { fireLight.intensity = Mathf.Clamp01(fireHairIntensity) * 2.0f; }

        //Fade the fire hair over time
        if (fireHairIntensity > 0) { fireHairIntensity -= Time.deltaTime; }
        else { fireHairIntensity = 0.0f; }
    }

    //Change fireboy's hair into fire
    private void FireHair() {
        fireHairIntensity = fireHairTime;
        if (fireHairFx != null) { fireHairFx.Play(); }
    }
}
