//          ======= Copyright (c) BlackPandaStudios, All rights reserved. =======
//              _____________________________________________________________ 
// <author>    | -  Joel Gabriel                                             |  
// <date>      | -  05/05/2017                                               |  
// <name>      | -  RenderTeleportArc.cs                                     |  
// <summary>   | -  This class handles the logic for rendering the teleport- |            
//             |    ing arc as well as move-ing the VR rig itself to that    |
//             |    location                                                 |  
//             |_____________________________________________________________|

using UnityEngine;
using Loco.VR.Input;

namespace Loco.VR.Movement
{
    public class RenderTeleportArc : MonoBehaviour {

        [Header("Controller Settings")]        
        [SerializeField] private VRBasicController.ViveButton m_buttonID;

        [Header("Arc Settings")]
        [Tooltip("The distance the line will go")][SerializeField] private float arcLength = 10.0f;      
        [Tooltip("The point in space in which the line will cut off")][SerializeField] private float groundLevel = 0.07391325f;

        [HideInInspector] private float timeResolution = 0.02f;
        [HideInInspector] private float maxTime = 10.0f;

        private LineRenderer lineRenderer;
        private VRBasicController thisController;
        private bool clicking = false;

        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            thisController = GetComponent<VRBasicController>();
        }

        void FixedUpdate()
        {
			if (thisController.GetButton((Valve.VR.EVRButtonId)m_buttonID))
            {
                clicking = true;
                lineRenderer.enabled = true;
                Vector3 velocityVector = transform.forward * arcLength;
#if UNITY_5_6
                lineRenderer.positionCount = (int)(maxTime / timeResolution) / 2;
#else 
                lineRenderer.positionCount = (int)(maxTime / timeResolution) / 2;
#endif

                int index = 0;

                Vector3 currentPosition = transform.position;

                for (float t = 0.0f; t < maxTime / 2; t += timeResolution)
                {
                    lineRenderer.SetPosition(index, currentPosition);
                    currentPosition += velocityVector * timeResolution;
                    if (currentPosition.y < -1.3f)
                    {
#if UNITY_5_6
                        lineRenderer.positionCount -= (lineRenderer.positionCount - index);
#else
                        lineRenderer.positionCount -= (lineRenderer.positionCount - index);
#endif

                        break;

                    }
                    velocityVector += Physics.gravity;
                    index++;
                }
            }
            else
            {
                lineRenderer.enabled = false;
                if (clicking == true)
                {
#if UNITY_5_6
                    transform.root.position = new Vector3(lineRenderer.GetPosition(lineRenderer.positionCount - 1).x, groundLevel, lineRenderer.GetPosition(lineRenderer.positionCount - 1).z);
#else
                    transform.root.position = new Vector3(lineRenderer.GetPosition(lineRenderer.positionCount - 1).x, groundLevel, lineRenderer.GetPosition(lineRenderer.positionCount - 1).z);
#endif
                    clicking = false;
                }
            }

        }
    }
}
