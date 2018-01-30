using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Loco.VR.Input;
using Loco.VR.Movement;
using UnityEngine.AI;

/// <summary>
/// Blink step. (WIP) - not yet functional, will be soon!!
/// </summary>

namespace Loco.VR.Movement {
	public class BlinkStep : MonoBehaviour {

		[Header("Controller Settings")]
		[SerializeField] private VRBasicController m_blinkController;
		[SerializeField] private VRBasicController.ViveButton m_blinkButton;

		[Header("Blink Settings")]
		[SerializeField] private Transform m_blinkPointRef;
		[SerializeField] private Vector3 m_blinkDistanceOffset;
		[SerializeField] private float m_blinkDelay;

		private float m_blinkTimer = 0.0f;
		private bool m_canBlink = false;

		// Use this for initialization
		void Start () {
			m_blinkPointRef.position += m_blinkDistanceOffset;
		}
		
		// Update is called once per frame
		void Update () {
			if(!m_canBlink) {
				m_blinkTimer += Time.deltaTime;
				if(m_blinkTimer >= m_blinkDelay) {
					m_canBlink = true;
					m_blinkTimer = 0.0f;
				}
			} else {
				// BLINK
				// SET CAN BLINK TO FALSE
			}
		}

		void UpdateBlinkStep(){
			// DISABLE NAV MESH
			transform.root.GetComponent<NavMeshAgent>().enabled = false;

			// BLINK AS LONG AS THERE IS FLOOR BENEATH THE OFFSET
			Ray ray = new Ray(m_blinkPointRef.position, -m_blinkPointRef.up);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 20)){
				if(hit.transform.tag == "Terrain") {
					transform.root.position  = hit.point;
				}
			}

			// ENABLE NAV MESH
			transform.root.GetComponent<NavMeshAgent>().enabled = true;

			// CANNOT BLINK ANYMORE
			m_canBlink = false;
		}
	}
}