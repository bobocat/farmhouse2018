//            ======= Copyright (c) BlackPandaStudios, All rights reserved. =======        
//              __________________________________________________________________  
// <author>    | -  Joel Gabriel                                                  |
// <date>      | -  30/03/2017                                                    |
// <name>      | -  VRCharacterController.cs                                      |
// <summary>   | -  This class is a wrapper for handling the real time locomotion |                 
//             |    logic swapping, as well as storing reference to the unity     |         
//             |    events for developers to easily invoke from the inspector.    |
//             |__________________________________________________________________|

using UnityEngine;
using UnityEngine.Events;   
using Loco.VR.Movement;
using Loco.VR.Utils;
using UnityEngine.AI;

namespace Loco.VR.Character {
    [HelpURL(@"Assets/Locomotion Essentials/VR/")] //FIXME: helpURL doesn't show in inspector?
    public class VRCharacterController : MonoBehaviour {

        #region Serialized Variables

        [Header("Speed Settings")]

        [SerializeField] private float bobMoveSpeed;
        [SerializeField] private float armSwingMoveSpeed;
        [SerializeField] private float pulleyMoveSpeed;

        [Header("Play Space Options")]     
        [SerializeField] [Tooltip("Used to quickly shift rotation by 90 degrees, swipe in the desired direction on the VIVE trackpad to rotate!")]
        private bool useQuickRotate = false;

        [Header("Head Bob Events")]
        [SerializeField] [Tooltip("This event is invoked for every single 'bob' detected")] private UnityEvent OnBob;
        [SerializeField] [Tooltip("This event is invoked when 'bobbing' starts")]           private UnityEvent OnBobbingStart;
        [SerializeField] [Tooltip("This event is invoked when 'bobbing' ends")]             private UnityEvent OnBobbingEnd;

        [Header("Pulley Events")]
        [SerializeField] private UnityEvent OnPullBegin;
        [SerializeField] private UnityEvent OnPullEnd;

        [Header("Locomotion Events")]
        [SerializeField] private UnityEvent OnLocomotionSwap;

        #endregion

        #region Private Variables

        private Utility.eMovementType   m_movementType;
        private VRLocomotionRig         m_locoRig;
        private HeadBob                 m_hbMovement;
        private ArmSwing                m_asMovement;
        private Pulley                  m_pMovement;
        //DEVELOPER: add you custom movement here!
        private const string HeadBobCollisionTag  = "HeadBobZone";
        private const string ArmSwingCollisionTag = "ArmSwingZone";
        private const string PulleyCollisionTag   = "PulleyZone";

        private AudioSource source;
        // DEVELOPER: add a string for your zone tag name! dont forget to create the tag in unity and apply it
        //private const string DevCollisionTag      = "DeveloperZone";

        #endregion

        #region Unity Methods

        void Awake() {    
            m_locoRig = GetComponent<VRLocomotionRig>();

            source = GetComponent<AudioSource>();

            try {
                m_hbMovement = m_locoRig.s_head.GetComponent<HeadBob>();
                m_asMovement = m_locoRig.s_head.GetComponent<ArmSwing>();
                m_pMovement = m_locoRig.s_head.GetComponent<Pulley>();
            } catch {
                Debug.LogError("there are no movement components attached to your VR rig! please ensure you did the setup correctly!");
            }
            AutoDetectCurrentMovement();
        }

        void OnTriggerEnter (Collider col) {
            switch ( col.tag ) {
                case HeadBobCollisionTag: {
                        SwapMovement(Utility.eMovementType.BOBBING);
                        GetComponent<NavMeshAgent>().enabled = true;
                        transform.position = new Vector3(transform.position.x, -0.07f, transform.position.z);
                    } break;
                case PulleyCollisionTag: {
                        SwapMovement(Utility.eMovementType.PULLEY);
                        GetComponent<NavMeshAgent>().enabled = false;
                    } break;
                // DEVELOPER: add a swap movement case here!
            }
        }

        void OnTriggerStay(Collider col) {
            if (GetComponent<NavMeshAgent>().enabled == false) {
                switch (col.tag) {
                    case HeadBobCollisionTag: {
                       GetComponent<NavMeshAgent>().enabled = true;
                    } break;
                }
            }
        }

       void OnTriggerExit (Collider col) {
            switch ( col.tag ) {
                case PulleyCollisionTag: {
                        SwapMovement(Utility.eMovementType.BOBBING);
                        GetComponent<NavMeshAgent>().enabled = true;
                        transform.position = new Vector3(transform.position.x, -0.07f, transform.position.z);
                    } break;
                case HeadBobCollisionTag: GetComponent<NavMeshAgent>().enabled = true; break;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Auto detects your initial movement setup based on the settings used to setup your custom loco VR rig.
        /// </summary>
        private void AutoDetectCurrentMovement() {
            if ( m_hbMovement.enabled ) {
                m_movementType = Utility.eMovementType.BOBBING;
                Debug.Log("Bob movement detected... SETTING IT UP FOR YOU!");
            } else if ( m_asMovement.enabled ) {
                m_movementType = Utility.eMovementType.ARM_SWING;
                Debug.Log("Arm Swing movement detected... SETTING IT UP FOR YOU!");
            } else if ( m_pMovement.enabled ) {
                m_movementType = Utility.eMovementType.PULLEY;
                Debug.Log("Pulley movement detected... SETTING IT UP FOR YOU!");
            //DEVELOPER: add an elseif for this logic to allow for autodetect on your movement
            } else {
                Debug.LogError("Your VR rig has not been setup properly! no movement types are enabled!");
            }
        }
        /// <summary>
        /// Looks at your previous movement type and new movement type, and disables and enables the correct component.
        /// </summary>
        /// <param name="a_previousMove">The previous movement type</param>
        /// <param name="a_newMove">The movement type you are changing to</param>
        private void disableInactiveMovement(Utility.eMovementType a_previousMove, Utility.eMovementType a_newMove) {
            switch ( a_previousMove ) {
                case Utility.eMovementType.ARM_SWING: m_asMovement.enabled = false;  break;
                case Utility.eMovementType.BOBBING:   m_hbMovement.enabled = false;  break;
                case Utility.eMovementType.PULLEY:    m_pMovement.enabled  = false; m_pMovement.ResetValues(); break;
                //DEVELOPER: you know what to do!
            }
            switch ( a_newMove ) {
                case Utility.eMovementType.ARM_SWING: m_asMovement.enabled = true;   break;
                case Utility.eMovementType.BOBBING:   m_hbMovement.enabled = true;   break;
                case Utility.eMovementType.PULLEY:    m_pMovement.enabled  = true;   break;
                //DEVELOPER: you know what to do!
            }
        }

        #endregion

        #region Public Methods

        public void RotateLeft() {
            if(useQuickRotate) transform.Rotate(new Vector3(0f, -90.0f, 0f), Space.Self);
        }
        public void RotateRight() {
            if (useQuickRotate) transform.Rotate(new Vector3(0f, 90.0f, 0f), Space.Self);
        }
        /// <summary>
        /// Simply swaps between what movement logic you want to use.
        /// <see cref="Utility.eMovementType.BOBBING"/>, 
        ///  <para>Represents Bobbing Logic.</para>
        /// <see cref="Utility.eMovementType.PULLEY"/>, 
        ///  <para>Represents Pulley Logic.</para>
        /// <see cref="Utility.eMovementType.ARM_SWING"/> 
        ///  <para>Represents Arm Swing Logic.</para>
        /// <param name="a_newType">What movement type do you want to switch to?</param>
        /// </summary>
        public void SwapMovement( Utility.eMovementType a_newType ) {
            Utility.eMovementType prevType = m_movementType;
            m_movementType = a_newType;
            disableInactiveMovement(prevType, a_newType);
            Debug.Log("Swapping to" + a_newType);
        }
        public void InvokeBobEvent() {
            OnBob.Invoke();
        }
        public void InvokeBobbingStartEvent() {
            OnBobbingStart.Invoke();
        }
        public void InvokeBobbingEndEvent() {
            OnBobbingEnd.Invoke();
        }
        public void InvokeMoveSwapEvent() {
            OnLocomotionSwap.Invoke();
        }

        public void InvokePullStartEvent() {
            OnPullBegin.Invoke();
        }

        public void PlaySound(AudioClip clip){
            source.PlayOneShot(clip);
        }

        #endregion
    }
}
