//          ======= Copyright (c) BlackPandaStudios, All rights reserved. =======
//              _____________________________________________________________ 
// <author>    | -  Joel Gabriel                                             |  
// <date>      | -  05/05/2017                                               |  
// <name>      | -  Shoot.cs                                                 |  
// <summary>   | -  This class handles the logic for shooting the gun in the |
//             |    HeadBob demo scene. This script is scene specific.       |
//             |_____________________________________________________________|

using Loco.VR;
using Loco.VR.Input;
using UnityEngine;

public class Shoot : MonoBehaviour {

    public Transform gunEnd;
    public AudioClip shootSound;

    private VRBasicController gunContReference;
    private AudioSource audioSource;

    void Start() {
        gunContReference = transform.root.GetComponent<VRLocomotionRig>().s_handRight.GetComponent<VRBasicController>();
        audioSource = transform.root.GetComponent<AudioSource>();
    }

	void Update () {
        if (TriggerPulled()) {
            ShootGun();
        }
	}

    #region Private Functions

    bool TriggerPulled() { return gunContReference.GetTriggerDown(); }

    void VibrateControllers() { gunContReference.TriggerHapticPulse(); }

    void PlayGunShotSound() { audioSource.PlayOneShot(shootSound); }

    void ShootGun() {
        VibrateControllers();
        PlayGunShotSound();

        Ray ray = new Ray(gunEnd.position, gunEnd.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.GetComponent<Rigidbody>() != null)
            {
                Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                rb.AddForceAtPosition(gunEnd.transform.forward * 5, hit.point, ForceMode.Impulse);
            }
            if (hit.transform.GetComponent<Enemy>() != null)
            {
                Enemy e = hit.transform.GetComponent<Enemy>();
                e.health -= 25;
            }
        }
    }

    #endregion
}
