//          ======= Copyright (c) BlackPandaStudios, All rights reserved. =======
//              _____________________________________________________________ 
// <author>    | -  Joel Gabriel                                             |  
// <date>      | -  28/03/2017                                               |  
// <name>      | -  VRLocomotionRig.cs                                       |  
// <summary>   | -  This class sets up the key references for the locomotion |            
//             |    rig for use in the auto setup, and to grab from, in code |            
//             |    elsewhere. Also has an autosetup method for ease of use. |  
//             |_____________________________________________________________|

using UnityEngine;
using Loco.VR.Input;	

namespace Loco.VR {
	public class VRLocomotionRig : MonoBehaviour {

        #region Public Variables

        //public InputOptions.Button CheckForMovementButton   = InputOptions.Button.Trigger1;
		//public InputOptions.Button MenuButton               = InputOptions.Button.Trigger1;
        
		[SerializeField]
		public Transform s_head      = null;
		[SerializeField]
		public Transform s_handLeft  = null;
		[SerializeField]
		public Transform s_handRight = null;

        #endregion

        #region Serialized Variables

        [SerializeField]
		private GameObject m_leftController  = null;
		[SerializeField]
		private GameObject m_rightController = null;

        #endregion

        #region Public Methods

        public void AutoSetup(){
            if ( GetComponent<SteamVR_ControllerManager>() != null ) {
                SteamVR_ControllerManager steamVRControllerManager = GetComponent<SteamVR_ControllerManager>();
                s_head = GetComponentInChildren<SteamVR_Camera>().transform;
                s_handLeft = steamVRControllerManager.left.transform;
                s_handRight = steamVRControllerManager.right.transform;
            } else {
                Debug.LogError(
                    "Could not setup SteamVR rig, is this script on the top level of your SteamVR camera prefab?"
                );
            }
		}

        #endregion
    }
}
