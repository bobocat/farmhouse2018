using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loco.Helpers;
using Loco.VR.Character;
using Loco.VR.Input;

namespace Loco.VR.Movement {
    public class Pulley : MonoBehaviour {

        #region Serialized Variables

        [Header("Controller References")]
        [SerializeField] private VRBasicController leftController;
        [SerializeField] private VRBasicController rightController;

        [Header("Speed Settings")]
        [SerializeField] private float moveSpeed = 5.0f;

        [Header("Direction Settings")]
        [SerializeField] private bool climbOnTrigger = true;
        [SerializeField] private string climbTag = "Rope";
        [SerializeField] private Vector3 climbDirection = new Vector3(0, 1, 0);
        [SerializeField] private bool lockUpDirection = false;
        #endregion

        #region Private Variables

        private enum ePullState {
            PULLING_WITH_LEFT,
            PULLING_WITH_RIGHT,
            PULL_RELEASED,
            PULL_IDLE,
            COUNT
        };

        private ePullState m_pullState         = ePullState.PULL_IDLE;
        private Transform  m_controllerLeft    = null;
        private Transform  m_controllerRight   = null;
        private Vector3    m_grabPoint         = Vector3.zero;
        private Vector3    m_currentPoint      = Vector3.zero;
        private bool       m_grabbing          = false;
        private VRCharacterController cont = null;
        #endregion

        #region Unity Methods

        void Start () {
            leftController  = transform.root.GetComponent<VRLocomotionRig>().s_handLeft.GetComponent<VRBasicController>();
            rightController = transform.root.GetComponent<VRLocomotionRig>().s_handRight.GetComponent<VRBasicController>();
            leftController.ropeTag = climbTag;
            rightController.ropeTag = climbTag;
            cont = transform.root.GetComponent<VRCharacterController>();
        }     
         
        void Update () {
            switch (m_pullState) {
                case ePullState.PULL_IDLE:  UpdateIdle();  break;
                case ePullState.PULLING_WITH_LEFT: UpdateLeftPull(); break;
                case ePullState.PULLING_WITH_RIGHT: UpdateRightPull();    break;
                case ePullState.PULL_RELEASED:          break;
            }
        }

        #endregion

        private void UpdateIdle(){
            if(leftController.GetTriggerDown()) {
                m_grabPoint = leftController.transform.position;
                m_grabbing = true;
                m_pullState = ePullState.PULLING_WITH_LEFT;
                leftController.TriggerHapticPulse();
                if (leftController.hoveringRope) cont.InvokePullStartEvent();
            }

            if (rightController.GetTriggerDown())
            {
                m_grabPoint = rightController.transform.position;
                m_grabbing = true;
                m_pullState = ePullState.PULLING_WITH_RIGHT;
                rightController.TriggerHapticPulse();
                if (rightController.hoveringRope) cont.InvokePullStartEvent();
            }
        }

        private void UpdateLeftPull() {
            m_currentPoint = leftController.transform.position;
            float dist = Vector3.Distance(m_grabPoint, m_currentPoint);
            Vector3 dir = m_grabPoint - m_currentPoint;

            Debug.Log("Hi");

            transform.root.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = moveSpeed;
            //transform.root.GetComponent<UnityEngine.AI.NavMeshAgent>().Move(dir);  
            if (!climbOnTrigger) transform.root.position += dir;
            else if (climbOnTrigger && leftController.hoveringRope) { transform.root.position += new Vector3(0, dir.y, 0); }

			if(rightController.GetTriggerDown()) {
				m_grabPoint = rightController.transform.position;
				m_grabbing = true;
				m_pullState = ePullState.PULLING_WITH_RIGHT;
				rightController.TriggerHapticPulse();
				if (rightController.hoveringRope) cont.InvokePullStartEvent();
			}

            // On trigger release go back to idle
            if (leftController.GetTriggerUp()){
                transform.root.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0;
                m_currentPoint = Vector3.zero;
                m_grabPoint = Vector3.zero;
                m_pullState = ePullState.PULL_IDLE;
                m_grabbing = false;
            }           
        }

        private void UpdateRightPull() {
            m_currentPoint = rightController.transform.position;
            float dist = Vector3.Distance(m_grabPoint, m_currentPoint);
            float rigDist = Vector3.Distance(m_currentPoint, transform.root.position);
            Vector3 dir = m_grabPoint - m_currentPoint;

            //transform.root.GetComponent<UnityEngine.AI.NavMeshAgent>().Move(dir);
            transform.root.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

            if (!climbOnTrigger) transform.root.position += dir;
            else if (climbOnTrigger && rightController.hoveringRope) { transform.root.position += new Vector3(0, dir.y, 0); }

			if(leftController.GetTriggerDown()) {
				m_grabPoint = leftController.transform.position;
				m_grabbing = true;
				m_pullState = ePullState.PULLING_WITH_LEFT;
				leftController.TriggerHapticPulse();
				if (leftController.hoveringRope) cont.InvokePullStartEvent();
			}

            // On trigger release go back to idle
            if (rightController.GetTriggerUp()) {
                //transform.root.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0;
                m_currentPoint = Vector3.zero;
                m_grabPoint = Vector3.zero;
                m_pullState = ePullState.PULL_IDLE;
                rigDist = 0;
                m_grabbing = false;
            }
        }

        public void ResetValues(){
            m_currentPoint = Vector3.zero;
            m_grabPoint = Vector3.zero;
            m_pullState = ePullState.PULL_IDLE;
            m_grabbing = false;
        }
    }
}
