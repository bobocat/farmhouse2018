//          ======= Copyright (c) BlackPandaStudios, All rights reserved. =======
//              _____________________________________________________________ 
// <author>    | -  Joel Gabriel                                             |  
// <date>      | -  28/03/2017                                               |  
// <name>      | -  HeadBob.cs                                               |  
// <summary>   | -  This class handles the logic for all Head Bob detection, |            
//             |    as well as auto height recalibration and invokes         |            
//             |    UnityEvents at key points in a head bob.                 |  
//             |_____________________________________________________________|

using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Loco.Helpers;
using Loco.VR.Character;
using Loco.VR.Input;

namespace Loco.VR.Movement{
	public class HeadBob : MonoBehaviour {

        #region Serialized Variables

		public enum eDirectionSettings {
			GAZE,
			CONTROLLER
		}

		[HideInInspector] public bool trackHeadBob = true;

        [Header("Time Settings")]
        [Tooltip("This value is the time you have to move your head above and below the bob threshold.")]
        [SerializeField]  private float maxTimeToBob = 0.5f;

        [Header("Movement Settings")]
        //[Tooltip("This boolean determines whether or not you want to move only when clicking a button")]
        //[SerializeField]
        //private bool holdKeyToMove = false;

		[SerializeField] private eDirectionSettings moveDirSettings = eDirectionSettings.GAZE;
		[SerializeField] private bool disableButton = false;
		[SerializeField] private bool isToggle = true;


		[Tooltip("This is simply the button you want to press, to activate the head bob tracking")]
		[SerializeField] private VRBasicController.ViveButton buttonToPress;

		[Tooltip("This is simply the multiplier value that is multiplied with the bob speed - Fast/big strides = faster movement")]
		[SerializeField] private float bobSpeedMultiplier = 20.0f;

        #endregion

        #region Private Variables

        private enum BobState {
            BOBBING,
            STILL
        }

        private VRCharacterController m_vrCharCont            = null;
        private Transform             m_root                  = null;
        private NavMeshAgent          m_agent                 = null;
        private BobState              m_bobState              = BobState.STILL;
        private float                 m_bobTimer              = 0.0f;
        private float                 m_height                = 1.461786f;
		private bool                  m_moveForwardUp         = false;
		private bool                  m_moveForwardDown       = false;
        private VRBasicController     m_leftController        = null;
        private VRBasicController     m_rightController       = null;
        private VRBasicController     m_activeController      = null;

		private Vector3 startPosition;
		private Vector3 endPosition;
		private Vector3 headVelocity;
		private float bobSpeed = 0.0f;

		[SerializeField] private bool m_toggleOn = false;
        private bool m_pressing = false;
        #endregion

        #region Unity Methods

        void Start() {
            SetUpRootTransform();
            m_leftController = transform.root.GetComponent<VRLocomotionRig>().s_handLeft.GetComponent<VRBasicController>();
            m_rightController = transform.root.GetComponent<VRLocomotionRig>().s_handRight.GetComponent<VRBasicController>();
            m_agent = m_root.GetComponent<UnityEngine.AI.NavMeshAgent>();
            m_vrCharCont = m_root.GetComponent<VRCharacterController>();
		
           // StartCoroutine("RecalibrateOnTick");
			StartCoroutine("TrackHeadsetVelocity");
        }

		void Update () {
			if(!disableButton) {
				if(!isToggle) {
					if (m_leftController.GetButtonDown((Valve.VR.EVRButtonId) buttonToPress)) {
		                m_pressing = true;
		                m_activeController = m_leftController;
					} else if (m_rightController.GetButtonDown((Valve.VR.EVRButtonId) buttonToPress)) {
		                m_pressing = true;
		                m_activeController = m_rightController;
		            }

					if (m_pressing == true && (m_activeController.GetButtonUp((Valve.VR.EVRButtonId) buttonToPress))){
		                    m_pressing = false;
		                    m_activeController = null;
		                    Debug.Log("Stop Tracking");
							m_agent.velocity = Vector3.zero;
#if UNITY_5_6
					        m_agent.isStopped = true;
#else
                            m_agent.Stop();
#endif
                        m_bobState = BobState.STILL;
					}
				} else {
					if (m_leftController.GetButtonDown((Valve.VR.EVRButtonId)   buttonToPress) || 
						m_rightController.GetButtonDown((Valve.VR.EVRButtonId) buttonToPress)) {
						m_toggleOn = !m_toggleOn;
					}
				}
			} else {
				m_pressing = true;
				m_activeController = m_leftController;
			}

			if(isToggle) {
				if(m_toggleOn){
					m_pressing = true;
					m_activeController = m_leftController;
				} else {
					m_pressing = false;
					m_activeController = null;
					m_agent.velocity = Vector3.zero;
#if UNITY_5_6
					m_agent.isStopped = true;
#else
                    m_agent.Stop();
#endif
                    m_bobState = BobState.STILL;
				}
			}
            
            if (TrackingHeadbob()) {
                CheckForHeadBob();
            }
            
            UpdateBobStates();
		}

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates and Updates the headbob 
        /// </summary>
        private void CheckForHeadBob() {
            m_bobTimer += Time.deltaTime;
			if(Helper.TimerElapsed(m_bobTimer, maxTimeToBob)) {
                OnBobbingStopped();
			} else {
				if (bobSpeed > 2 && bobSpeed < 15) m_moveForwardUp = true;

				if(m_moveForwardUp || m_moveForwardDown) {                                                                                              // if this occurs, reset the timer, reset up&forward bools
                    OnBobDetected();
                    Debug.Log("Bob");
				}
			}
        }

        /// <summary>
        /// Updates the current BobState - <para></para>
        ///       <see cref="BobState.BOBBING"/> ,
        ///       <para>Updates the logic for moving you forward through the scene.</para>
        ///       <see cref="BobState.RECALIBRATING"/> ,
        ///       <para>Updates the logic for recalibrating the player height.</para>
        ///       <see cref="BobState.STILL"/> 
        ///       <para>Updates the logic for standing still.</para>
        /// </summary>
        private void UpdateBobStates() {
			bobSpeed = (headVelocity.y < 0) ? (headVelocity.y * -1) * bobSpeedMultiplier : (headVelocity.y) * bobSpeedMultiplier;
//			Debug.Log(bobSpeed);
            switch (m_bobState) {
				case BobState.BOBBING:  
					if(m_agent != null) {
						if(moveDirSettings == eDirectionSettings.GAZE) m_agent.Move(transform.forward * (0.5f * (bobSpeed)) * Time.deltaTime); 
						else if(moveDirSettings == eDirectionSettings.CONTROLLER) {
							m_agent.Move(m_activeController.GetFaceDirection() * (0.5f * (bobSpeed)) * Time.deltaTime); 
						}
					}
					break;    
					//TODO: if both VIVE controller buttons are being pressed, move backwards
				case BobState.STILL: if (m_agent != null && m_agent.enabled) {

#if UNITY_5_6
					    m_agent.isStopped = true;
#else
                        m_agent.Stop();
#endif
                    }
                    break;
            }
        }

        /// <summary>
        /// Wrapper for the logic that will update head bob if it is enabled
		/// <para>Returns <c>true</c>  if you are bobbing, Returns <c>false</c> if you are not.</para>
        /// </summary>
        private bool TrackingHeadbob() {
            if (trackHeadBob == false) {
                m_bobState = BobState.STILL;
                return false;
            }
                if (m_pressing) {
                    return true;
                } else return false;
            

            return true;
        }

        /// <summary>
        /// Recalibrates height every 'X' amount of seconds
        /// </summary> 0.001
	//	private IEnumerator RecalibrateOnTick() {
		//	while(true){
        //        if (m_bobState == BobState.STILL) {
         //           if (transform.position.y < (m_height - heightRecalThreshold) || transform.position.y > (m_height + heightRecalThreshold)) {
        //                m_height = transform.position.y;
        //            }
        //            yield return new WaitForSeconds(0.5f);
      //          }
       //         yield return null;
      //      }
        //}

		private IEnumerator TrackHeadsetVelocity() {
			while(true){
				float time = 0.01f;
				startPosition = transform.position;

				yield return new WaitForSeconds(time);

				endPosition = transform.position;
				headVelocity = (endPosition - startPosition) / time;

				startPosition = Vector3.zero;
				endPosition = Vector3.zero;

				yield return null;
			}
		}

        private void SetUpRootTransform() {
            m_root = transform.root;
            if (m_root == this) {
                Debug.LogWarning("WARNING: this is attached to the root of your VR rig, please ensure that you attach this to your 'eye'\n " +
                                              "or whatever child object has the mainCamera");
            }
        }

        private void OnBobbingStopped() {
            if (m_bobState == BobState.BOBBING) m_vrCharCont.InvokeBobbingEndEvent();
            m_bobState = BobState.STILL;                                        // if the timer exceeds this time, 
            m_moveForwardUp = false;                                            // it means both of the up & down 
            m_moveForwardDown = false;                                          // conditions were not hit in that time,
            m_bobTimer = 0.0f;                                                  // therefore stop movement, proceed back to checking
        }

        private void OnBobDetected()
        {
            m_vrCharCont.InvokeBobEvent();
            m_vrCharCont.InvokeBobbingStartEvent();
            m_moveForwardUp = false;                                                                                                                            // update movement state & check again.
            m_moveForwardDown = false;
            m_bobState = BobState.BOBBING;
            m_bobTimer = 0.0f;
        }

        #endregion
    }
}
