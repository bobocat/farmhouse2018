using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Loco.VR.Input;
using Loco.VR.Movement;

/// <summary>
/// VRUI pointer (WIP) - not yet functional, but no errors that will break anything
/// </summary>

namespace Loco.VR.Input {
	public class VRUIPointer : MonoBehaviour {

		[SerializeField] private Transform m_pointerPosOverride;
		[SerializeField] private LayerMask m_layerMask;

		private Transform m_head;
		private VRBasicController m_controller;

		private VRBasicController.ViveButton m_buttonToRender = VRBasicController.ViveButton.TOUCHPAD;
		private VRBasicController.ViveButton m_buttonToInteract = VRBasicController.ViveButton.GRIP;

		private Transform m_pointerPos;

		// Use this for initialization
		void Start () {
			m_head = transform.root.GetComponentInChildren<HeadBob>().transform;

			if(m_pointerPosOverride!= null) {
				m_pointerPos = m_pointerPosOverride;
			} else {
				m_pointerPos = transform;
			}
			m_controller = GetComponent<VRBasicController>();
		}
		
		// Update is called once per frame
		void Update () {
			if(m_controller.GetButtonDown((Valve.VR.EVRButtonId)m_buttonToRender)) {
				Ray ray = new Ray(m_pointerPos.position, m_pointerPos.forward); 
				RaycastHit hit;

				if(Physics.Raycast(ray, out hit, Mathf.Infinity, m_layerMask)) {
					if(m_controller.GetButtonDown((Valve.VR.EVRButtonId)m_buttonToInteract)) {
						if(hit.transform.GetComponent<Button>() != null) {
							hit.transform.GetComponent<Button>().onClick.Invoke();
						}
					}
				}
			}

			if(Vector3.Cross(m_head.forward, transform.up) == Vector3.zero) {
				// Activate UI		
			}
		}
	}
}