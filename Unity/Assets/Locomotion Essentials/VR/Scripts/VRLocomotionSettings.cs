//          ======= Copyright (c) BlackPandaStudios, All rights reserved. =======
//              _____________________________________________________________ 
// <author>    | -  Joel Gabriel                                             |  
// <date>      | -  28/03/2017                                               |  
// <name>      | -  VRLocomotionSettings.cs                                  |  
// <summary>   | -  This is simply a scriptable object that can be modified  |            
//             |    in the inspector, and save as a .settings file.          |            
//             |    This allows you to save any editor settings.             |  
//             |_____________________________________________________________|

using UnityEngine;
using Loco.VR.Utils;

namespace Loco.VR {
    public class VRLocomotionSettings : ScriptableObject {    
        public Utility.eVRType vrType;
        public Utility.eMovementType movementType;
        public float startMoveSpeed;
        public bool haveRigPrefab;
        public GameObject vrRig;
    }
}
