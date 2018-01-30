//          ======= Copyright (c) BlackPandaStudios, All rights reserved. =======
//              _____________________________________________________________ 
// <author>    | -  Joel Gabriel                                             |  
// <date>      | -  29/03/2017                                               |  
// <name>      | -  VRLocomotionSettingsWindow.cs                            |  
// <summary>   | -  This class handles the logic for creating my custom unity|            
//             |    edtior window, as well as the auto rig setup to use my   |            
//             |    plugin based off the desired settings in this window.    |  
//             |_____________________________________________________________|
// <name>      | -  Utility.cs                                               |  
// <summary>   | -  This class simply provides some public static utility    |            
//             |    variables and functions to make the getting and setting  |            
//             |    of the values in the inspector easier.                   |  
//             |_____________________________________________________________|

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Loco.VR.Utils;
using Loco.VR.Movement;
//using UnityEngine.AI;
using Loco.VR.Character;
using Loco.VR.Input;

namespace Loco.VR.Utils {
    public static class Utility {

        #region Public Variables

        public enum eVRType {
            VIVE,
            OCULUS
        }

        public enum eMovementType {
            BOBBING,
            ARM_SWING,
            BREAST_STROKE,
            SKIING,
            TOUCH_PAD,
            PULLEY
            // DEVELOPER: IF YOU ADD YOUR OWN MOVEMENT, DONT FORGET TO UPDATE THE ENUM!
        }
        
        public const string  RESOURCES_PATH      = @"Assets/Locomotion Essentials/VR/Resources/";
        public const string  SETTINGS_FILE_PATH  = RESOURCES_PATH + @"Settings/Settings.asset";
        public static bool   RigInScene          = false;
        public static string BUTTON_TEXT         = "Find & Setup Locomotion";

        #endregion

        #region Public Methods

        public static VRLocomotionSettings GetLocomotionSettings () {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath(SETTINGS_FILE_PATH, typeof(VRLocomotionSettings)) as VRLocomotionSettings;
#else
            Debug.Log("GET GESTURE SETTINGS FROM RESOURCES");
            return Resources.Load(Utility.SETTINGS_FILE_PATH + "Settings/Settings", typeof(VRLocomotionSettings)) as VRLocomotionSettings;
#endif
        }
        public static void ChangeVRType (eVRType type) {
            GetLocomotionSettings().vrType = type;
        }
        public static void ChangeMovementType (eMovementType type) {
            GetLocomotionSettings().movementType = type;
        }
        public static void ChangeIsInScene () {
            RigInScene = !RigInScene;
        }

        #endregion  
    }
}

#if UNITY_EDITOR
namespace Loco.VR {
    public class VRLocomotionSettingsWindow : EditorWindow {

        #region Public Variables

        public VRLocomotionSettings LocomotionSettings;
        public SerializedObject SerializedObject;

        #endregion

        #region Private Variables

        private int        m_maxWidth           = 600;
        private bool       m_showSettingsGUI    = true;
        private GameObject m_steamVRrig         = null;
        private GameObject m_rig                = null;
        private HeadBob    m_headBob            = null;
        private ArmSwing   m_armSwing           = null;
        private Pulley     m_pulley             = null;
        private Touchpad   m_tPad               = null;

        #endregion

        #region Window Initialization

        [MenuItem("Tools/VR Locomotion Essentials/Settings")]
        public static void Init () {
            // get exisiting open window or if none make one
            VRLocomotionSettingsWindow window = (
                VRLocomotionSettingsWindow)EditorWindow.GetWindow<VRLocomotionSettingsWindow>(
                    false,
                    "VR Locomotion",
                    true );
        }

        #endregion

        #region Unity Methods

        void OnEnable () {
            hideFlags = HideFlags.HideAndDontSave;
            GetSetLocomotionSettings();
            SetSerializedObject();
        }

        void OnGUI () {
            SerializedObject.Update();

            GetSetLocomotionSettings();
            SetSerializedObject();

            GUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.currentViewWidth));

            GUILayout.Space(5);
            ShowToolbarContent();

            GUILayout.EndVertical();

            SerializedObject.ApplyModifiedProperties();
        }

        void OnDestroy () {
            if ( Utility.GetLocomotionSettings() != null ) {
                EditorUtility.SetDirty(LocomotionSettings);
                AssetDatabase.SaveAssets();
            }
        }

        #endregion

        #region Private Methods

        private void GetSetLocomotionSettings () {
            if ( Utility.GetLocomotionSettings() == null ) {
                LocomotionSettings = CreateLocomotionSettingsAsset();
            } else {
                LocomotionSettings = Utility.GetLocomotionSettings();
            }
        }       
        private void SetSerializedObject () {
            if ( SerializedObject == null ) {
                SerializedObject = new SerializedObject(LocomotionSettings);
            }
        }
        private void ShowSettingsTab () {
            EditorGUILayout.Separator();
            SerializedProperty vrType = SerializedObject.FindProperty("vrType");
            SerializedProperty movementType = SerializedObject.FindProperty("movementType");
            SerializedProperty startMoveSpeed = SerializedObject.FindProperty("startMoveSpeed");
            SerializedProperty rigAlreadyInScene = SerializedObject.FindProperty("haveRigPrefab");
            SerializedProperty vrRig = SerializedObject.FindProperty("vrRig");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(vrType);
            if ( EditorGUI.EndChangeCheck() ) {
                Utility.ChangeVRType((Utility.eVRType) vrType.enumValueIndex);
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(movementType);
            if ( EditorGUI.EndChangeCheck() ) {
                Utility.ChangeMovementType((Utility.eMovementType) movementType.enumValueIndex);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Start Movement Speed");
            LocomotionSettings.startMoveSpeed = (float) EditorGUILayout.Slider((float) startMoveSpeed.floatValue, 1.0f, 4.0f);
            EditorGUILayout.EndHorizontal();

            if ( !Utility.RigInScene ) {
                EditorGUILayout.PropertyField(vrRig);
                Utility.BUTTON_TEXT = "Find & Setup Locomotion";

            } else {
                Utility.BUTTON_TEXT = "Update Rig In Scene";
            }

            if ( GUILayout.Button(Utility.BUTTON_TEXT) ) {
                if ( Utility.BUTTON_TEXT.Equals("Find & Setup Locomotion") && !Utility.RigInScene ) {

                    try {
                        LocomotionSettings.vrRig = PrefabUtility.GetPrefabParent(FindObjectOfType<SteamVR_ControllerManager>().gameObject) as GameObject;
                    } catch {
                        Debug.LogError("SETUP ERROR: 'missing camera rig' Please drag in the [CameraRig] prefab provided by SteamVR and try again!");
                        return;
                    }
                    VRLocomotionRig vrLR;

                    FindAndDisableRig();                                        // finds and disables the steamVR rig
                    CreateLocoRig("[Camera Rig] - LOCO");                       // Instantiating a custom rig called "[Camera Rig] - LOCO"               
                    AddNavMesh(0.5f, 1.5f);                                     // Adding the nav mesh component and setting its height and radius              
                    AddLocomotionRig(out vrLR);                                 // Adding the VRLocomotionRig and running the AutoSetup method to attach all the needed components
                    AddCapsuleCollider(0.5f, 1.5f, new Vector3(0, 0.68f, 0));   // Adding the capsule collider component and setting up its radius, height, and center offset
                    AddRigidbody(false, true);                                  // Adding the rigidbody component and setting gravity to false and kinimetic to true
                    AddVrCharController();                                      // Adds the VR character controller to allow for realtime switching
                    AddMovementComponents(vrLR);                                // Adds the required movement logic for HeadBob, ArmSwing and Pulley

                    vrLR.s_handLeft.gameObject.AddComponent<VRBasicController>();
                    vrLR.s_handRight.gameObject.AddComponent<VRBasicController>();

                    //-----------------------------------------------------------------------------------------------------
                    //                    Enables the appropriate movement logic, based on your settings
                    //-----------------------------------------------------------------------------------------------------
                    switch ( LocomotionSettings.movementType ) {
                        case Utility.eMovementType.BOBBING: m_headBob.enabled = true; m_armSwing.enabled = false; m_pulley.enabled = false; break;
                        case Utility.eMovementType.ARM_SWING:  m_headBob.enabled = false; m_armSwing.enabled = true; m_pulley.enabled = false; m_armSwing.motionType = ArmSwing.eMotionType.ARM_SWING; break;      
                        case Utility.eMovementType.BREAST_STROKE: m_headBob.enabled = false; m_armSwing.enabled = true; m_pulley.enabled = false; m_armSwing.motionType = ArmSwing.eMotionType.BREAST_STROKE; break;
                        case Utility.eMovementType.SKIING: m_headBob.enabled = false; m_armSwing.enabled = true; m_pulley.enabled = false; m_armSwing.motionType = ArmSwing.eMotionType.SKIING; break;
                        case Utility.eMovementType.PULLEY: m_headBob.enabled = false; m_armSwing.enabled = false; m_pulley.enabled = true; break;
                        // DEVELOPER: IF YOU ADD YOUR OWN, DONT FORGET TO ADD A CASE TO SWITCH IT ON AND OFF
                    }
                    //----------------------------------------------------------------------------------------------------

                    Utility.RigInScene = true;
                } else if ( Utility.RigInScene ) {
                    VRLocomotionRig vrLocoRig = null;
                    if ( !m_steamVRrig.activeInHierarchy ) {
                        try {
                            vrLocoRig = FindObjectOfType<SteamVR_ControllerManager>().gameObject.GetComponent<VRLocomotionRig>();
                        } catch {
                            vrLocoRig = null;
                            m_steamVRrig.SetActive(true);
                        }
                    }

                    if ( vrLocoRig == null ) {
                        Utility.RigInScene = false;
                        LocomotionSettings.vrRig = PrefabUtility.GetPrefabParent(m_steamVRrig.gameObject) as GameObject;

                        //FIXME: add new functions to replace the old redundant code below!
                        GameObject rig = Instantiate(LocomotionSettings.vrRig) as GameObject;
                        rig.name = "[Camera Rig] - LOCO";
                        UnityEngine.AI.NavMeshAgent ag = rig.AddComponent<UnityEngine.AI.NavMeshAgent>();
                        ag.radius = 1;
                        ag.height = 1.5f;
                        VRLocomotionRig vrLR = rig.AddComponent<VRLocomotionRig>();
                        vrLR.AutoSetup();
                        HeadBob  hd = vrLR.s_head.gameObject.AddComponent<HeadBob>();
                        ArmSwing ar = vrLR.s_head.gameObject.AddComponent<ArmSwing>();
                        Pulley   pl = vrLR.s_head.gameObject.AddComponent<Pulley>();
                        vrLR.s_head.gameObject.AddComponent<SteamVR_UpdatePoses>();

                        vrLR.s_handLeft.gameObject.AddComponent<VRBasicController>();
                        vrLR.s_handRight.gameObject.AddComponent<VRBasicController>();

                        switch ( LocomotionSettings.movementType ) {
                            case Utility.eMovementType.BOBBING: hd.enabled = true; ar.enabled = false; pl.enabled = false; break;
                            case Utility.eMovementType.ARM_SWING: hd.enabled = false; ar.enabled = true; pl.enabled = false; break;
                            case Utility.eMovementType.PULLEY: hd.enabled = false; ar.enabled = false; pl.enabled = true; break;
                        }

                        vrLocoRig = rig.GetComponent<VRLocomotionRig>();
                        m_steamVRrig.SetActive(false);
                        Utility.RigInScene = true;
                    }

                    HeadBob  h = vrLocoRig.s_head.gameObject.GetComponent<HeadBob>();
                    ArmSwing a = vrLocoRig.s_head.gameObject.GetComponent<ArmSwing>();
                    Pulley   p = vrLocoRig.s_head.gameObject.GetComponent<Pulley>();
                    switch ( LocomotionSettings.movementType ) {
                        case Utility.eMovementType.BOBBING: h.enabled = true; a.enabled = false; p.enabled = false; break;
                        case Utility.eMovementType.ARM_SWING: h.enabled = false; a.enabled = true; p.enabled = false; break;
                        case Utility.eMovementType.PULLEY: h.enabled = false; a.enabled = false; p.enabled = true; break;
                    }

                    m_steamVRrig.SetActive(false);
                }
            }

            if ( GUILayout.Button("Click to reset Settings & Scene") ) {
                Utility.RigInScene = false;
                try {
                    m_steamVRrig.SetActive(true);
                    //try {
                    GameObject o = GameObject.Find("[Camera Rig] - LOCO");
                    DestroyImmediate(o);
                    DestroyImmediate(m_steamVRrig);

                } catch {
                    Debug.LogWarning("WARNING: can't reset! it has already been reset! drag the CameraRig provided by steam VR into your hierarchy and click the button above to get started");
                }
            }
        }
        private void ShowToolbarContent () {
            if ( m_showSettingsGUI )
                ShowSettingsTab();
        }
        private void FindAndDisableRig () {
            try {
                m_steamVRrig = FindObjectOfType<SteamVR_ControllerManager>().gameObject;  // setting up a reference to the steamVRrig for later
                FindObjectOfType<SteamVR_ControllerManager>().gameObject.SetActive(false); // setting it to be inactive in the scene
            } catch {
                Debug.LogError("ERROR: This asset depends on SteamVR! please download and import the steamVR plugin!");
            }
        }

        // Wraps up simple logic to keep code clean. Gives me room to expand off these methods
        private void CreateLocoRig (string a_name) {
            m_rig = Instantiate(LocomotionSettings.vrRig) as GameObject;
            m_rig.name = "[Camera Rig] - LOCO";
        }
        private void AddNavMesh (float a_radius, float a_height) {
            UnityEngine.AI.NavMeshAgent ag = m_rig.AddComponent<UnityEngine.AI.NavMeshAgent>();
            ag.radius = a_radius;
            ag.height = a_height;
        }
        private void AddLocomotionRig (out VRLocomotionRig a_vrLR) {
            a_vrLR = m_rig.AddComponent<VRLocomotionRig>();
            a_vrLR.AutoSetup();
        }
        private void AddCapsuleCollider (float a_radius, float a_height, Vector3 a_center) {
            CapsuleCollider col = m_rig.AddComponent<CapsuleCollider>();
            col.radius = a_radius;
            col.height = a_height;
            col.center = a_center;
        }
        private void AddRigidbody(bool a_useGravity, bool a_isKinematic) {
            Rigidbody rb = m_rig.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
        }
        private void AddVrCharController () {
            VRCharacterController vrCharCont = m_rig.AddComponent<VRCharacterController>();
            // Setup variable values below?
            // ______
            // ______
        }
        private void AddMovementComponents (VRLocomotionRig a_vrLR) {
            a_vrLR.s_head.gameObject.AddComponent<SteamVR_UpdatePoses>();
            m_headBob = a_vrLR.s_head.gameObject.AddComponent<HeadBob>();  // Adds the HeadBob movement logic component
            m_armSwing = a_vrLR.s_head.gameObject.AddComponent<ArmSwing>(); // Adds the ArmSwing movement logic component
            m_pulley = a_vrLR.s_head.gameObject.AddComponent<Pulley>();   // Adds the Pulley movement logic component
            m_tPad = a_vrLR.s_head.gameObject.AddComponent<Touchpad>();
            // DEVELOPER: ADD YOUR OWN CUSTOM VR MOVEMENT COMPONENT HERE!
        }

        #endregion

        #region Public Methods

        public static VRLocomotionSettings CreateLocomotionSettingsAsset() {
            VRLocomotionSettings instance = CreateInstance<VRLocomotionSettings>();
            AssetDatabase.CreateAsset(instance, Utility.SETTINGS_FILE_PATH);
            return instance;
        }

        #endregion
    }
}
#endif
