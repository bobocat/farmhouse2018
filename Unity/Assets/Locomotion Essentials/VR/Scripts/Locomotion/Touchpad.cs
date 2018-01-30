//          ======= Copyright (c) BlackPandaStudios, All rights reserved. =======
//              _____________________________________________________________ 
// <author>    | -  Joel Gabriel                                             |  
// <date>      | -  28/03/2017                                               |  
// <name>      | -  Touchpad.cs                                              |  
// <summary>   | -  This class handles the logic for all touch-pad movement  |            
//             |    detection and moves you along the navigation mesh        |            
//             |    Including UnityEvents for touchpad interations.          |  
//             |_____________________________________________________________|

using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Loco.Helpers;
using Loco.VR.Character;
using Loco.VR.Input;

namespace Loco.VR.Movement{
    public class Touchpad : MonoBehaviour {

        public enum eMoveDirectionMode {
            GAZE,
            LEFT_COTROLLER,
            RIGHT_CONTROLLER,
            ACTIVE_CONTROLLER
        }      
        
        [Header("Movement Speed Settings")]
        [SerializeField] private float m_walkSpeed = 20.0f;
        [SerializeField] private float m_runSpeed  = 20.0f;

        [Header("Movement Direction Settings")]
        [SerializeField] private eMoveDirectionMode m_directionMode;

        private VRBasicController.ViveButton m_buttonToPress = VRBasicController.ViveButton.TOUCHPAD;

        // private references
        private VRBasicController m_leftController;
        private VRBasicController m_rightController;
        private VRBasicController m_activeController;
        private NavMeshAgent      m_agent; 
        private Transform         m_rootRef;
        private Transform         m_headRef;

        void Start() {
            m_rootRef = transform.root;
            m_headRef = m_rootRef.GetComponent<VRLocomotionRig>().s_head;
            m_agent = m_rootRef.GetComponent<NavMeshAgent>();
            m_leftController = m_rootRef.GetComponent<VRLocomotionRig>().s_handLeft.GetComponent<VRBasicController>();
            m_rightController = m_rootRef.GetComponent<VRLocomotionRig>().s_handRight.GetComponent<VRBasicController>();
        }

        void Update() {
            switch (m_directionMode) {
                case eMoveDirectionMode.ACTIVE_CONTROLLER: {
                        UpdateActiveController();
                        if (m_activeController == null) break;

                        UpdateMove();

                        break;
                    }
                case eMoveDirectionMode.LEFT_COTROLLER: {
                        if (m_leftController.isTouching() || m_rightController.isTouching()) m_activeController = m_leftController;
                        else m_activeController = null;
                        if (m_activeController == null) break;

                        UpdateMove();

                        break;
                    }
                case eMoveDirectionMode.RIGHT_CONTROLLER: {
                        if (m_leftController.isTouching() || m_rightController.isTouching()) m_activeController = m_rightController;
                        else m_activeController = null;
                        if (m_activeController == null) break;

                        UpdateMove();

                        break;
                    }
                case eMoveDirectionMode.GAZE: {
                        UpdateActiveController();
                        if (m_activeController == null) break;

                        UpdateMove();

                        break;
                    }
            }
        }

        void UpdateActiveController() {
            if (m_leftController.isTouching())          m_activeController = m_leftController;         
            else if (m_rightController.isTouching())    m_activeController = m_rightController;
            else                                        m_activeController = null;   
        }

        void UpdateMove() {
            Vector3 moveDir;
            if (m_directionMode == eMoveDirectionMode.GAZE) moveDir = m_headRef.forward;
            else moveDir = m_activeController.GetFaceDirection();

            if (m_activeController.GetButtonDown((Valve.VR.EVRButtonId)m_buttonToPress))
				m_agent.Move(moveDir * m_runSpeed * Time.deltaTime);
			else m_agent.Move(moveDir * m_walkSpeed * Time.deltaTime);
        }
    }
}
