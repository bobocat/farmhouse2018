using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Loco.VR.Movement;

#if UNITY_EDITOR
[CustomEditor(typeof(ArmSwing))]
public class ArmSwingEditor : Editor {

	SerializedProperty s_MotionType;

	SerializedProperty s_SwimVel;
	SerializedProperty s_SwingSlideTime;
	SerializedProperty s_SwimSettings;

	SerializedProperty s_SkiVel;
	SerializedProperty s_SkiSlideTime;

	SerializedProperty s_ArmSwingVel;
	SerializedProperty s_ArmSwingSlideTime;

	void OnEnable() {
		s_MotionType = serializedObject.FindProperty("motionType");

		s_SwimVel = serializedObject.FindProperty("maxSwimVelocity");
		s_SwingSlideTime = serializedObject.FindProperty("swimSlideTime");
		s_SwimSettings = serializedObject.FindProperty("swimSettings");

		s_SkiVel = serializedObject.FindProperty("maxSkiVelocity");
		s_SkiSlideTime = serializedObject.FindProperty("SkiSlideTime");

		s_ArmSwingVel = serializedObject.FindProperty("maxArmSwingVelocity");
		s_ArmSwingSlideTime = serializedObject.FindProperty("ArmSwingSlideTime");
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();
		DrawDefaultInspector();
		EditorGUILayout.PropertyField(s_MotionType);

		ArmSwing.eMotionType mt = (ArmSwing.eMotionType)s_MotionType.enumValueIndex;

		switch(mt){
			case ArmSwing.eMotionType.ARM_SWING:
				EditorGUILayout.LabelField("Arm Swing Motion Settings", EditorStyles.boldLabel);
				EditorGUILayout.Slider(s_ArmSwingVel, 0.1f, 1.0f, new GUIContent("maxArmSwingVelocity"));
				EditorGUILayout.Slider(s_ArmSwingSlideTime, 0.1f, 1.5f, new GUIContent("ArmSwingSlideTime"));
				break;
			case ArmSwing.eMotionType.BREAST_STROKE: 
				EditorGUILayout.LabelField("Swim Motion Settings", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(s_SwimSettings, true);
				EditorGUILayout.Slider(s_SwimVel, 0.1f, 1.0f, new GUIContent("maxSwimVelocity"));
				EditorGUILayout.Slider(s_SwingSlideTime, 0.1f, 1.5f, new GUIContent("swimSlideTime"));
				break;
			case ArmSwing.eMotionType.SKIING:
				EditorGUILayout.LabelField("Ski Motion Settings", EditorStyles.boldLabel);
				EditorGUILayout.Slider(s_SkiVel, 0.1f, 1.0f, new GUIContent("maxSkiVelocity"));
				EditorGUILayout.Slider(s_SkiSlideTime, 0.1f, 1.5f, new GUIContent("SkiSlideTime"));
				break;
		}

		serializedObject.ApplyModifiedProperties();
	}
}
#endif