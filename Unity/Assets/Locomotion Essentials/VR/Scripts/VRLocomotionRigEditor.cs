//          ======= Copyright (c) BlackPandaStudios, All rights reserved. =======
//              _____________________________________________________________ 
// <author>    | -  Joel Gabriel                                             |  
// <date>      | -  28/03/2017                                               |  
// <name>      | -  VRLocomotionRigEditor.cs                                 |  
// <summary>   | -  This just does some simple Editor logic to setup the     |            
//             |    inspector in unity. Also does the logic for adding a     |            
//             |    Button to the inspector that simply runs the auto setup. | 
//             |_____________________________________________________________|

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
namespace Loco.VR {
	[CustomEditor(typeof(VRLocomotionRig)), CanEditMultipleObjects]
	public class VRLocomotionRigEditor : Editor {

        #region Serialized Properties

        SerializedProperty s_head;
		SerializedProperty s_handLeft;
		SerializedProperty s_handRight;

        #endregion

        #region Unity Methods

		public override void OnInspectorGUI() {
			serializedObject.Update();

            s_head = serializedObject.FindProperty("s_head");
            s_handLeft = serializedObject.FindProperty("s_handLeft");
            s_handRight = serializedObject.FindProperty("s_handRight");

			VRLocomotionRig vrLocoRig = (VRLocomotionRig)target;

			if (GUILayout.Button(new GUIContent("Auto Detect",
				"Press Auto Detect to automatically fill in the needed variables from the camera rig"))) {
				    vrLocoRig.AutoSetup();
			}

			EditorGUILayout.PropertyField(s_head, new GUIContent("Head",
				"the head transform on the VR camera rig"));
			EditorGUILayout.PropertyField(s_handLeft, new GUIContent("Hand Left",
				"the left hand transform on the VR camera rig"));
			EditorGUILayout.PropertyField(s_handRight, new GUIContent("Hand Right",
				"the right hand transform on the VR camera rig"));

			serializedObject.ApplyModifiedProperties();
		}

        #endregion
    }
}

#endif