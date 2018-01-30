using Loco.VR.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

namespace Loco.VR.Movement {
    public class ArmSwing : MonoBehaviour {
		
		public enum eMotionType{
			ARM_SWING,
			SKIING,
			BREAST_STROKE/*,
			Developer can add more?*/
		};

		public enum eSwimSettings  {
			FREE,
			LOCKED_TO_NAV_MESH
		}
			
		private VRBasicController leftController;
		private VRBasicController rightController;
		private VRBasicController activeController;

		[Header("Speed Settings")]
        [SerializeField] private float moveSpeed = 5.0f;
        [SerializeField] private float swingForceMultiplier = 1.5f;

        [Header("Movement Settings")]
		[SerializeField] private VRBasicController.ViveButton runButton;
		[SerializeField] private bool isToggle = false;

		//-------------------------------------------------------------------------------
		[HideInInspector]  public eMotionType motionType;
		[HideInInspector]  public eSwimSettings swimSettings;

		[HideInInspector][SerializeField] private float maxSwimVelocity = 0.35f;
		[HideInInspector][SerializeField] private float swimSlideTime = 0.5f;

		[HideInInspector][SerializeField] private float maxSkiVelocity = 0.2f;
		[HideInInspector][SerializeField] private float SkiSlideTime = 0.25f;

		[HideInInspector][SerializeField] private float maxArmSwingVelocity = 0.2f;
		[HideInInspector][SerializeField] private float ArmSwingSlideTime = 0.25f;

		//--------------------------------------------------------------------------------
		private float zVelLeft;
		private float zVelRight;
		private float xVelLeft;
		private float xVelRight;
        private float swingForce;

		public 	bool toggleOn = false;
		private bool swingingArmBack = false;
        
		private NavMeshAgent agent = null;
		private VRLocomotionRig vrRig = null;

		void Start() {
			vrRig = transform.root.GetComponent<VRLocomotionRig>();
			leftController = vrRig.s_handLeft.GetComponent<VRBasicController>();
			rightController = vrRig.s_handRight.GetComponent<VRBasicController>();
			agent = vrRig.GetComponent<NavMeshAgent>();
			agent.speed = moveSpeed;
		}

		void Update() {
			if (leftController.GetButton((Valve.VR.EVRButtonId)runButton) || rightController.GetButton((Valve.VR.EVRButtonId)runButton) &&
				!isToggle) {
				zVelLeft = vrRig.s_head.transform.InverseTransformDirection(leftController.getDeviceVelocity()).z;
				zVelRight = vrRig.s_head.transform.InverseTransformDirection(rightController.getDeviceVelocity()).z;

				xVelLeft = vrRig.s_head.transform.InverseTransformDirection(leftController.getDeviceVelocity()).x;
				xVelRight = vrRig.s_head.transform.InverseTransformDirection(rightController.getDeviceVelocity()).x;

                if ( swingingArmBack && motionType != eMotionType.BREAST_STROKE ) {

                    float posX = (leftController.getDeviceVelocity().x < 0) ? leftController.getDeviceVelocity().x * -1 : leftController.getDeviceVelocity().x;
                    float posY = (leftController.getDeviceVelocity().y < 0) ? leftController.getDeviceVelocity().y * -1 : leftController.getDeviceVelocity().y;
                    float posZ = (leftController.getDeviceVelocity().z < 0) ? leftController.getDeviceVelocity().z * -1 : leftController.getDeviceVelocity().z;

                    swingForce = Mathf.Max(posX, posY, posZ) * swingForceMultiplier;

                    agent.Move(transform.root.GetComponent<VRLocomotionRig>().s_head.transform.forward * moveSpeed * swingForce * Time.deltaTime);
                } else if ( swingingArmBack && motionType == eMotionType.BREAST_STROKE ) {
                    switch ( swimSettings ) {
                        case eSwimSettings.FREE: {
                            agent.enabled = false;

                            float posX = (leftController.getDeviceVelocity().x < 0) ? leftController.getDeviceVelocity().x * -1 : leftController.getDeviceVelocity().x;
                            float posY = (leftController.getDeviceVelocity().y < 0) ? leftController.getDeviceVelocity().y * -1 : leftController.getDeviceVelocity().y;
                            float posZ = (leftController.getDeviceVelocity().z < 0) ? leftController.getDeviceVelocity().z * -1 : leftController.getDeviceVelocity().z;

                            transform.root.position += transform.forward / ( moveSpeed * 12 ) * ( Mathf.Max(posX, posY, posZ) * 4 );
                            break;
                        }
                        case eSwimSettings.LOCKED_TO_NAV_MESH: agent.Move(transform.root.GetComponent<VRLocomotionRig>().s_head.transform.forward * Time.deltaTime * moveSpeed); break;
                    }
                }

				switch(motionType)
				{
					case eMotionType.ARM_SWING: 			UpdateArmSwing();			 break;
					case eMotionType.SKIING: 		 		 	UpdateSkiing();					 break;
					case eMotionType.BREAST_STROKE:	UpdateBreastStroke();		 break;
					default: Debug.LogWarning("Somehow you have no motion type set??"); break;
				}
			} else if (leftController.GetButtonDown((Valve.VR.EVRButtonId) runButton) || 
				rightController.GetButtonDown((Valve.VR.EVRButtonId) runButton) && isToggle) {
				toggleOn = !toggleOn;
			}

			if(isToggle) {
				if(toggleOn){
					if (swingingArmBack && motionType != eMotionType.BREAST_STROKE) agent.Move(transform.root.GetComponent<VRLocomotionRig>().s_head.transform.forward * Time.deltaTime * moveSpeed);
					else if (swingingArmBack && motionType == eMotionType.BREAST_STROKE) {
						switch(swimSettings) {
							case eSwimSettings.FREE: {
									agent.enabled = false;
									transform.root.position += transform.forward / (moveSpeed * 12);
									break;
								}
							case eSwimSettings.LOCKED_TO_NAV_MESH: agent.Move(transform.root.GetComponent<VRLocomotionRig>().s_head.transform.forward * Time.deltaTime * moveSpeed); break;
						}

						switch(motionType)
						{
							case eMotionType.ARM_SWING: 			UpdateArmSwing();			 break;
							case eMotionType.SKIING: 		 		 	UpdateSkiing();					 break;
							case eMotionType.BREAST_STROKE:	UpdateBreastStroke();		 break;
							default: Debug.LogWarning("Somehow you have no motion type set??"); break;
						}
					}
				} else {
					agent.velocity = Vector3.zero;
#if UNITY_5_6
					agent.isStopped = true;
#else
                    agent.Stop();
#endif
                }
			}
		}

		IEnumerator moveForTime(float a_timeInMs, float a_multiplier = 1.0f){
			while (swingingArmBack) {
				yield return new WaitForSeconds(a_timeInMs);
				swingingArmBack = false;
				yield return null;
			}
		}

		void UpdateArmSwing(){
			if ((zVelLeft   < -maxArmSwingVelocity && zVelRight > maxArmSwingVelocity) ||
				(zVelRight < -maxArmSwingVelocity && zVelLeft   > maxArmSwingVelocity)) {
				swingingArmBack = true;
				StopCoroutine("moveForTime");
				StartCoroutine("moveForTime", ArmSwingSlideTime);
			}
		}
		void UpdateSkiing(){
			if(zVelLeft  < -maxSkiVelocity && zVelRight < -maxSkiVelocity) {
				swingingArmBack = true;
				//Debug.Log("Left " + zVelLeft + " Right " + zVelRight);
				StopCoroutine("moveForTime");
				StartCoroutine("moveForTime", SkiSlideTime);
			}
		}
		void UpdateBreastStroke(){
			if((xVelLeft < -maxSwimVelocity && xVelRight > maxSwimVelocity) ||
				(xVelRight < -maxSwimVelocity && xVelLeft > maxSwimVelocity)){
				swingingArmBack = true;
				StopCoroutine("moveForTime");
				StartCoroutine("moveForTime", swimSlideTime);
			}
		}
	}
}
